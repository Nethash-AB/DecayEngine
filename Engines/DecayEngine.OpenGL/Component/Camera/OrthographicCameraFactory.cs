using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;

namespace DecayEngine.OpenGL.Component.Camera
{
    public class OrthographicCameraFactory : IComponentFactory<OrthographicCameraComponent>, IComponentFactory<ICameraOrtho>
    {
        public OrthographicCameraComponent CreateComponent()
        {
            OrthographicCameraComponent camera = new OrthographicCameraComponent();
            GameEngine.RenderEngine.TrackCamera(camera);
            return camera;
        }

        ICameraOrtho IComponentFactory<ICameraOrtho>.CreateComponent()
        {
            return CreateComponent();
        }
    }
}