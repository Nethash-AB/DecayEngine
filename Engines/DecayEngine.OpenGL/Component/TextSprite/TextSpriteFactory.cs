using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Sprite;

namespace DecayEngine.OpenGL.Component.TextSprite
{
    public class TextSpriteFactory : IComponentFactory<TextSpriteComponent, FontResource>, IComponentFactory<ITextSprite, FontResource>
    {
        public TextSpriteComponent CreateComponent(FontResource resource)
        {
            return new TextSpriteComponent {Resource = resource};
        }

        ITextSprite IComponentFactory<ITextSprite, FontResource>.CreateComponent(FontResource resource)
        {
            return CreateComponent(resource);
        }
    }
}