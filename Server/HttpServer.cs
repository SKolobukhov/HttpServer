using System;
using System.Net;
using System.Threading;
using log4net;

namespace HttpServer.Server
{
    public class HttpServer : IHttpServer
    {
        private readonly ILog log;
        private readonly IRequestHandler handler;
        private readonly ThreadPool threadPool;
        private readonly FiexdBuffer<HttpListenerContext> buffer;
        private readonly AuthenticationSchemes authenticationSchemes;
        private readonly Func<Uri, AuthenticationSchemes> authenticationSelector;
        private readonly object locker = new object();

        private CancellationTokenSource tokenSource;

        public bool IsRunning => tokenSource != null;


        public HttpServer(int throttling, IRequestHandler handler, ILog log,
            AuthenticationSchemes authenticationSchemes = AuthenticationSchemes.Anonymous,
            Func<Uri, AuthenticationSchemes> authenticationSelector = null)
        {
            Preconditions.EnsureNotNull(log, "log");
            Preconditions.EnsureNotNull(handler, "handler");
            Preconditions.EnsureCondition(throttling > 0, "throttling", "Worker threads count must be > 0.");
            this.log = log;
            this.handler = handler;
            buffer = new FiexdBuffer<HttpListenerContext>();
            this.authenticationSchemes = authenticationSchemes;
            this.authenticationSelector = authenticationSelector ?? (_ => authenticationSchemes);
            threadPool = new ThreadPool(throttling, ProcessContextsRoutine, log);
        }

        public void Start(int port = 80, CancellationToken? token = null)
        {
            Preconditions.EnsureCondition(port > 0 && port <= 65535, "port");
            if (IsRunning) return;
            var prefix = string.Format("http://+:{0}/", port);
            tokenSource = new CancellationTokenSource();
            token?.Register(() => tokenSource.Cancel());
            tokenSource.Token.Register(() =>
            {
                lock (locker)
                {
                    tokenSource = null;
                    log.Info("Server is stopped");
                }
            });
            lock (locker)
            {
                new Thread(t => Listen(prefix, (CancellationToken)t))
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest,
                    Name = "HttpServer-Listener-" + Guid.NewGuid()
                }
                .Start(tokenSource.Token);
                threadPool.Start(tokenSource.Token);
                log.Info("Server is running");
            }
        }

        [Obsolete]
        public void Stop()
        {
            tokenSource?.Cancel();
        }

        public void Dispose()
        {
            tokenSource?.Cancel();
            threadPool.Dispose();
        }

        private void Listen(string prefix, CancellationToken token)
        {
            var listener = new HttpListener
            {
                Prefixes = { prefix },
                IgnoreWriteExceptions = false,
                AuthenticationSchemes = authenticationSchemes,
                AuthenticationSchemeSelectorDelegate = r => authenticationSelector(r.Url)
            };

            var listenerLog = log.WithPrefix("Listener");
            listener.Start();
            token.Register(listener.Stop);
            listenerLog.Info("Listener is running");
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        buffer.Enqueue(context, token);
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }
                catch (OperationCanceledException) { }
                catch (HttpListenerException exception)
                {
                    if (exception.ErrorCode == 995 || exception.ErrorCode == 6)
                    {
                        break;
                    }
                    listenerLog.Error($"Network error while getting context. Code: {exception.ErrorCode}", exception);
                }
                catch (Exception exception)
                {
                    listenerLog.Error($"Error in getting context: {exception.Message}", exception);
                }
            }
            listenerLog.Info("Listener is stopped");
        }

        private void ProcessContextsRoutine(ILog log, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var listenerContext = buffer.Dequeue(token);
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
                catch (OperationCanceledException) { }
                catch (AggregateException aggregateException)
                {
                    foreach (var exception in aggregateException.InnerExceptions)
                    {
                        handlerLog.Error($"Error in handling request: {exception.Message}. Request:\r\n{context.Request}", exception);
                    }
                    if (context.Response.ResponseInitiated)
                    {
                        continue;
                    }
                    try
                    {
                        context.Response.RespondAsync(new HttpServerResponse(HttpStatusCode.InternalServerError)).Wait();
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception anotherException)
                    {
                        handlerLog.Error($"Error responding to request: {anotherException.Message}.", anotherException);
                    }
                }
            }
        }
    }
}