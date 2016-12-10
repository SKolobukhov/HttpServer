using System;
using System.Linq;
using System.Threading;
using log4net;

namespace HttpServer.Server
{
    internal class ThreadPool : IDisposable
    {
        private readonly ILog log;
        private readonly Thread[] workerThreads;
        private readonly Action<CancellationToken> workerAction;

        private volatile bool running;
        private CancellationTokenSource tokenSource;
        public bool IsRunning => running;

        public ThreadPool(int multiplier, Action<CancellationToken> workerAction, ILog log)
        {
            this.log = log;
            this.workerAction = workerAction;
            workerThreads = new Thread[multiplier];
            tokenSource = null;
            running = false;
        }

        public void Start(CancellationToken? token = null)
        {
            if (IsRunning) return;
            tokenSource = new CancellationTokenSource();
            token?.Register(() => tokenSource.Cancel());
            tokenSource.Token.Register(StopThreadPool);
            StartThreadPool();
        }

        [Obsolete]
        public void Stop()
        {
            tokenSource?.Cancel();
        }

        private void StartThreadPool()
        {
            if (IsRunning) return;
            for (var threadIndex = 0; threadIndex < workerThreads.Length; threadIndex++)
            {
                StartThread(threadIndex);
            }
            running = true;
            log.Info("Workers are running");
        }

        private void StopThreadPool()
        {
            if (!IsRunning) return;
            tokenSource = null;
            foreach (var workThread in workerThreads.Where(workerThread => workerThread.ThreadState == ThreadState.Running))
            {
                workThread.Abort();
                workThread.Join();
            }
            running = false;
            log.Info("Workers are stopped");
        }

        public void Dispose()
        {
            if (IsRunning)
            {
                tokenSource.Cancel();
            }
        }

        private void StartThread(int threadIndex)
        {
            var thread = new Thread(WorkerRoutine)
            {
                IsBackground = true,
                Name = "HttpServer-Worker-" + Guid.NewGuid()
            };
            thread.Start(threadIndex);
            workerThreads[threadIndex] = thread;
        }

        private void WorkerRoutine(object indexObject)
        {
            var index = (int)indexObject;
            try
            {
                workerAction(tokenSource.Token);
            }
            catch (ThreadAbortException) { }
            catch (Exception exception)
            {
                log.Error($"[Worker_{index}] An exception occured in requests pipeline: {exception.Message}", exception);
                StartThread(index);
            }
        }
    }
}