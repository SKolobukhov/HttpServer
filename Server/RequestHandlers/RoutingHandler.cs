using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Common;
using log4net;

namespace HttpServer.Server
{
    /*public class RoutingHandler : IRequestHandler
    {
        private readonly RouteTable routeTable;
        private readonly ILog log;

        public RoutingHandler(RouteTable routeTable, ILog log)
        {
            this.routeTable = routeTable;
            this.log = log;
        }

        #region Logging
        private void LogUnrecognizedUrl(Uri url)
        {
            log.Error("Request url does not match any template: '{0}'.", url.GetLeftPart(UriPartial.Path));
        }
        private void LogUnrecognizedMethod(Uri url, HttpMethod method)
        {
            log.Error("Request method ('{0}') is not supported for url '{1}'.", method, url.GetLeftPart(UriPartial.Path));
        }
        #endregion

        public void HandleContext(ListenerContext listenerContext, ILog log, CancellationToken token)
        {
            IRoutedHandler handler;
            HttpServerResponse response;
            var matchResult = routeTable.TryMatch(listenerContext.Request.Request.HttpMethod, listenerContext.Request.Request.Url.AbsolutePath, out handler);
            switch (matchResult)
            {
                case RouteMatchResult.Matched:
                    response = handler.HandleRequestAsync(listenerContext.Request, token).Result;
                    break;
                case RouteMatchResult.UnrecognizedUri:
                    LogUnrecognizedUrl(request.Url);
                    response = new HttpServerResponse(HttpStatusCode.BadRequest);
                    break;
                case RouteMatchResult.UnrecognizedMethod:
                    LogUnrecognizedMethod(request.Url, request.Method);
                    response = new HttpServerResponse(HttpResponseCode.MethodNotAllowed);
                default:
                    throw new Exception(String.Format("Unknown route match result: '{0}'", matchResult));
            }
        }
    }*/
}