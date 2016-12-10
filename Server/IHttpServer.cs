using System;
using System.Threading;
using HttpServer.Common;

namespace HttpServer.Server
{
    public interface IHttpServer: IDisposable
    {
        bool IsRunning { get; }


        void Start(ushort port = 80, HttpScheme scheme = HttpScheme.Http, CancellationToken? token = null);

        [Obsolete]
        void Stop();
    }
}