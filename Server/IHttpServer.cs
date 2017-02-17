using System;
using System.Threading;

namespace HttpServer.Server
{
    public interface IHttpServer: IDisposable
    {
        bool IsRunning { get; }


        void Start(int port = 80, CancellationToken? token = null);

        [Obsolete]
        void Stop();
    }
}