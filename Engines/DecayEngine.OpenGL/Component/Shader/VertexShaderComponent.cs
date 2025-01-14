using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.Shader
{
    public class VertexShaderComponent : ShaderComponent
    {
        public VertexShaderComponent(byte[] binaryShaderData) : base(binaryShaderData)
        {
        }

        public VertexShaderComponent(string rawShaderData) : base(rawShaderData)
        {
        }

        public override uint Compile()
        {
            ShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            return base.Compile();
        }
    }
}