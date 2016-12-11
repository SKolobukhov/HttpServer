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
            queue = new Queue<HttpListenerContext>(capacity);
            tokenSource = null;
            running = false;
            this.log = log;
        }

        public void Enqueue(HttpListenerContext task)
        {
            lock (queue)
            {
                if (!IsRunnig) return;
                queue.Enqueue(task);
                Monitor.Pulse(queue);
                log.Debug("Enqueue");
            }
        }

        public HttpListenerContext Dequeue()
        {
            lock (queue)
            {
                log.Debug("Wait");
                while (queue.Count == 0 && IsRunnig)
                {
                    Monitor.Wait(queue);
                }
                log.Debug("Dequeue");
                return IsRunnig ? queue.Dequeue() : null;
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
            tokenSource?.Cancel();
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
                log.Debug("ContextQueue is stopped");
            }
        }

        public void Dispose()
        {
            StopQueue();   
        }
    }
}