using System;

namespace DecayEngine.ModuleSDK.Coroutines.Results
{
    internal class CoroutineFaultResult : IYieldResult
    {
        public bool Ready => true;
        public Exception Exception { get; private set; }

        public CoroutineFaultResult(Exception exception)
        {
            Exception = exception;
        }
    }
}