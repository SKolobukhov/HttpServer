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
            var server = new HttpServer.Server.HttpServer(2, new RequestHandler(), log);
            server.Start(789);
            Console.ReadKey();
            server.Stop();
            Console.ReadKey();
        }
    }

    public class RequestHandler : IRequestHandler
    {
        
        public async Task HandleContextAsync(ListenerContext listenerContext, ILog log, CancellationToken token)
        {
            log.Debug(listenerContext.Request.Request.RemoteEndPoint + ":" + listenerContext.Request.Request.RawUrl);
            await Task.Delay(100).ConfigureAwait(false);
            await listenerContext.Response.RespondAsync(new HttpServerResponse(HttpStatusCode.OK)).ConfigureAwait(false);
        }
    }
}
