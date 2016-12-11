using System;
using System.Net;
using System.Threading;
using HttpServer.Common;
using log4net;
using log4net.Core;

namespace HttpServer.Server
{
    public class HttpServer : IHttpServer
    {
        private readonly ILog log;
        private readonly HttpListener listener;
        private readonly ThreadPool threadPool;
        private readonly ContextQueue contextQueue;
        private bool disposed;
        private Thread listenerThread;
        private volatile bool running;
        private IRequestHandler handler;
        private CancellationTokenSource tokenSource;

        public bool IsRunning => running;


        public HttpServer(int throttling, IRequestHandler handler, AuthenticationSchemes authenticationSchemes, ILog log)
        {
            Preconditions.EnsureNotNull(log, "log");
            //Preconditions.EnsureNotNull(handler, "handler");
            Preconditions.EnsureCondition(throttling > 0, "throttling", "Worker threads count must be > 0.");
            this.log = log;
            this.handler = handler;
            listener = new HttpListener
            {
                IgnoreWriteExceptions = false,
                AuthenticationSchemes = authenticationSchemes
            };
            threadPool = new ThreadPool(throttling, ProcessContextsRoutine, log.WithPrefix("Workers"));
            contextQueue = new ContextQueue(log.WithPrefix("ContextQueue"));
        }

        public void Start(ushort port = 80, HttpScheme scheme = HttpScheme.Http, CancellationToken? token = null)
        {
            if (IsRunning) return;
            var prefix = string.Format("{0}://+:{1}/", scheme.ToString().ToLower(), port);
            tokenSource = new CancellationTokenSource();
            token?.Register(() => tokenSource.Cancel());
            tokenSource.Token.Register(StopServer);
            StartServer(prefix);
        }

        [Obsolete]
        public void Stop()
        {
            tokenSource?.Cancel();
        }

        private void StartServer(string prefix)
        {
            lock (listener)
            {
                if (!IsRunning)
                {
                    contextQueue.Start(tokenSource.Token);
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();
                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest,
                        Name = "HttpServer-Listener-" + Guid.NewGuid()
                    };
                    listenerThread.Start();
                    threadPool.Start(tokenSource.Token);
                    running = true;
                    log.Info($"Start listening for prefix {prefix}");
                }
            }
        }

        private void StopServer()
        {
            lock (listener)
            {
                if (IsRunning)
                {
                    listener.Stop();
                    listenerThread.Abort();
                    listenerThread.Join();
                    tokenSource = null;
                    running = false;
                    log.Info("Stop listening");
                }
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            if (IsRunning)
            {
                StopServer();
            }
            listener.Close();
            threadPool.Dispose();
        }

        public void SetAuthenticationSelector(Func<Uri, AuthenticationSchemes> authenticationSelector)
        {
            listener.AuthenticationSchemeSelectorDelegate = request => authenticationSelector(request.Url);
        }

        private void Listen()
        {
            var log = this.log.WithPrefix("Listener");
            log.Info("Listener is running");
            while (!tokenSource.Token.IsCancellationRequested)
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
                catch (ThreadAbortException)
                {
                    break;
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

        private void ProcessContextsRoutine(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var listenerContext = contextQueue.Dequeue();
                if (listenerContext == null)
                {
                    return;
                }

                var prefix = "RE-" + listenerContext.Request.GetHashCode();
                var handlerLog = new LogWithPrefix(log, prefix);
                var context = new ListenerContext(listenerContext, handlerLog);
                try
                {
                    handler.HandleContext(context, handlerLog, token);
                }
                catch (Exception exception)
                {
                    handlerLog.Error($"Error in handling request from {context.Request.Request.RemoteEndPoint}. Request: '{context.Request}'", exception);
                    if (context.Response.ResponseInitiated)
                    {
                        continue;
                    }
                    try
                    {
                        context.Response.Respond(new HttpServerResponse(HttpStatusCode.InternalServerError));
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