using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;

namespace DecayEngine.OpenGL.Component.Camera
{
    public class PerspectiveCameraFactory : IComponentFactory<PerspectiveCameraComponent>, IComponentFactory<ICameraPersp>
    {
        public PerspectiveCameraComponent CreateComponent()
        {
            PerspectiveCameraComponent camera = new PerspectiveCameraComponent();
            GameEngine.RenderEngine.TrackCamera(camera);
            return camera;
        }

        ICameraPersp IComponentFactory<ICameraPersp>.CreateComponent()
        {
            return CreateComponent();
        }
    }
}