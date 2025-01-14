using System;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Exports.BaseExports.Camera;

namespace DecayEngine.OpenGL.Component.Camera
{
    public sealed class OrthographicCameraComponent : CameraComponent, ICameraOrtho
    {
        public override Type ExportType => typeof(OrthographicCameraExport);
        public Vector2 ViewSpaceBBox { get; private set; }

        public OrthographicCameraComponent()
        {
            ZNear = 0.1f;
            ZFar = 100f;

            UpdateCameraProperties();
        }

        public override void UpdateCameraProperties()
        {
            base.UpdateCameraProperties();
            RecalculateViewSpaceBBox(out float _, out float _);
        }

        protected override void GetCameraMatrices(out Matrix4 viewMatrix, out Matrix4 projectionMatrix)
        {
            RecalculateViewSpaceBBox(out float right, out float bottom);

            projectionMatrix = Matrix4.CreateOrthographicOffCenter(-right, right, bottom, -bottom, ZNear, ZFar);
            viewMatrix = Matrix4.CreateTranslation(new Vector3(0, 0, -ZNear));
        }

        private void RecalculateViewSpaceBBox(out float right, out float bottom)
        {
            float aspectRatio = Transform.Scale.X / Transform.Scale.Y;
            right = aspectRatio * Transform.Scale.X / 2f;
            bottom = -1f * Transform.Scale.Y / 2f;
            ViewSpaceBBox = new Vector2(right, bottom);
        }
    }
}