using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttpServer.Common;
using HttpServer.Server;
using log4net;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new ConsoleLog();
            var t = new RouteTableBuilder()
                .MapHandler(HttpMethod.Get, "hello", new RequestHandler())
                .MapHandler(HttpMethod.Get, "hello/123", new RequestHandler());
            var handler = new RoutingHandler(t.Build());
            var server = new HttpServer.Server.HttpServer(2, handler, log);
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
            log.Debug(request.Request.RemoteEndPoint + ":" + request.Request.RawUrl + ":" + request.Request.QueryString["log"]);
            await Task.Delay(100, token).ConfigureAwait(false);
            return new HttpServerResponse(HttpStatusCode.OK);
        }
    }
}
