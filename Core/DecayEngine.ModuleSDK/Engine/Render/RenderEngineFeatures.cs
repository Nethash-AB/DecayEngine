using System;

namespace DecayEngine.ModuleSDK.Engine.Render
{
    [Flags]
    public enum RenderEngineFeatures
    {
        None = 0,
        Srgb = 1,
        GeometryShaders = 2,
        CompressedTextures = 4,
        SpirvShaders = 8
    }
}