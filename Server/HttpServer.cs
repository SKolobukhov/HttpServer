using System;
using System.Net;
using System.Threading;
using log4net;

namespace HttpServer.Server
{
    public class HttpServer : IHttpServer
    {
        private readonly ILog log;
        private readonly AuthenticationSchemes authenticationSchemes;
        private readonly Func<Uri, AuthenticationSchemes> authenticationSelector;

        private readonly object locker = new object();
        private CancellationTokenSource tokenSource;
        private volatile bool running;
        private bool disposed;

        private readonly ContextQueue contextQueue;
        private readonly ThreadPool threadPool;


        public bool IsRunning => running;


        public HttpServer(int throttling, IRequestHandler handler, ILog log,
            AuthenticationSchemes authenticationSchemes = AuthenticationSchemes.Anonymous,
            Func<Uri, AuthenticationSchemes> authenticationSelector = null)
        {
            Preconditions.EnsureNotNull(log, "log");
            Preconditions.EnsureNotNull(handler, "handler");
            Preconditions.EnsureCondition(throttling > 0, "throttling", "Worker threads count must be > 0.");
            this.log = log;
            this.authenticationSchemes = authenticationSchemes;
            this.authenticationSelector = authenticationSelector;
            contextQueue = new ContextQueue(log.WithPrefix("ContextQueue"));
            threadPool = new ThreadPool(throttling,
                (token, workerLog) => ProcessContextsRoutine(contextQueue, handler, token, workerLog),
                log.WithPrefix("Workers"));
        }

        public void Start(ushort port = 80, HttpScheme scheme = HttpScheme.Http, CancellationToken? token = null)
        {
            if (IsRunning) return;
            var prefix = string.Format("{0}://+:{1}/", scheme.ToString().ToLower(), port);
            tokenSource = new CancellationTokenSource();
            token?.Register(() => tokenSource.Cancel());
            tokenSource.Token.Register(StopServer);
            StartServer(prefix, tokenSource.Token);
        }

        [Obsolete]
        public void Stop()
        {
            if (IsRunning)
            {
                tokenSource?.Cancel();
            }
        }

        private void StartServer(string prefix, CancellationToken token)
        {
            lock (locker)
            {
                if (!IsRunning)
                {
                    contextQueue.Start(token);
                    var listenerThread =
                        new Thread(
                            () =>
                                Listen(prefix, authenticationSchemes, authenticationSelector, token,
                                    log.WithPrefix("Listener"), contextQueue))
                        {
                            IsBackground = true,
                            Priority = ThreadPriority.Highest,
                            Name = "HttpServer-Listener-" + Guid.NewGuid()
                        };
                    listenerThread.Start();
                    threadPool.Start(token);
                    running = true;
                    log.Info($"Start listening for prefix {prefix}");
                }
            }
        }

        private void StopServer()
        {
            lock (locker)
            {
                if (IsRunning)
                {
                    tokenSource = null;
                    running = false;
                    log.Info("Stop listening");
                }
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            if (IsRunning)
            {
                tokenSource?.Cancel();
            }
            threadPool.Dispose();
        }

        private static void Listen(string prefix, AuthenticationSchemes authenticationSchemes,
            Func<Uri, AuthenticationSchemes> authenticationSelector,
            CancellationToken token, ILog log, ContextQueue contextQueue)
        {
            var listener = new HttpListener
            {
                IgnoreWriteExceptions = false,
                AuthenticationSchemes = authenticationSchemes,
                Prefixes = {prefix}
            };
            if (authenticationSelector != null)
            {
                listener.AuthenticationSchemeSelectorDelegate = request => authenticationSelector(request.Url);
            }
            listener.Start();
            token.Register(listener.Stop);
            log.Info("Listener is running");
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        contextQueue.Enqueue(context);
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }
                catch (HttpListenerException exception)
                {
                    if (exception.ErrorCode == 995 || exception.ErrorCode == 6)
                    {
                        break;
                    }
                    log.Error($"Network error while getting context. Code: {exception.ErrorCode}", exception);
                }
                catch (Exception exception)
                {
                    log.Error($"Error in getting context: {exception.Message}", exception);
                }
            }
            log.Info("Listener is stopped");
        }

        private static void ProcessContextsRoutine(ContextQueue contextQueue, IRequestHandler handler, CancellationToken token, ILog log)
        {
            while (!token.IsCancellationRequested)
            {
                var listenerContext = contextQueue.Dequeue(token);
                if (listenerContext == null)
                {
                    return;
                }

                var prefix = "RE-" + listenerContext.Request.GetHashCode();
                var handlerLog = log.WithPrefix(prefix);
                var context = new ListenerContext(listenerContext, handlerLog);
                try
                {
                    handler.HandleContextAsync(context, handlerLog, token).Wait(token);
                }
                catch (Exception exception)
                {
                    handlerLog.Error($"Error in handling request that caused failure: {exception.Message}.\r\n'{context.Request}'", exception);
                    if (context.Response.ResponseInitiated)
                    {
                        continue;
                    }
                    try
                    {
                        context.Response.RespondAsync(new HttpServerResponse(HttpStatusCode.InternalServerError)).Wait(token);
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception anotherException)
                    {
                        handlerLog.Error($"Error responding to request that caused failure: {anotherException.Message}.", anotherException);
                    }
                }
            }
        }
    }
}