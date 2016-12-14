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
        private readonly Action<CancellationToken, ILog> workerAction;

        private volatile bool running;
        private CancellationTokenSource tokenSource;
        public bool IsRunning => running;

        public ThreadPool(int multiplier, Action<CancellationToken, ILog> workerAction, ILog log)
        {
            this.log = log;
            this.workerAction = workerAction;
            workerThreads = new Thread[multiplier];
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

        public void Dispose()
        {
            if (IsRunning)
            {
                tokenSource.Cancel();
            }
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

        private void StartThread(int threadIndex)
        {
            var thread = new Thread(WorkerRoutine)
            {
                IsBackground = true,
                Name = "HttpServer-Worker-" + threadIndex
            };
            thread.Start(threadIndex);
            workerThreads[threadIndex] = thread;
        }

        private void WorkerRoutine(object indexObject)
        {
            var threadIndex = (int) indexObject;
            var workerLog = this.log.WithPrefix("Worker-" + threadIndex);
            try
            {
                workerAction(tokenSource.Token, workerLog);
            }
            catch (ThreadAbortException) { }
            catch (Exception exception)
            {
                workerLog.Error($"An exception occured in requests pipeline: {exception.Message}", exception);
                StartThread(threadIndex);
            }
        }
    }
}