using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task2.interfaces
{

    public interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
        void Run();
    }
}