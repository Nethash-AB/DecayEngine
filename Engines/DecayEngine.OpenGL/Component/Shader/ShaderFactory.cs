using System;
using System.IO;
using System.Linq;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Shader;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.OpenGL.Component.Shader
{
    public class ShaderFactory : IComponentFactory<ShaderComponent, ShaderResource>, IComponentFactory<IShader, ShaderResource>
    {
        public ShaderComponent CreateComponent(ShaderResource resource)
        {
            ShaderComponent component;
            if (GameEngine.RenderEngine.SupportsFeature(RenderEngineFeatures.SpirvShaders))
            {
                byte[] binaryShaderData = resource.Source.GetDataAsByteArray();

                switch (resource.Type)
                {
                    case ShaderType.Vertex:
                        component = new VertexShaderComponent(binaryShaderData) {Resource = resource};
                        break;
                    case ShaderType.Geometry:
                        component = new GeometryShaderComponent(binaryShaderData) {Resource = resource};
                        break;
                    case ShaderType.Fragment:
                        component = new FragmentShaderComponent(binaryShaderData) {Resource = resource};
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                ShaderFallback fallback = resource.Fallbacks.FirstOrDefault(f => f.Language == GameEngine.RenderEngine.FallbackShaderLanguage);
                GameEngine.LogAppendLine(LogSeverity.Debug, "OpenGL",
                    $"SPIR-V not supported on this platform, available native fallbacks for shader {resource.Id}: " +
                    $"{string.Join(", ", resource.Fallbacks.Select(f => f.Language.ToString()))}");
                if (fallback == null)
                {
                    throw new Exception($"Unable to find fallback of type {GameEngine.RenderEngine.FallbackShaderLanguage} for shader {resource.Id}");
                }

                string rawShaderData;
                using (TextReader reader = new StreamReader(fallback.Source.GetData()))
                {
                    rawShaderData = reader.ReadToEnd();
                }

                switch (resource.Type)
                {
                    case ShaderType.Vertex:
                        component = new VertexShaderComponent(rawShaderData) {Resource = resource};
                        break;
                    case ShaderType.Geometry:
                        component = new GeometryShaderComponent(rawShaderData) {Resource = resource};
                        break;
                    case ShaderType.Fragment:
                        component = new FragmentShaderComponent(rawShaderData) {Resource = resource};
                        break;
                    default:
                        return null;
                }
            }

            return component;
        }

        IShader IComponentFactory<IShader, ShaderResource>.CreateComponent(ShaderResource resource)
        {
            return CreateComponent(resource);
        }
    }
}