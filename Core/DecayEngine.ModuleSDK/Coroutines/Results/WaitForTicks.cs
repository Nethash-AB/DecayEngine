using System.Diagnostics;

namespace DecayEngine.ModuleSDK.Coroutines.Results
{
    public class WaitForTicks : IYieldResult
    {
        private readonly int _ticksToWait;
        private int _currentTickCount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool Ready
        {
            get
            {
                _currentTickCount++;
                return _currentTickCount > _ticksToWait;
            }
        }

        private bool ReadyDebug => _currentTickCount > _ticksToWait;

        public WaitForTicks(int ticksToWait)
        {
            _ticksToWait = ticksToWait;
        }
    }
}