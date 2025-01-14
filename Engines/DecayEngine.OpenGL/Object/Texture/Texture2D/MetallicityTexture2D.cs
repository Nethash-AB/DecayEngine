using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Texture.Texture2D;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.Texture2D
{
    public class MetallicityTexture2D : Texture2D, IMetallicityTexture
    {
        public MetallicityTexture2D(TextureDataStructure textureDataStructure)
            : base(textureDataStructure)
        {
        }

        public override void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.Metallicity);
                GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.Metallicity] = this;
            });
        }

        public override void Unbind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (OpenGlGlobalState.TextureTargetState[TextureTargets.Metallicity] != this) return;

                GL.ActiveTexture((int) TextureTargets.Metallicity);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            });
        }
    }
}