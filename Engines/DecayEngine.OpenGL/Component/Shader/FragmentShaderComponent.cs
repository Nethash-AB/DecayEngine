using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.Shader
{
    public class FragmentShaderComponent : ShaderComponent
    {
        public FragmentShaderComponent(byte[] binaryShaderData) : base(binaryShaderData)
        {
        }

        public FragmentShaderComponent(string rawShaderData) : base(rawShaderData)
        {
        }

        public override uint Compile()
        {
            ShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            return base.Compile();
        }
    }
}