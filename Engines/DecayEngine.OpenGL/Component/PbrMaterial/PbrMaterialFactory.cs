using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Expression;
using DecayEngine.OpenGL.Object.Texture.Texture2D;

namespace DecayEngine.OpenGL.Component.PbrMaterial
{
    public class PbrMaterialFactory : IComponentFactory<PbrMaterialComponent, PbrMaterialResource>, IComponentFactory<IPbrMaterial, PbrMaterialResource>
    {
        public PbrMaterialComponent CreateComponent(PbrMaterialResource resource)
        {
            ColorTexture2D albedoTexture = null;
            if (resource.AlbedoTexture != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.AlbedoTexture);
                Texture2DResource texture = resolver.Resolve<Texture2DResource>();
                if (texture != null)
                {
                    albedoTexture = Texture2DFactory.Create<ColorTexture2D>(texture);
                }
            }

            NormalTexture2D normalTexture = null;
            if (resource.NormalTexture != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.NormalTexture);
                Texture2DResource texture = resolver.Resolve<Texture2DResource>();
                if (texture != null)
                {
                    normalTexture = Texture2DFactory.Create<NormalTexture2D>(texture);
                }
            }

            MetallicityTexture2D metallicityTexture = null;
            if (resource.MetallicityTexture != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.MetallicityTexture);
                Texture2DResource texture = resolver.Resolve<Texture2DResource>();
                if (texture != null)
                {
                    metallicityTexture = Texture2DFactory.Create<MetallicityTexture2D>(texture);
                }
            }

            RoughnessTexture2D roughnessTexture = null;
            if (resource.RoughnessTexture != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.RoughnessTexture);
                Texture2DResource texture = resolver.Resolve<Texture2DResource>();
                if (texture != null)
                {
                    roughnessTexture = Texture2DFactory.Create<RoughnessTexture2D>(texture);
                }
            }

            EmissionTexture2D emissionTexture = null;
            if (resource.EmissionTexture != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.EmissionTexture);
                Texture2DResource texture = resolver.Resolve<Texture2DResource>();
                if (texture != null)
                {
                    emissionTexture = Texture2DFactory.Create<EmissionTexture2D>(texture);
                }
            }

            return new PbrMaterialComponent
            {
                Resource = resource,
                AlbedoTexture = albedoTexture,
                AlbedoColor = resource.AlbedoColor,
                NormalTexture = normalTexture,
                MetallicityTexture = metallicityTexture,
                MetallicityFactor = resource.MetallicityFactor,
                RoughnessTexture = roughnessTexture,
                RoughnessFactor = resource.RoughnessFactor,
                EmissionTexture = emissionTexture,
                EmissionColor = resource.EmissionColor
            };
        }

        IPbrMaterial IComponentFactory<IPbrMaterial, PbrMaterialResource>.CreateComponent(PbrMaterialResource resource)
        {
            return CreateComponent(resource);
        }
    }
}