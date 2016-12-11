using System.Threading;
using log4net;

namespace HttpServer.Server
{
    public interface IRequestHandler
    {
        void HandleContext(ListenerContext listenerContext, ILog log, CancellationToken token);
    }
}