using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DecayEngine.SDL2.Native;
using DecayEngine.SDL2.WinRt;
using static DecayEngine.Demo.Uwp.GLTest;

namespace DecayEngine.Demo.Uwp
{
    public class App
    {
        [MTAThread]
        public static void Main(string[] args)
        {
//            while (!System.Diagnostics.Debugger.IsAttached)
//            {
//                Task.Delay(1).Wait();
//            }

            WinRtLoader.Run(() =>
            {
                Sdl2WinRtGlWindow sdl2Window = new Sdl2WinRtGlWindow("WinRT Test", true, new Vector2(0f, 0f));
                sdl2Window.OnInit += OnInit;
                sdl2Window.Run();

                while (true)
                {
                    glClearColor(1f, 0f, 0f, 1f);
                    glClear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    sdl2Window.Update();
                }
            });
        }

        private static void OnInit()
        {
            glEnable = Marshal.GetDelegateForFunctionPointer<Enable>(SDL.SDL_GL_GetProcAddress("glEnable"));
            glClear = Marshal.GetDelegateForFunctionPointer<Clear>(SDL.SDL_GL_GetProcAddress("glClear"));
            glClearColor = Marshal.GetDelegateForFunctionPointer<ClearColor>(SDL.SDL_GL_GetProcAddress("glClearColor"));

            glEnable(EnableCap.Multisample);
            glEnable(EnableCap.CullFace);
            glEnable(EnableCap.Blend);
        }
    }
}