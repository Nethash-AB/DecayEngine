using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.OpenGL.Component.Light.Directional;
using DecayEngine.OpenGL.Component.Light.Point;
using DecayEngine.OpenGL.Component.Light.Spot;

namespace DecayEngine.OpenGL.Component.Light
{
    public class LightFactory :
        IComponentFactory<PointLightComponent>, IComponentFactory<IPointLight>,
        IComponentFactory<DirectionalLightComponent>, IComponentFactory<IDirectionalLight>,
        IComponentFactory<SpotLightComponent>, IComponentFactory<ISpotLight>
    {
        PointLightComponent IComponentFactory<PointLightComponent>.CreateComponent()
        {
            PointLightComponent light = new PointLightComponent();
            return light;
        }

        IPointLight IComponentFactory<IPointLight>.CreateComponent()
        {
            return ((IComponentFactory<PointLightComponent>) this).CreateComponent();
        }

        DirectionalLightComponent IComponentFactory<DirectionalLightComponent>.CreateComponent()
        {
            DirectionalLightComponent light = new DirectionalLightComponent();
            return light;
        }

        IDirectionalLight IComponentFactory<IDirectionalLight>.CreateComponent()
        {
            return ((IComponentFactory<DirectionalLightComponent>) this).CreateComponent();
        }

        SpotLightComponent IComponentFactory<SpotLightComponent>.CreateComponent()
        {
            SpotLightComponent light = new SpotLightComponent();
            return light;
        }

        ISpotLight IComponentFactory<ISpotLight>.CreateComponent()
        {
            return ((IComponentFactory<SpotLightComponent>) this).CreateComponent();
        }
    }
}