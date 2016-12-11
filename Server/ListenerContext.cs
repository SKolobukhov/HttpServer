using System.Net;
using log4net;

namespace HttpServer.Server
{
    public class ListenerContext
    {
        public readonly HttpRequestWrapper Request;
        public readonly HttpResponseWrapper Response;

        public ListenerContext(HttpListenerContext context, ILog log)
        {
            Request = new HttpRequestWrapper(context.User, context.Request);
            Response = new HttpResponseWrapper(context, log);
        }
    }
}