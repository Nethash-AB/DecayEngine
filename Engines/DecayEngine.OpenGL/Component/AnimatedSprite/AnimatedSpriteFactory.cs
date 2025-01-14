using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Sprite;

namespace DecayEngine.OpenGL.Component.AnimatedSprite
{
    public class AnimatedSpriteFactory : IComponentFactory<AnimatedSpriteComponent>, IComponentFactory<IAnimatedSprite>
    {
        public AnimatedSpriteComponent CreateComponent()
        {
            return new AnimatedSpriteComponent();
        }

        IAnimatedSprite IComponentFactory<IAnimatedSprite>.CreateComponent()
        {
            return CreateComponent();
        }
    }
}