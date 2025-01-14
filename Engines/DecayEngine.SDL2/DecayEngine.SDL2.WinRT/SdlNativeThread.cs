using System;
using System.Threading;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Threading;
using DecayEngine.SDL2.Native.Sdl2Interop;

namespace DecayEngine.SDL2.WinRT
{
    public class SdlNativeThread : ManagedEngineThread
    {
        public SdlNativeThread(float ticksPerSecond) : base(ticksPerSecond)
        {
        }

        public override void Run()
        {
            if (Running) return;

            SDL.SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");
            Task.Run(() => SDL.SDL_WinRTRunApp(SdlInitFunction, IntPtr.Zero));
        }

        private int SdlInitFunction(int argc, IntPtr[] argv)
        {
            Thread = Thread.CurrentThread;
            Thread.Name = "SDL2";

            Running = true;
            Loop();

            return 0;
        }
    }
}
