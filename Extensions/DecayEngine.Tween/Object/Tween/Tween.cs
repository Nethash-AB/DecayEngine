using System;
using System.Collections.Generic;
using System.Diagnostics;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Coroutines.Results;
using DecayEngine.Tween.Object.Tween.Data;

namespace DecayEngine.Tween.Object.Tween
{
    public class Tween
    {
        private readonly TweenEase _tweenEase;
        private readonly List<TweenData> _tweenData;
        private readonly double _targetTime;
        private ICoroutine _coroutine;

        public event Action OnEnd;

        public Tween(TweenEase ease, List<TweenData> tweenData, double targetTime)
        {
            _tweenEase = ease;
            _tweenData = tweenData;
            _targetTime = targetTime;
        }

        public void Run(CoroutineContext context)
        {
            if (_coroutine != null) return;

            _coroutine = GameEngine.CreateCoroutine(UpdateTween, context);
            _coroutine.OnComplete += EndCallback;
            _coroutine.OnCancel += EndCallback;
            _coroutine.OnFault += EndCallback;
            _coroutine.Run();
        }

        private IEnumerator<IYieldResult> UpdateTween()
        {
            _tweenData.ForEach(tweenData => tweenData.Reset());
            Stopwatch stopwatch = Stopwatch.StartNew();
            double functionTime;

            do
            {
                functionTime = stopwatch.Elapsed.TotalMilliseconds / _targetTime;

                float functionResult = _tweenEase.Calculate((float) functionTime);

                foreach (TweenData tweenData in _tweenData)
                {
                    tweenData.SetValue(functionResult);
                }

                yield return new WaitForNextTick();
            } while (functionTime < 1);

            stopwatch.Stop();
        }

        private void EndCallback(ICoroutine coroutine)
        {
            _coroutine = null;
            OnEnd?.Invoke();
        }
    }
}