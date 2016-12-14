using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace HttpServer.Server
{
    public class RoutingHandler : IRequestHandler
    {
        private readonly RouteTable routeTable;

        public RoutingHandler(RouteTable routeTable)
        {
            this.routeTable = routeTable;
        }
        
        public async Task HandleContextAsync(ListenerContext listenerContext, ILog log, CancellationToken token)
        {
            IRoutedHandler handler;
            HttpServerResponse response;
            var matchResult = routeTable.TryMatch(listenerContext.Request.Request.HttpMethod, listenerContext.Request.Request.Url.AbsolutePath, out handler);
            switch (matchResult)
            {
                case RouteMatchResult.Matched:
                    response = await handler.HandleRequestAsync(listenerContext.Request, log, token).ConfigureAwait(false);
                    break;
                case RouteMatchResult.UnrecognizedUri:
                    log.Warn($"Request url does not match any template: '{listenerContext.Request.Request.Url.AbsoluteUri}'");
                    response = new HttpServerResponse(HttpStatusCode.BadRequest);
                    break;
                case RouteMatchResult.UnrecognizedMethod:
                    log.Error(string.Format("Request method ('{0}') is not supported for url '{1}'.", 
                        listenerContext.Request.Request.HttpMethod, 
                        listenerContext.Request.Request.Url.AbsoluteUri));
                    response = new HttpServerResponse(HttpStatusCode.MethodNotAllowed);
                    break;
                default:
                    throw new Exception($"Unknown route match result: '{matchResult}'");
            }
            await listenerContext.Response.RespondAsync(response).ConfigureAwait(false);
        }
    }
}