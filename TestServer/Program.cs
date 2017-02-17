using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Server;
using log4net;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new ConsoleLog();
            var routeTableBuilder = new RouteTableBuilder()
                .MapHandler(HttpMethod.Get, "hello", new RequestHandler())
                .MapHandler(HttpMethod.Get, "hello/123", new RequestHandler());
            var handler = new RoutingHandler(routeTableBuilder.Build());
            var server = new AsyncHttpServer(handler, log);
            server.Start(789);
            Console.ReadKey();
            server.Stop();
            Console.ReadKey();
        }
    }

    public class RequestHandler : IRoutedHandler
    {

        public async Task<HttpServerResponse> HandleRequestAsync(HttpRequestWrapper request, ILog log, CancellationToken token)
        {
            var message = "RawUrl: " + request.Request.RawUrl;
            log.Debug(message);
            return new HttpServerResponse(HttpStatusCode.OK, new StringContent(message));
        }
    }
}
