using System;
using System.Threading;

namespace HttpServer.Server
{
    internal class FiexdBuffer<T> : IDisposable
    {
        private readonly T[] buffer;
        private readonly Semaphore emptySemaphore;
        private readonly Semaphore fullSemaphore;

        private volatile int head;
        private volatile int tail;
        private volatile int count;

        public bool IsEmpty
        {
            get
            {
                lock (buffer)
                {
                    return count == 0;
                }
            }
        }

        public bool IsFull
        {
            get
            {
                lock (buffer)
                {
                    return count == buffer.Length;
                }
            }
        }


        public FiexdBuffer(int bufferSize = 1000)
        {
            head = 0;
            tail = 0;
            count = 0;
            buffer = new T[bufferSize];
            emptySemaphore = new Semaphore(0, bufferSize);
            fullSemaphore = new Semaphore(bufferSize, bufferSize);
        }

        public void Enqueue(T data, CancellationToken token)
        {
            WaitHandle.WaitAny(new[] { fullSemaphore, token.WaitHandle });
            if (token.IsCancellationRequested)
            {
                return;
            }
            
            lock (buffer)
            {
                buffer[tail++] = data;
                if (tail == buffer.Length)
                {
                    tail = 0;
                }
                count++;
            }
            emptySemaphore.Release();
        }

        public T Dequeue(CancellationToken token)
        {
            var data = default(T);
            WaitHandle.WaitAny(new[] { emptySemaphore, token.WaitHandle });
            if (token.IsCancellationRequested)
            {
                return data;
            }
            
            lock (buffer)
            {
                data = buffer[head++];
                if (head == buffer.Length)
                {
                    head = 0;
                }
                count--;
            }
            fullSemaphore.Release();
            return data;
        }

        public void Dispose()
        {
            emptySemaphore.Dispose();
            fullSemaphore.Dispose();
        }
    }
}