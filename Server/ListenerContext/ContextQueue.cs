using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using log4net;

namespace HttpServer.Server
{
    internal class ContextQueue: IDisposable
    {
        private readonly Queue<HttpListenerContext> queue;
        private CancellationTokenSource tokenSource;
        private volatile bool running;
        private readonly ILog log;

        public bool IsRunnig => running;

        public ContextQueue(ILog log, int capacity = 20000)
        {
            this.log = log;
            queue = new Queue<HttpListenerContext>(capacity);
        }

        public void Enqueue(HttpListenerContext task)
        {
            lock (queue)
            {
                if (!IsRunnig) return;
                queue.Enqueue(task);
                Monitor.Pulse(queue);
            }
        }

        public HttpListenerContext Dequeue(CancellationToken token)
        {
            token.Register(() =>
            {
                lock (queue)
                {
                    Monitor.PulseAll(queue);
                }
            });
            lock (queue)
            {
                while (queue.Count == 0 && IsRunnig && !token.IsCancellationRequested)
                {
                    Monitor.Wait(queue);
                }
                return IsRunnig && !token.IsCancellationRequested ? queue.Dequeue() : null;
            }
        }

        public void Start(CancellationToken? token = null)
        {
            lock (queue)
            {
                if (IsRunnig) return;
                tokenSource = new CancellationTokenSource();
                token?.Register(() => tokenSource.Cancel());
                tokenSource.Token.Register(StopQueue);
                running = true;
                log.Info("ContextQueue is running");
            }
        }

        [Obsolete]
        public void Stop()
        {
            if (IsRunnig)
            {
                tokenSource?.Cancel();
            }
        }

        private void StopQueue()
        {
            lock (queue)
            {
                if (!IsRunnig) return;
                queue.Clear();
                running = false;
                tokenSource = null;
                Monitor.PulseAll(queue);
                log.Info("ContextQueue is stopped");
            }
        }

        public void Dispose()
        {
            if (IsRunnig)
            {
                tokenSource?.Cancel();
            } 
        }
    }
}