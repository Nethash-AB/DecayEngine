using System;
using System.Threading.Tasks;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Coroutines;

namespace DecayEngine.ModuleSDK.Threading
{
    public interface IEngineThread
    {
        event Action<double> OnUpdate;
        bool Alive { get; }
        int ThreadId { get; }

        void Run();
        void Stop();

        Task ExecuteOnThreadAsync(Action action);
        Task<T> ExecuteOnThreadAsync<T>(Func<T> func);
        void ExecuteOnThread(Action action);
        T ExecuteOnThread<T>(Func<T> func);
        ICoroutine CreateCoroutine(CoroutineDelegate body);
        ICoroutine<TState> CreateCoroutine<TState>(CoroutineDelegate<TState> body, ByReference<TState> state);
    }
}