using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace HttpServer.Server
{
    public interface IRoutedHandler
    {
        Task<HttpServerResponse> HandleRequestAsync(HttpRequestWrapper request, ILog log, CancellationToken token);
    }
}