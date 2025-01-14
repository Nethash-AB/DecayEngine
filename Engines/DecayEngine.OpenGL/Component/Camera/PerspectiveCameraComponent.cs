using System;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Exports.BaseExports.Camera;
using DecayEngine.ModuleSDK.Object.Texture.Texture3D;
using DecayEngine.OpenGL.Object.Texture.Texture3D;

namespace DecayEngine.OpenGL.Component.Camera
{
    public sealed class PerspectiveCameraComponent : CameraComponent, ICameraPersp
    {
        private Texture3DResource _environmentTextureResource;

        public float FieldOfView { get; set; }

        public Texture3DResource EnvironmentTextureResource
        {
            get => _environmentTextureResource;
            set
            {
                if (value == _environmentTextureResource) return;

                EnvironmentTexture = Texture3DFactory.Create<EnvironmentTexture3D>(value);
                EnvironmentTexture.Active = true;
                _environmentTextureResource = value;
            }
        }
        public IEnvironmentTexture EnvironmentTexture { get; private set; }

        public override Type ExportType => typeof(PerspectiveCameraExport);

        public PerspectiveCameraComponent()
        {
            ZNear = 0.1f;
            ZFar = 100f;
            FieldOfView = MathHelper.PiOver2;
        }

        protected override void GetCameraMatrices(out Matrix4 viewMatrix, out Matrix4 projectionMatrix)
        {
            float aspectRatio = Transform.Scale.X / Transform.Scale.Y;

            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, ZNear, ZFar);
//            Matrix4 transformMatrix = Transform.TransformMatrix.ClearScale();
//            viewMatrix = transformMatrix.Inverted();
//            viewMatrix = Matrix4.Invert(Transform.RotationMatrix * Matrix4.CreateTranslation(Transform.Position));
//            viewMatrix = Matrix4.CreateTranslation(-Transform.Position) * Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted());
//            viewMatrix = Matrix4.CreateTranslation(-Transform.Position) * Matrix4.CreateFromQuaternion(Transform.Rotation);
//            Matrix4 rotationMatrix = Transform.RotationMatrix;
//            rotationMatrix.Transpose();
//            viewMatrix = Matrix4.LookAt(Transform.Position, Transform.Position + rotationMatrix.Forward, Vector3.UnitY);
//            viewMatrix = Matrix4.LookAt(Transform.Position, Transform.Position + rotationMatrix.Forward, rotationMatrix.Up);
//            viewMatrix = Matrix4.CreateTranslation(-Transform.Position) * Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted());

//            viewMatrix = Matrix4.Transpose(Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted()));

            viewMatrix = Matrix4.CreateTranslation(-Transform.Position) * Matrix4.Transpose(Transform.RotationMatrix);

//            viewMatrix = Matrix4.CreateTranslation(-Transform.Position) * Matrix4.Transpose(Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted()));
//            viewMatrix = Matrix4.CreateTranslation(-Transform.Position) * Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted());
//            viewMatrix = Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted()) * Matrix4.CreateTranslation(-Transform.Position);
//            viewMatrix = Matrix4.Transpose(Matrix4.CreateFromQuaternion(Transform.Rotation.Inverted())) * Matrix4.CreateTranslation(-Transform.Position);
        }
    }
}