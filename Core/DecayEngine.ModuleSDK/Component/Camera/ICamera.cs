using System.Collections.Generic;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.ModuleSDK.Object.FrameBuffer;

namespace DecayEngine.ModuleSDK.Component.Camera
{
    public interface ICamera : ISceneAttachableComponent, IAudioListener
    {
        bool RenderToScreen { get; set; }
        bool ManualSize { get; set; }
        float ZNear { get; set; }
        float ZFar { get; set; }

        IDebugDrawer DebugDrawer { get; set; }
        IEnumerable<IDrawable> Drawables { get; }
        IEnumerable<ILight> Lights { get; }
        IEnumerable<IFrameBuffer> FrameBuffers { get; }

        void AddDrawable(IDrawable drawable);
        void RemoveDrawable(IDrawable drawable);

        void AddLight(ILight light);
        void RemoveLight(ILight light);

        void AddFrameBuffer(IFrameBuffer frameBuffer);
        void InsertFrameBufferAt(IFrameBuffer frameBuffer, uint position);
        void RemoveFrameBuffer(IFrameBuffer frameBuffer);
        void RemoveFrameBufferAt(uint position);

        void UpdateCameraProperties();

        void DrawGeometry();
    }
}