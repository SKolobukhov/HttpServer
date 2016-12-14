using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace HttpServer.Server
{
    public interface IRequestHandler
    {
        Task HandleContextAsync(ListenerContext listenerContext, ILog log, CancellationToken token);
    }
}