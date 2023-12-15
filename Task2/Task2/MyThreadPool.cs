using Task2.interfaces;

namespace Task2
{
    public class MyThreadPool : IMyThreadPool
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly CancellationToken cancellationToken;
        private readonly Thread[] threads;
        private readonly Queue<Action> taskQueue = new();
        private volatile bool isRunning = true;
        private readonly object monitor = new();

        public MyThreadPool(int threadCount)
        {
            cancellationToken = cancellationTokenSource.Token;

            threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() => { ThreadRoutine(cancellationToken); });
                threads[i].Start();
            }
        }

        private void ThreadRoutine(CancellationToken token)
        {
            Monitor.Enter(monitor);
            while (!token.IsCancellationRequested || taskQueue.Count > 0)
            {
                if (taskQueue.Count == 0)
                {
                    Monitor.Wait(monitor);
                    continue;
                }

                var task = taskQueue.Dequeue();
                Monitor.Pulse(monitor);
                Monitor.Exit(monitor);

                task();

                Monitor.Enter(monitor);
            }
            Monitor.PulseAll(monitor);
            Monitor.Exit(monitor);
        }

        public void Enqueue<TResult>(IMyTask<TResult> task)
        {
            Monitor.Enter(monitor);
            if (isRunning)
            {
                taskQueue.Enqueue(task.Run);
                Monitor.Pulse(monitor);
                Monitor.Exit(monitor);
            }
            else
            {
                Monitor.Exit(monitor);
                throw new Exception("Thread pool was disposed");
            }
        }

        public void Dispose()
        {
            Monitor.Enter(monitor);
            if (isRunning)
            {
                isRunning = false;
                cancellationTokenSource.Cancel();
                Monitor.PulseAll(monitor);
            }
            Monitor.Exit(monitor);

            GC.SuppressFinalize(this);
        }
    }
}