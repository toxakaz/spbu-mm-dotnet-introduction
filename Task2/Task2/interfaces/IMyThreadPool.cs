using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task2.interfaces
{
    public interface IMyThreadPool : IDisposable
    {
        void Enqueue<TResult>(IMyTask<TResult> task);
    }
}