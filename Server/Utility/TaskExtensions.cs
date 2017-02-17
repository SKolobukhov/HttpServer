using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.Server
{
    public static class TaskExtensions
    {
        public static async Task WithCancellation(this Task task, CancellationToken token)
        {
            var tsc = new TaskCompletionSource<bool>();
            using (token.Register(() => tsc.SetResult(true)))
            {
                if (task == await Task.WhenAny(task, tsc.Task).ConfigureAwait(false))
                { 
                    tsc.SetCanceled();
                }
            }
        }

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken token)
        {
            var tsc = new TaskCompletionSource<T>();
            using (token.Register(() => tsc.SetResult(default(T))))
            {
                var finishedTask = await Task.WhenAny(task, tsc.Task).ConfigureAwait(false);
                if (finishedTask == task)
                {
                    tsc.SetCanceled();
                }
                return await finishedTask.ConfigureAwait(false);
            }
        }
    }
}
