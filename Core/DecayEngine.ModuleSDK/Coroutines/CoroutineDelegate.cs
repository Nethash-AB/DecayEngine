using System.Collections.Generic;
using DecayEngine.DecPakLib;

namespace DecayEngine.ModuleSDK.Coroutines
{
    public delegate IEnumerator<IYieldResult> CoroutineDelegate();
    public delegate IEnumerator<IYieldResult> CoroutineDelegate<TState>(ByReference<TState> state);
}