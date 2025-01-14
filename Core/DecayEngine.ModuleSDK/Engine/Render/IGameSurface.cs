using System;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.ModuleSDK.Engine.Render
{
    public delegate IntPtr GetRenderFunctionPtrDelegate(string functionName);

    public interface IGameSurface
    {
        string Title { get; set; }
        Vector2 Position { get; set; }
        Vector2 Size { get; set; }
        bool Maximized { get; set; }
        bool FullScreen { get; set; }
        bool Borderless { get; set; }
        bool VSync { get; set; }
        int RefreshRate { get; }
        bool Alive { get; }
        IEngineThread UiThread { get; }

        event Action OnInit;
        event Action<Vector2> OnResize;
        event Action OnQuit;

        Task Run();
        void Update();
        void Quit();

        IntPtr GetRenderFunctionPtr(string functionName);
    }
}