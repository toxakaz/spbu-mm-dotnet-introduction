using Task2.interfaces;

namespace Task2
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        protected volatile bool isCompleted = false;
        private volatile bool isInProcess = false;
        protected AggregateException? error = null;
        private TResult? result = default;
        protected Func<TResult>? func;
        protected readonly IMyThreadPool myPool;
        private readonly Queue<Action> continuedTasksTriggers = new();
        protected readonly object monitor = new();

        public MyTask(Func<TResult> func, IMyThreadPool myPool)
        {
            this.func = func;
            this.myPool = myPool;
        }
        protected MyTask(IMyThreadPool myPool)
        {
            func = null;
            this.myPool = myPool;
        }

        public bool IsCompleted { get { return isCompleted; } }

        public TResult Result
        {
            get
            {
                Monitor.Enter(monitor);
                if (!isCompleted)
                {
                    Monitor.Wait(monitor);
                }
                Monitor.Exit(monitor);

                if (error is not null)
                {
                    throw error;
                }
                return result;
            }
        }

        public void Run()
        {
            Monitor.Enter(monitor);
            if ((func is null) || isCompleted || isInProcess)
            {
                Monitor.Exit(monitor);
                return;
            }
            isInProcess = true;
            Monitor.Exit(monitor);

            try
            {
                result = func();
            }
            catch (Exception e)
            {
                error = new AggregateException(e);
            }

            Monitor.Enter(monitor);
            isCompleted = true;
            isInProcess = false;
            Monitor.PulseAll(monitor);
            Monitor.Exit(monitor);

            RunContinuedTasks();
        }

        protected void RunContinuedTasks()
        {
            while (continuedTasksTriggers.Count > 0)
            {
                continuedTasksTriggers.Dequeue()();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
        {
            var newTask = new MyContinuedTask<TNewResult, TResult>(this, myPool, func);
            Monitor.Enter(monitor);
            if (isCompleted)
            {
                newTask.ParentTaskReady();
            }
            else
            {
                continuedTasksTriggers.Enqueue(newTask.ParentTaskReady);
            }
            Monitor.Exit(monitor);
            return newTask;
        }
    }
}