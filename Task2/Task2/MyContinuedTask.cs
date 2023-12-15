using Task2.interfaces;

namespace Task2
{
    public class MyContinuedTask<TNewResult, TResult> : MyTask<TNewResult>
    {
        private readonly Func<TResult, TNewResult> continuedFunc;
        private readonly MyTask<TResult> parent;
        public MyContinuedTask(MyTask<TResult> parent, IMyThreadPool myPool, Func<TResult, TNewResult> func)
        : base(myPool)
        {
            continuedFunc = func;
            this.parent = parent;
        }
        public void ParentTaskReady()
        {
            try
            {
                TResult result = parent.Result;
                func = () => continuedFunc(result);
                myPool.Enqueue(this);
            }
            catch (Exception ex)
            {
                Monitor.Enter(monitor);
                if (ex is AggregateException aggregateException)
                {
                    error = aggregateException;
                }
                else
                {
                    error = new AggregateException(ex);
                }
                isCompleted = true;
                Monitor.PulseAll(monitor);
                Monitor.Exit(monitor);

                RunContinuedTasks();
            }
        }
    }
}