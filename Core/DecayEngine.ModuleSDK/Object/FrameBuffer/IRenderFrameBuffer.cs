using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;

namespace DecayEngine.ModuleSDK.Object.FrameBuffer
{
    public interface IRenderFrameBuffer : IFrameBuffer
    {
        PostProcessingStage PostProcessingStage { get; set; }
    }
}