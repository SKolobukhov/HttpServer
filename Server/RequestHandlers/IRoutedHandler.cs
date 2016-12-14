using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.Server
{
    public interface IRoutedHandler
    {
        Task<HttpServerResponse> HandleRequestAsync(HttpRequestWrapper request, CancellationToken token);
    }
}