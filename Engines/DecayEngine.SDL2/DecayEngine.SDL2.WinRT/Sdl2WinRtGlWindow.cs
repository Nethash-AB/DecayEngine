using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Threading;
using DecayEngine.SDL2.Native.GameSurface;
using DecayEngine.SDL2.Native.Sdl2Interop;
using DecayEngine.SDL2.WinRT;

namespace DecayEngine.SDL2.WinRt
{
    public class Sdl2WinRtGlWindow : Sdl2GameSurface
    {
        private IntPtr _glContext;

        public override IEngineThread UiThread { get; }

        public override event Action OnInit;
        public override event Action<Vector2> OnResize;
        public override event Action OnQuit;

        public override string Title
        {
            get => ApplicationView.GetForCurrentView().Title;
            set => ApplicationView.GetForCurrentView().Title = value;
        }

        public override bool FullScreen
        {
            get => ApplicationView.GetForCurrentView().IsFullScreenMode;
            set
            {
                if (!FullScreen && value)
                {
                    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                }
                else if (FullScreen && !value)
                {
                    ApplicationView.GetForCurrentView().ExitFullScreenMode();
                }
            }
        }

        public Sdl2WinRtGlWindow(string title, bool vsync, Vector2 size, bool debugGraphics, int ticksPerSecond) : base(title, vsync, size, debugGraphics)
        {
            UiThread = new SdlNativeThread(ticksPerSecond);
        }

        public override Task Run()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            SDL.SDL_GLcontext glFlags = SDL.SDL_GLcontext.SDL_GL_CONTEXT_ROBUST_ACCESS_FLAG;
#if DEBUG
            glFlags |= SDL.SDL_GLcontext.SDL_GL_CONTEXT_DEBUG_FLAG;
#endif
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_FLAGS, (int) glFlags);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int) SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_ES);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RETAINED_BACKING, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ACCELERATED_VISUAL, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);

//          string appDir = SDL.SDL_WinRTGetFSPathUTF8(SDL.SDL_WinRT_Path.SDL_WINRT_PATH_INSTALLED_LOCATION);
//          SDL.SDL_GL_LoadLibrary(appDir + "\\libEGL.dll");

          SDL.SDL_SetHint(SDL.SDL_HINT_VIDEO_HIGHDPI_DISABLED,"1");

            SDL.SDL_GetCurrentDisplayMode(0, out SDL.SDL_DisplayMode displayMode);
            int width = InitSize.X > 0 ? (int) InitSize.X : displayMode.w;
            int height = InitSize.Y > 0 ? (int) InitSize.Y : displayMode.h;

            WindowHandle = SDL.SDL_CreateWindow(
                "",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                width,
                height,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN |
                SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS |
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            _glContext = SDL.SDL_GL_CreateContext(WindowHandle);

            if (InitVsync)
            {
                int vSyncResult = SDL.SDL_GL_SetSwapInterval(-1);
                if (vSyncResult == -1)
                {
                    SDL.SDL_GL_SetSwapInterval(1);
                }
            }
            else
            {
                SDL.SDL_GL_SetSwapInterval(0);
            }

            SDL.SDL_GL_MakeCurrent(WindowHandle, _glContext);

            Title = InitTitle;

            SDL.SDL_ShowWindow(WindowHandle);

            OnInit?.Invoke();

            return Task.CompletedTask;
        }

        public override void Update()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        OnQuit?.Invoke();
                        break;
                    case SDL.SDL_EventType.SDL_WINDOWEVENT:
                        switch (e.window.windowEvent)
                        {
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                            case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                OnResize?.Invoke(Size);
                                break;
                        }

                        break;
                }
            }

            SDL.SDL_GL_SwapWindow(WindowHandle);
        }

        public override void Quit()
        {
            SDL.SDL_DestroyWindow(WindowHandle);
            SDL.SDL_GL_DeleteContext(_glContext);
            WindowHandle = IntPtr.Zero;
        }

        public override IntPtr GetRenderFunctionPtr(string functionName) => SDL.SDL_GL_GetProcAddress(functionName);
    }
}