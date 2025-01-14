using System;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.SDL2.Native.GameSurface;
using DecayEngine.SDL2.Native.Sdl2Interop;

namespace DecayEngine.SDL2.Desktop
{
    public class Sdl2DesktopGlWindow : Sdl2GameSurface
    {
        private IntPtr _glContext;
        private readonly ISurfaceIcon _icon;

        public override event Action OnInit;
        public override event Action<Vector2> OnResize;
        public override event Action OnQuit;

        public Sdl2DesktopGlWindow(string title, bool vsync, Vector2 size, bool debugGraphics, ISurfaceIcon icon)
            : base(title, vsync, size, debugGraphics)
        {
            _icon = icon;
        }

        public override Task Run()
        {
#if DEBUG
            if (DebugGraphics)
            {
                RenderDocDebugger.WaitForGraphicsDebugger();
            }
#endif
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            SDL.SDL_InitSubSystem(SDL.SDL_INIT_GAMECONTROLLER);
            SDL.SDL_InitSubSystem(SDL.SDL_INIT_JOYSTICK);
            SDL.SDL_InitSubSystem(SDL.SDL_INIT_HAPTIC);

            SDL.SDL_GLcontext glFlags = SDL.SDL_GLcontext.SDL_GL_CONTEXT_ROBUST_ACCESS_FLAG;
#if DEBUG
            glFlags |= SDL.SDL_GLcontext.SDL_GL_CONTEXT_DEBUG_FLAG;
#endif

            if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                glFlags |= SDL.SDL_GLcontext.SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG;
            }
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_FLAGS, (int) glFlags);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int) SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 4);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 4);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, 4);

            SDL.SDL_SetHint(SDL.SDL_HINT_VIDEO_HIGHDPI_DISABLED,"1");

            SDL.SDL_GetCurrentDisplayMode(0, out SDL.SDL_DisplayMode displayMode);
            int width = InitSize.X > 0 ? (int) InitSize.X : displayMode.w;
            int height = InitSize.Y > 0 ? (int) InitSize.Y : displayMode.h;

            WindowHandle = SDL.SDL_CreateWindow(
                InitTitle,
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                width,
                height,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN |
                SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS | SDL.SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS);

            if (_icon != null)
            {
                IntPtr iconSurface = SDL.SDL_CreateRGBSurfaceWithFormatFrom(_icon.Handle,
                    _icon.Width, _icon.Height, _icon.Depth, _icon.Pitch, SDL.SDL_PIXELFORMAT_RGBA8888);

                SDL.SDL_SetWindowIcon(WindowHandle, iconSurface);
                SDL.SDL_FreeSurface(iconSurface);
            }

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
                        if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                        {
                            OnResize?.Invoke(Size);
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
            _icon.Dispose();
        }

        public override IntPtr GetRenderFunctionPtr(string functionName) => SDL.SDL_GL_GetProcAddress(functionName);
    }
}