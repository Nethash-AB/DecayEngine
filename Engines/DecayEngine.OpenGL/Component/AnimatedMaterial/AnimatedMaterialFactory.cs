using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Expression;
using DecayEngine.OpenGL.Object.Texture.Texture2D;

namespace DecayEngine.OpenGL.Component.AnimatedMaterial
{
    public class AnimatedMaterialFactory
        : IComponentFactory<AnimatedMaterialComponent, AnimatedMaterialResource>, IComponentFactory<IAnimatedMaterial, AnimatedMaterialResource>
    {
        public AnimatedMaterialComponent CreateComponent(AnimatedMaterialResource resource)
        {
            ColorTexture2D diffuseTexture = null;
            if (resource.DiffuseTexture != null)
            {
                PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.DiffuseTexture);
                Texture2DResource texture = resolver.Resolve<Texture2DResource>();
                if (texture != null)
                {
                    diffuseTexture = Texture2DFactory.Create<ColorTexture2D>(texture);
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

            return new AnimatedMaterialComponent
            {
                Resource = resource,
                DiffuseTexture = diffuseTexture,
                NormalTexture = normalTexture
            };
        }

        IAnimatedMaterial IComponentFactory<IAnimatedMaterial, AnimatedMaterialResource>.CreateComponent(AnimatedMaterialResource resource)
        {
            return CreateComponent(resource);
        }
    }
}