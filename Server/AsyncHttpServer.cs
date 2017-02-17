using System;
using System.Net;
using System.Threading;
using log4net;

namespace HttpServer.Server
{
    public class AsyncHttpServer : IHttpServer
    {
        private readonly ILog log;
        private readonly IRequestHandler handler;
        private readonly AuthenticationSchemes authenticationSchemes;
        private readonly Func<Uri, AuthenticationSchemes> authenticationSelector;
        private readonly object locker = new object();

        private CancellationTokenSource tokenSource;

        public bool IsRunning => tokenSource != null;


        public AsyncHttpServer(IRequestHandler handler, ILog log,
            AuthenticationSchemes authenticationSchemes = AuthenticationSchemes.Anonymous,
            Func<Uri, AuthenticationSchemes> authenticationSelector = null)
        {
            Preconditions.EnsureNotNull(log, "log");
            Preconditions.EnsureNotNull(handler, "handler");
            this.log = log;
            this.handler = handler;
            this.authenticationSchemes = authenticationSchemes;
            this.authenticationSelector = authenticationSelector ?? (_ => authenticationSchemes);
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
                var listener = new Thread(t => Listen(prefix, (CancellationToken)t))
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest
                };
                listener.Start(tokenSource.Token);
            }
        }

        [Obsolete]
        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            tokenSource?.Cancel();
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
            token.Register(listener.Stop);
            listener.Start();
            log.Info("Server is running");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        HandleContextAsync(context, token);
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
                    log.Error($"Network error while getting context. Code: {exception.ErrorCode}", exception);
                }
                catch (Exception exception)
                {
                    log.Error($"Error in getting context: {exception.Message}", exception);
                }
            }
        }

        private async void HandleContextAsync(HttpListenerContext listenerContext, CancellationToken token)
        {
            if (listenerContext == null)
            {
                return;
            }
            var handlerLog = log.WithPrefix("RE-" + listenerContext.Request.GetHashCode());
            var context = new ListenerContext(listenerContext, handlerLog);
            try
            {
                await handler.HandleContextAsync(context, handlerLog, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    handlerLog.Error($"Error in handling request: {exception.Message}. Request: {context.Request}.", exception);
                }
                if (context.Response.ResponseInitiated)
                {
                    return;
                }
                try
                {
                    await context.Response.RespondAsync(new HttpServerResponse(HttpStatusCode.InternalServerError))
                        .WithCancellation(token)
                        .ConfigureAwait(false);
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