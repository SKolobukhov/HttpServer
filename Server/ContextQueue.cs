using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HttpServer.Server
{
    internal class ContextQueue: IDisposable
    {
        private readonly Queue<HttpListenerContext> queue;
        private CancellationTokenSource tokenSource;
        private volatile bool running;
        
        public bool IsRunnig => running;


        public ContextQueue(int capacity = 20000)
        {
            queue = new Queue<HttpListenerContext>(capacity);
            tokenSource = null;
            running = false;
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

        public HttpListenerContext Dequeue()
        {
            lock (queue)
            {
                if (queue.Count == 0 || IsRunnig)
                {
                    Monitor.Wait(queue);
                }
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
                tokenSource = null;
                Monitor.PulseAll(queue);
                queue.Clear();
                running = false;
            }
        }

        public void Dispose()
        {
            StopQueue();   
        }
    }
}