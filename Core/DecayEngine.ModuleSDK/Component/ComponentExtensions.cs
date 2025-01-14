using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.Mesh;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Component.Shader;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Component
{
    public static class ComponentExtensions
    {
        public static void ClearParent(this IComponent component)
        {
            component.Parent?.RemoveComponent(component);
            if (component is ISceneAttachableComponent sceneComponent)
            {
                ((IParentable<IScene>) sceneComponent).Parent?.RemoveComponent(sceneComponent);
            }

            component.SetParent(null);
        }

        public static bool IsComponentType(this IComponent component, ComponentType componentType)
        {
            return componentType switch
            {
                ComponentType.AnimatedMaterial => (component is IAnimatedMaterial),
                ComponentType.PbrMaterial => (component is IPbrMaterial),
                ComponentType.Shader => (component is IShader),
                ComponentType.ShaderProgram => (component is IShaderProgram),
                ComponentType.Script => (component is IScript),
                ComponentType.AnimatedSprite => (component is IAnimatedSprite),
                ComponentType.RenderTargetSprite => (component is IRenderTargetSprite),
                ComponentType.Mesh => (component is IMesh),
                ComponentType.TextSprite => (component is ITextSprite),
                ComponentType.Camera => (component is ICamera),
                ComponentType.CameraPersp => (component is ICameraPersp),
                ComponentType.CameraOrtho => (component is ICameraOrtho),
                ComponentType.SoundBank => (component is ISoundBank),
                ComponentType.Sound => (component is ISound),
                ComponentType.RigidBody => (component is IRigidBody),
                _ => false
            };
        }
    }
}