using System;

namespace DecayEngine.ModuleSDK.Engine.Render
{
    public interface ISurfaceIcon : IDisposable
    {
        IntPtr Handle { get; }
        int Width { get; }
        int Height { get; }
        int Depth { get; }
        int Pitch { get; }
    }
}