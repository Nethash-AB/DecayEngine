using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Expression;
using DecayEngine.OpenGL.Component.Shader;

namespace DecayEngine.OpenGL.Component.ShaderProgram
{
    public class ShaderProgramFactory : IComponentFactory<ShaderProgramComponent, ShaderProgramResource>, IComponentFactory<IShaderProgram, ShaderProgramResource>
    {
        public ShaderProgramComponent CreateComponent(ShaderProgramResource resource)
        {
            VertexShaderComponent vertexShader = null;
            if (resource.VertexShader != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.VertexShader);
                ShaderResource shader = resolver.Resolve<ShaderResource>();
                if (shader != null)
                {
                    vertexShader = (VertexShaderComponent) GameEngine.CreateComponent(shader);
                }
            }

            GeometryShaderComponent geometryShader = null;
            if (resource.GeometryShader != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.GeometryShader);
                ShaderResource shader = resolver.Resolve<ShaderResource>();
                if (shader != null)
                {
                    geometryShader = (GeometryShaderComponent) GameEngine.CreateComponent(shader);
                }
            }

            FragmentShaderComponent fragmentShader = null;
            if (resource.FragmentShader != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.FragmentShader);
                ShaderResource shader = resolver.Resolve<ShaderResource>();
                if (shader != null)
                {
                    fragmentShader = (FragmentShaderComponent) GameEngine.CreateComponent(shader);
                }
            }

            return new ShaderProgramComponent
            {
                Resource = resource,
                VertexShader = vertexShader,
                GeometryShader = geometryShader,
                FragmentShader = fragmentShader
            };
        }

        IShaderProgram IComponentFactory<IShaderProgram, ShaderProgramResource>.CreateComponent(ShaderProgramResource resource)
        {
            return CreateComponent(resource);
        }
    }
}