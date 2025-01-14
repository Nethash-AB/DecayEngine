using System;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Threading;
using DecayEngine.SDL2.Native.Sdl2Interop;

namespace DecayEngine.SDL2.Native.GameSurface
{
    public abstract class Sdl2GameSurface : IGameSurface
    {
        protected IntPtr WindowHandle { get; set; }
        protected string InitTitle { get; }
        protected bool InitVsync { get; }
        protected Vector2 InitSize { get; }
        protected bool DebugGraphics { get; }

        public virtual string Title {
            get => SDL.SDL_GetWindowTitle(WindowHandle);
            set => SDL.SDL_SetWindowTitle(WindowHandle, value);
        }

        public virtual Vector2 Position
        {
            get
            {
                SDL.SDL_GetWindowPosition(WindowHandle, out int x, out int y);
                return new Vector2(x, y);
            }
            set => SDL.SDL_SetWindowPosition(WindowHandle, (int) value.X, (int) value.Y);
        }

        public Vector2 Size
        {
            get
            {
                SDL.SDL_GetWindowSize(WindowHandle, out int x, out int y);
                return new Vector2(x, y);
            }
            set => SDL.SDL_SetWindowSize(WindowHandle, (int) value.X, (int) value.Y);
        }

        public virtual bool Maximized
        {
            get => ((SDL.SDL_WindowFlags) SDL.SDL_GetWindowFlags(WindowHandle)).HasFlag(SDL.SDL_WindowFlags.SDL_WINDOW_MAXIMIZED);
            set
            {
                if (value)
                {
                    SDL.SDL_MaximizeWindow(WindowHandle);
                }
                else
                {
                    SDL.SDL_RestoreWindow(WindowHandle);
                }
            }
        }

        public virtual bool FullScreen
        {
            get => ((SDL.SDL_WindowFlags) SDL.SDL_GetWindowFlags(WindowHandle)).HasFlag(SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
            set
            {
                if (value)
                {
                    SDL.SDL_RestoreWindow(WindowHandle);
                    SDL.SDL_GetCurrentDisplayMode(0, out SDL.SDL_DisplayMode displayMode);
                    SDL.SDL_SetWindowSize(WindowHandle, displayMode.w, displayMode.h);
                    SDL.SDL_MaximizeWindow(WindowHandle);
                    SDL.SDL_SetWindowFullscreen(WindowHandle, (uint) SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
                }
                else
                {
                    SDL.SDL_SetWindowFullscreen(WindowHandle, 0);
                    SDL.SDL_RestoreWindow(WindowHandle);
                }
            }
        }

        public virtual bool Borderless
        {
            get => !FullScreen && ((SDL.SDL_WindowFlags) SDL.SDL_GetWindowFlags(WindowHandle)).HasFlag(SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
            set
            {
                if (FullScreen) return;
                SDL.SDL_SetWindowBordered(WindowHandle, value ? SDL.SDL_bool.SDL_FALSE : SDL.SDL_bool.SDL_TRUE);
            }
        }

        public bool VSync
        {
            get => SDL.SDL_GL_GetSwapInterval() != 0;
            set => SDL.SDL_GL_SetSwapInterval(value ? 1 : 0);
        }

        public int RefreshRate
        {
            get
            {
                SDL.SDL_GetCurrentDisplayMode(0, out SDL.SDL_DisplayMode displayMode);
                return displayMode.refresh_rate;
            }
        }

        public bool Alive => WindowHandle != IntPtr.Zero;
        public virtual IEngineThread UiThread => null;

        public abstract event Action OnInit;
        public abstract event Action<Vector2> OnResize;
        public abstract event Action OnQuit;

        protected Sdl2GameSurface(string title, bool vsync, Vector2 size, bool debugGraphics)
        {
            InitTitle = title;
            InitVsync = vsync;
            InitSize = size;
            DebugGraphics = debugGraphics;
        }

        public abstract Task Run();
        public abstract void Update();
        public abstract void Quit();

        public abstract IntPtr GetRenderFunctionPtr(string functionName);
    }
}