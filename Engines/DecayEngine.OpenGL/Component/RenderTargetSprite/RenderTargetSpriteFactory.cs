using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Sprite;

namespace DecayEngine.OpenGL.Component.RenderTargetSprite
{
    public class RenderTargetSpriteFactory : IComponentFactory<RenderTargetSpriteComponent>, IComponentFactory<IRenderTargetSprite>
    {
        public RenderTargetSpriteComponent CreateComponent()
        {
            return new RenderTargetSpriteComponent();
        }

        IRenderTargetSprite IComponentFactory<IRenderTargetSprite>.CreateComponent()
        {
            return CreateComponent();
        }
    }
}