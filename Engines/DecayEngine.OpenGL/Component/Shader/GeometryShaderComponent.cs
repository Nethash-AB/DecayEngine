using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.Shader
{
    public class GeometryShaderComponent : ShaderComponent
    {
        public GeometryShaderComponent(byte[] binaryShaderData) : base(binaryShaderData)
        {
        }

        public GeometryShaderComponent(string rawShaderData) : base(rawShaderData)
        {
        }

        public override uint Compile()
        {
            ShaderHandle = GL.CreateShader(ShaderType.GeometryShader);
            return base.Compile();
        }
    }
}