using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Coroutines.Results;

namespace DecayEngine.ModuleSDK.Coroutines
{
    internal class ManagedCoroutine : ICoroutine
    {
        private readonly IEnumerator<IYieldResult> _enumerator;
        private readonly TaskCompletionSource<bool> _tcs;

        public IYieldResult LastResult { get; private set; }

        public Task<bool> Task => _tcs.Task;
        public bool IsRunning => !(LastResult is CoroutineCreatedResult) && !(LastResult is CoroutineCancelledResult) && !(LastResult is CoroutineEndResult);
        public bool IsCanceled => LastResult is CoroutineCancelledResult;
        public bool IsCompleted => LastResult is CoroutineEndResult;
        public bool IsFaulted => LastResult is CoroutineFaultResult;
        public Exception Exception => (LastResult as CoroutineFaultResult)?.Exception;

        public event Action<ICoroutine> OnComplete;
        public event Action<ICoroutine> OnCancel;
        public event Action<ICoroutine> OnFault;

        internal ManagedCoroutine(CoroutineDelegate body)
        {
            _enumerator = body() ?? throw new ArgumentNullException(nameof(body), "The body of a coroutine cannot be null.");
            _tcs = new TaskCompletionSource<bool>();
            LastResult = new CoroutineCreatedResult();
        }

        public bool Step()
        {
            try
            {
                if (!_enumerator.MoveNext())
                {
                    LastResult = new CoroutineEndResult();
                    _tcs.SetResult(true);
                    OnComplete?.Invoke(this);
                    return false;
                }
            }
            catch (Exception e)
            {
                LastResult = new CoroutineFaultResult(e);
                _tcs.SetResult(false);
                OnFault?.Invoke(this);
            }

            LastResult = _enumerator.Current ?? new WaitForNextTick();
            return true;
        }

        public void Run()
        {
            if (LastResult is CoroutineCreatedResult)
            {
                LastResult = new WaitForNextTick();
            }
        }

        // ToDo: Verify that cancelling while thread is stepping does not cause unintended behaviour.
        public void Cancel()
        {
            LastResult = new CoroutineCancelledResult();
            _tcs.SetResult(false);
            OnCancel?.Invoke(this);
        }

        public async Task<bool> Wait()
        {
            return await this;
        }

        public TaskAwaiter<bool> GetAwaiter()
        {
            return Task.GetAwaiter();
        }
    }

    public class ManagedCoroutine<TState> : ICoroutine<TState>
    {
        private readonly IEnumerator<IYieldResult> _enumerator;
        private readonly TaskCompletionSource<bool> _tcs;

        public IYieldResult LastResult { get; private set; }

        public Task<bool> Task => _tcs.Task;
        public bool IsRunning => !(LastResult is CoroutineCreatedResult) && !(LastResult is CoroutineCancelledResult) && !(LastResult is CoroutineEndResult);
        public bool IsCanceled => LastResult is CoroutineCancelledResult;
        public bool IsCompleted => LastResult is CoroutineEndResult;
        public bool IsFaulted => LastResult is CoroutineFaultResult;
        public Exception Exception => (LastResult as CoroutineFaultResult)?.Exception;

        public ByReference<TState> State { get; private set; }

        public event Action<ICoroutine<TState>> OnComplete;
        public event Action<ICoroutine<TState>> OnCancel;
        public event Action<ICoroutine<TState>> OnFault;

        event Action<ICoroutine> ICoroutine.OnComplete
        {
            add => OnComplete += value;
            remove => OnComplete -= value;
        }

        event Action<ICoroutine> ICoroutine.OnCancel
        {
            add => OnCancel += value;
            remove => OnCancel -= value;
        }

        event Action<ICoroutine> ICoroutine.OnFault
        {
            add => OnFault += value;
            remove => OnFault -= value;
        }

        internal ManagedCoroutine(CoroutineDelegate<TState> body, ByReference<TState> state)
        {
            _enumerator = body(state) ?? throw new ArgumentNullException(nameof(body), "The body of a coroutine cannot be null.");
            _tcs = new TaskCompletionSource<bool>();
            State = state;
            LastResult = new CoroutineCreatedResult();
        }

        public bool Step()
        {
            try
            {
                if (!_enumerator.MoveNext())
                {
                    LastResult = new CoroutineEndResult();
                    _tcs.SetResult(true);
                    OnComplete?.Invoke(this);
                    return false;
                }
            }
            catch (Exception e)
            {
                LastResult = new CoroutineFaultResult(e);
                _tcs.SetResult(false);
                OnFault?.Invoke(this);
            }

            LastResult = _enumerator.Current ?? new WaitForNextTick();
            return true;
        }

        public void Run()
        {
            if (LastResult is CoroutineCreatedResult)
            {
                LastResult = new WaitForNextTick();
            }
        }

        // ToDo: Verify that cancelling while thread is stepping does not cause unintended behaviour.
        public void Cancel()
        {
            LastResult = new CoroutineCancelledResult();
            _tcs.SetResult(false);
            OnCancel?.Invoke(this);
        }

        public async Task<bool> Wait()
        {
            return await this;
        }

        public TaskAwaiter<bool> GetAwaiter()
        {
            return Task.GetAwaiter();
        }
    }
}