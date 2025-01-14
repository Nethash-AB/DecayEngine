using System.Diagnostics;

namespace DecayEngine.ModuleSDK.Coroutines.Results
{
    public class WaitForSeconds : IYieldResult
    {
        private readonly Stopwatch _timer;
        private readonly float _secondsToWait;
        private bool _completed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool Ready
        {
            get
            {
                if (_completed)
                {
                    return true;
                }

                if (!_timer.IsRunning)
                {
                    _timer.Start();
                }

                if (_timer.Elapsed.TotalSeconds <= _secondsToWait) return false;

                return _completed = true;
            }
        }

        private bool ReadyDebug => _completed;

        public WaitForSeconds(float secondsToWait)
        {
            _timer = new Stopwatch();
            _secondsToWait = secondsToWait;
        }
    }
}