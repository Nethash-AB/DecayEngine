using System;
using DecayEngine.SDL2.Native;

namespace DecayEngine.Demo.Uwp
{
    public static class WinRtLoader
    {
        private static Action _entryPoint;
        private static bool _initialized;

        public static void Run(Action entryPoint)
        {
            if (_initialized)
            {
                entryPoint();
                return;
            }

            _entryPoint = entryPoint;
            SDL.SDL_SetHint("SDL_WINRT_HANDLE_BACK_BUTTON", "1");
            SDL.SDL_WinRTRunApp(SdlMain, IntPtr.Zero);
        }

        private static int SdlMain(int argc, IntPtr[] argv)
        {
            _initialized = true;
            try
            {
                _entryPoint?.Invoke();
                return 0;
            }
            catch
            {
                return 1;
            }
        }
    }
}
