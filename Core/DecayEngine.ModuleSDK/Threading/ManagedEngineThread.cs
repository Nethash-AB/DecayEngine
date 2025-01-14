using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Coroutines.Results;

namespace DecayEngine.ModuleSDK.Threading
{
    public class ManagedEngineThread : IEngineThread
    {
        private readonly SingleThreadTaskScheduler _taskScheduler;
        private readonly CancellationTokenSource _taskCts;
        private readonly ConcurrentQueue<Task> _taskQueue;
        private readonly ConcurrentQueue<ICoroutine> _coroutineQueue;
        private readonly Stopwatch _threadTimer;
        private readonly Stopwatch _updateTimer;
        private readonly float _msBetweenTicks;

        protected Thread Thread;
        protected bool Running;

        public event Action<double> OnUpdate;
        public bool Alive { get; private set; }
        public int ThreadId => Alive ? Thread.ManagedThreadId : -1;

        public ManagedEngineThread(string name, float ticksPerSecond, ThreadPriority priority = ThreadPriority.Highest, bool background = false)
            : this(ticksPerSecond)
        {
            Thread = new Thread(Loop)
            {
                Name = name,
                IsBackground = background,
                Priority = priority,
            };
            Thread.SetApartmentState(ApartmentState.MTA);
        }

        protected ManagedEngineThread(float ticksPerSecond)
        {
            _msBetweenTicks = 1 / ticksPerSecond * 1000;
            _threadTimer = new Stopwatch();
            _updateTimer = new Stopwatch();
            _taskQueue = new ConcurrentQueue<Task>();
            _coroutineQueue = new ConcurrentQueue<ICoroutine>();

            _taskScheduler = new SingleThreadTaskScheduler();
            _taskCts = new CancellationTokenSource();
        }

        public virtual void Run()
        {
            if (Running) return;

            Running = true;
            Thread.Start();
        }

        public virtual void Stop()
        {
            if (!Running) return;

            Running = false;
            Thread.Join();
        }

        public Task ExecuteOnThreadAsync(Action action)
        {
            Task task = new Task(action, _taskCts.Token);
            ExecuteOnThread(task);
            return task;
        }

        public Task<T> ExecuteOnThreadAsync<T>(Func<T> func)
        {
            Task<T> task = new Task<T>(func, _taskCts.Token);
            ExecuteOnThread(task);
            return task;
        }

        public void ExecuteOnThread(Action action)
        {
            Task task = new Task(action, _taskCts.Token);
            ExecuteOnThread(task);
            task.Wait();
        }

        public T ExecuteOnThread<T>(Func<T> func)
        {
            Task<T> task = new Task<T>(func, _taskCts.Token);
            ExecuteOnThread(task);
            return task.Result;
        }

        public ICoroutine CreateCoroutine(CoroutineDelegate body)
        {
            ManagedCoroutine coroutine = new ManagedCoroutine(body);
            _coroutineQueue.Enqueue(coroutine);
            return coroutine;
        }

        public ICoroutine<TState> CreateCoroutine<TState>(CoroutineDelegate<TState> body, ByReference<TState> state)
        {
            ManagedCoroutine<TState> coroutine = new ManagedCoroutine<TState>(body, state);
            _coroutineQueue.Enqueue(coroutine);
            return coroutine;
        }

        private void ExecuteOnThread(Task task)
        {
            if (Thread.CurrentThread == Thread)
            {
                task.Start(_taskScheduler);
            }
            else
            {
                _taskQueue.Enqueue(task);
            }
        }

        protected void Loop()
        {
            Alive = true;

            _threadTimer.Start();
            _updateTimer.Start();
            while (Running)
            {
                RunStackedTasks();
                RunCoroutines();

                double deltaTime = _updateTimer.Elapsed.TotalSeconds;
                _updateTimer.Restart();

                OnUpdate?.Invoke(deltaTime);

                while (_threadTimer.Elapsed.TotalMilliseconds < _msBetweenTicks)
                {
                    Thread.Sleep(1);
                }

                _threadTimer.Restart();
            }
            _threadTimer.Stop();
            _updateTimer.Stop();

            ClearThread();

            Alive = false;
        }

        private void ClearThread()
        {
            _taskCts.Cancel();
            foreach (ICoroutine coroutine in _coroutineQueue.ToArray())
            {
                coroutine.Cancel();
            }
        }

        private void RunStackedTasks()
        {
            while (!_taskQueue.IsEmpty)
            {
                if (!_taskQueue.TryDequeue(out Task task)) continue;
                if (task.Status != TaskStatus.Created) continue;

                try
                {
                    task.ContinueWith(prevTask =>
                    {
                        if (task.Exception != null)
                        {
                            throw task.Exception;
                        }
                    });

                    task.Start(_taskScheduler);
                }
                catch (Exception e)
                {
                    if (e is InvalidOperationException)
                    {
                        continue;
                    }

                    throw;
                }
            }
        }

        private void RunCoroutines()
        {
            List<ICoroutine> pendingCoroutines = new List<ICoroutine>();

            while (!_coroutineQueue.IsEmpty)
            {
                if (!_coroutineQueue.TryDequeue(out ICoroutine coroutine)) continue;

                IYieldResult lastResult = coroutine.LastResult;
                if (lastResult != null)
                {
                    if (!lastResult.Ready || lastResult is CoroutineCreatedResult)
                    {
                        pendingCoroutines.Add(coroutine);
                        continue;
                    }

                    if (lastResult is CoroutineEndResult || lastResult is CoroutineCancelledResult || lastResult is CoroutineFaultResult) continue;
                }

                if (coroutine.Step())
                {
                    pendingCoroutines.Add(coroutine);
                }
            }

            pendingCoroutines.ForEach(_coroutineQueue.Enqueue);
        }
    }
}