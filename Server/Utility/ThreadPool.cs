using System;
using System.Threading;
using log4net;

namespace HttpServer.Server
{
    internal class ThreadPool : IDisposable
    {
        private readonly ILog log;
        private readonly Thread[] threads;
        private readonly Action<ILog, CancellationToken> action;

        private int running;
        private CancellationTokenSource tokenSource;

        public bool IsRunning => running == 1;


        public ThreadPool(int multiplier, Action<ILog, CancellationToken> action, ILog log)
        {
            this.log = log;
            this.action = action;
            threads = new Thread[multiplier];
        }

        public void Start(CancellationToken? token = null)
        {
            if (Interlocked.CompareExchange(ref running, 1, 0) != 0)
            {
                throw new ApplicationException("ThreadPool is already running.");
            }
            tokenSource = new CancellationTokenSource();
            token?.Register(() => tokenSource.Cancel());
            tokenSource.Token.Register(StopThreadPool);
            for (var index = 0; index < threads.Length; index++)
            {
                StartThread(index);
            }
        }
        
        public void Dispose()
        {
            if (IsRunning)
            {
                tokenSource.Cancel();
            }
        }
        
        private void StopThreadPool()
        {
            if (Interlocked.CompareExchange(ref running, 0, 1) != 1)
            {
                throw new ApplicationException("ThreadPool is not running.");
            }
            tokenSource = null;
            foreach (var workThread in threads)
            {
                workThread.Abort();
                workThread.Join();
            }
        }

        private void StartThread(int index)
        {
            var thread = new Thread(WorkerRoutine)
            {
                IsBackground = true,
                Name = "HttpServer-Worker" + index
            };
            thread.Start(index);
            threads[index] = thread;
        }

        private void WorkerRoutine(object indexObject)
        {
            var workerLog = log.WithPrefix("Worker" + indexObject);
            workerLog.Info("Worker are running");
            try
            {
                action(workerLog, tokenSource.Token);
            }
            catch (ThreadAbortException) { }
            catch (Exception exception)
            {
                workerLog.Error($"Fail with message: {exception.Message}", exception);
                workerLog.Info("Worker is stopped");
                StartThread((int)indexObject);
            }
            workerLog.Info("Worker is stopped");
        }
    }
}