using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.Sprite;

namespace DecayEngine.ModuleSDK.Component.Sprite
{
    public interface IRenderTargetSprite : ISprite, IComponent
    {
        IRenderFrameBuffer SourceFrameBuffer { get; set; }
        ByReference<IRenderFrameBuffer> SourceFrameBufferByRef { get; set; }
    }
}