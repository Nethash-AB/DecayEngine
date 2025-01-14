using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Texture.Texture3D;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.Texture3D
{
    public class EnvironmentTexture3D : Texture3D, IEnvironmentTexture
    {
        public EnvironmentTexture3D(TextureDataStructure[] textureDataStructures)
            : base(textureDataStructures)
        {
        }

        public override void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.FrameBufferEnvironment);
                GL.BindTexture(TextureTarget.TextureCubeMap, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferEnvironment] = this;
            });
        }

        public override void Unbind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferEnvironment] != this) return;

                GL.ActiveTexture((int) TextureTargets.FrameBufferEnvironment);
                GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            });
        }
    }
}