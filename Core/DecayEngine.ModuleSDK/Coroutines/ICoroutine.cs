using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DecayEngine.DecPakLib;

namespace DecayEngine.ModuleSDK.Coroutines
{
    public interface ICoroutine
    {
        IYieldResult LastResult { get; }

        Task<bool> Task { get; }
        bool IsRunning { get; }
        bool IsCanceled { get; }
        bool IsCompleted { get; }
        bool IsFaulted { get; }
        Exception Exception { get; }

        event Action<ICoroutine> OnComplete;
        event Action<ICoroutine> OnCancel;
        event Action<ICoroutine> OnFault;

        bool Step();
        void Run();
        void Cancel();
        Task<bool> Wait();
        TaskAwaiter<bool> GetAwaiter();
    }

    public interface ICoroutine<TState> : ICoroutine
    {
        ByReference<TState> State { get; }

        new event Action<ICoroutine<TState>> OnComplete;
        new event Action<ICoroutine<TState>> OnCancel;
        new event Action<ICoroutine<TState>> OnFault;
    }
}