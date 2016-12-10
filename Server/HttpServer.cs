using System;
using System.IO;
using System.Net;
using System.Threading;
using HttpServer.Common;
using log4net;

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
        private CancellationTokenSource tokenSource;

        public bool IsRunning => running;


        public HttpServer(int throttling, IRequestHandler handler, AuthenticationSchemes authenticationSchemes, ILog log)
        {
            Preconditions.EnsureNotNull(log, "log");
            //Preconditions.EnsureNotNull(handler, "handler");
            Preconditions.EnsureArgumentRange(throttling > 0, "throttling", "Worker threads count must be > 0.");
            this.log = log;
            disposed = false;
            //this.handler = handler;
            listener = new HttpListener
            {
                IgnoreWriteExceptions = false,
                AuthenticationSchemes = authenticationSchemes
            };
            threadPool = new ThreadPool(throttling, ProcessContextsRoutine, log);
            contextQueue = new ContextQueue();
            tokenSource = null;
            running = false;
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
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();
                    contextQueue.Start(tokenSource.Token);
                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest,
                        Name = "HttpServer-Listener-" + Guid.NewGuid()
                    };
                    listenerThread.Start(tokenSource.Token);
                    threadPool.Start(tokenSource.Token);
                    running = true;
                    log.Info($"Started listening for prefix {prefix}");
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
                    log.Info("Stopped listening");
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

        private void Listen(object tokenObject)
        {
            var token = (CancellationToken)tokenObject;
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
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (HttpListenerException exception)
                {
                    if (exception.ErrorCode == 995 || exception.ErrorCode == 6)
                    {
                        return;
                    }
                    log.Error($"Network error while getting context. Code: {exception.ErrorCode}", exception);
                }
                catch (Exception exception)
                {
                    log.Error($"Error in getting context: {exception.Message}", exception);
                }
            }
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


                /*string traceId;
                string traceSpanId;
                string traceParentSpanId;
                string traceProfileId;
                bool? traceSampled;
                TraceDataExtractor.ExtractFromHttpHeaders(listenerContext.Request.Headers, out traceId, out traceSpanId, out traceParentSpanId, out traceProfileId, out traceSampled, log);
                using (var traceScope = Trace.DefineContext(traceId, traceSpanId, traceParentSpanId, traceProfileId, traceSampled, log))
                using (var spanWriter = Trace.BeginSpan(TraceSpanKind.Server, log))
                using (new ContextualLogPrefix(TraceIdentifierGenerator.CreateLogPrefix(traceScope.Context)))
                {
                    var context = new ListenerContext(listenerContext, spanWriter, log);
                    // (iloktionov): Здесь - не самое честное место для фиксирования момента server-receive (запрос мог долго лежать в очереди).
                    // (iloktionov): Однако, мы закрываем на это глаза из-за крайне малой распространенности синхронного сервера.
                    context.RequestWrapper.TraceWith(spanWriter, log);
                    try
                    {
                        handler.HandleContext(context);
                    }
                    catch (Exception error)
                    {
                        LogHandlerException(error, context);
                        // (iloktionov): Если исключение произошло непосредственно при отсылке ответа, мы уже ничего не можем сделать.
                        if (context.Response.ResponseInitiated)
                            continue;
                        try
                        {
                            context.Response.Respond(HttpResponseCode.InternalServerError);
                        }
                        catch (ObjectDisposedException) { }
                        catch (Exception anotherError)
                        {
                            LogFailureInInternalErrorResponse(anotherError);
                        }
                    }
                }*/
            }
        }
    }
}