using Task2;
using Task2.interfaces;

namespace Task2Tests;

public class Tests
{
    [Test]
    public void TestSingleTask()
    {
        IMyThreadPool pool = new MyThreadPool(1);

        IMyTask<int> task = new MyTask<int>(() => 42, pool);
        pool.Enqueue(task);
        pool.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(task.Result, Is.EqualTo(42));
            Assert.That(task.IsCompleted, Is.True);
        });
    }

    [Test]
    public void TestMultipleTasks()
    {
        IMyThreadPool pool = new MyThreadPool(1);

        int n = 10;
        IMyTask<int>[] tasks = new IMyTask<int>[n];
        for (int i = 0; i < n; i++)
        {
            var func = () =>
            {
                Thread.Sleep(100);
                return 42;
            };
            tasks[i] = new MyTask<int>(func, pool);
        }

        foreach (var task in tasks)
        {
            pool.Enqueue(task);
        }
        pool.Dispose();

        foreach (var task in tasks)
        {
            Assert.Multiple(() =>
            {
                Assert.That(task.Result, Is.EqualTo(42));
                Assert.That(task.IsCompleted, Is.True);
            });
        }
    }

    [Test]
    public void TestMultipleThreads()
    {
        IMyThreadPool pool = new MyThreadPool(100);

        int n = 10;
        IMyTask<int>[] tasks = new IMyTask<int>[n];
        for (int i = 0; i < n; i++)
        {
            var func = () =>
            {
                Thread.Sleep(100);
                return 42;
            };
            tasks[i] = new MyTask<int>(func, pool);
        }

        foreach (var task in tasks)
        {
            pool.Enqueue(task);
        }
        pool.Dispose();

        foreach (var task in tasks)
        {
            Assert.Multiple(() =>
            {
                Assert.That(task.Result, Is.EqualTo(42));
                Assert.That(task.IsCompleted, Is.True);
            });
        }
    }

    [Test]
    public void TestContinueWith()
    {
        IMyThreadPool pool = new MyThreadPool(4);

        int n = 10;
        IMyTask<int>[] tasks = new IMyTask<int>[n];
        int[] expected = new int[n];

        tasks[0] = new MyTask<int>(() => 1, pool);
        expected[0] = 1;

        for (int i = 1; i < n; i++)
        {
            tasks[i] = tasks[i - 1].ContinueWith((int x) => x * 2);
            expected[i] = expected[i - 1] * 2;
        }

        pool.Enqueue(tasks[0]);

        for (int i = 0; i < n; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(tasks[i].Result, Is.EqualTo(expected[i]));
                Assert.That(tasks[i].IsCompleted, Is.True);
            });
        }

        pool.Dispose();
    }

    [Test]
    public void TestContinueWithWrongUsage()
    {
        IMyThreadPool pool = new MyThreadPool(4);

        int n = 10;
        IMyTask<int>[] tasks = new IMyTask<int>[n];
        int[] expected = new int[n];

        tasks[0] = new MyTask<int>(() => 1, pool);
        expected[0] = 1;

        for (int i = 1; i < n; i++)
        {
            var func = (int x) =>
            {
                Thread.Sleep(100);
                return x * 2;
            };
            tasks[i] = tasks[i - 1].ContinueWith(func);
            expected[i] = expected[i - 1] * 2;
        }

        pool.Enqueue(tasks[0]);

        // current realization does not guarantee that ContinueWith task will be completed
        // because thread pool do not know about it before parent task ended
        pool.Dispose();

        int failedCount = 0;
        for (int i = 0; i < n; i++)
        {
            try
            {
                Assert.That(tasks[i].Result, Is.EqualTo(expected[i]));
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is null)
                {
                    Assert.Fail();
                }
                else
                {
                    Assert.That(ex.InnerException.Message, Is.EqualTo("Thread pool was disposed"));
                    failedCount += 1;
                }
            }
            Assert.That(tasks[i].IsCompleted, Is.True);
        }
        Assert.That(failedCount, Is.Not.EqualTo(0));
    }

    [Test]
    public void TestThreadCount()
    {
        int count = 4;
        IMyThreadPool pool = new MyThreadPool(count);

        int n = 50;
        IMyTask<int>[] tasks = new IMyTask<int>[n];

        for (int i = 0; i < n; i++)
        {
            Func<int> func = () =>
            {
                Thread.Sleep(100);
                return Environment.CurrentManagedThreadId;
            };
            tasks[i] = new MyTask<int>(func, pool);
        }

        foreach (var task in tasks)
        {
            pool.Enqueue(task);
        }

        HashSet<int> actualThreads = [];
        foreach (var task in tasks)
        {
            actualThreads.Add(task.Result);
        }

        pool.Dispose();

        Assert.That(actualThreads, Has.Count.EqualTo(count));
    }

    [Test]
    public void TestException()
    {
        string message = "Oh no, an exception occurred";

        IMyThreadPool pool = new MyThreadPool(1);
        IMyTask<string> task = new MyTask<string>(() => throw new Exception(message), pool);

        pool.Enqueue(task);
        pool.Dispose();

        try
        {
            var result = task.Result;
            Assert.Fail();
        }
        catch (AggregateException ex)
        {
            Assert.That(ex.InnerException is not null, Is.True);
            if (ex.InnerException is not null)
            {
                Assert.That(ex.InnerException.Message, Is.EqualTo(message));
            }
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }
}