using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.Material;

namespace DecayEngine.ModuleSDK.Object.FrameBuffer
{
    public interface IFrameBuffer : IActivable, INameable, IDestroyable
    {
        Vector2 Size { get; set; }
        IShaderProgram ShaderProgram { get; set; }
        ByReference<IShaderProgram> ShaderProgramByRef { get; }
        IRenderTargetMaterial FrameBufferMaterial { get; }
        ByReference<IRenderTargetMaterial> FrameBufferMaterialByRef { get; }

        void Draw(ICamera camera, Matrix4 viewMatrix, Matrix4 projectionMatrix);
        void CopyDepthBuffer(IFrameBuffer target);
        void Bind(bool clearBuffers);
        void BindReadOnly(bool clearBuffers);
        void BindWriteOnly(bool clearBuffers);
        void Unbind();
    }
}