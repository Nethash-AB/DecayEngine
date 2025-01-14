using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Transform
{
    public class Transform
    {
        public ITransformSource TransformSource { get; set; }

        public Vector3 Position
        {
            get => TransformSource.Position;
            set => TransformSource.Position = value;
        }

        public Quaternion Rotation
        {
            get => TransformSource.Rotation;
            set => TransformSource.Rotation = value;
        }

        public Vector3 Scale
        {
            get => TransformSource.Scale;
            set => TransformSource.Scale = value;
        }

        public Vector3 Right => Rotation * Vector3.UnitX;
        public Vector3 Up => Rotation * Vector3.UnitY;
        public Vector3 Forward => Rotation * -Vector3.UnitZ;

        public Matrix4 TransformMatrix
        {
            get => Matrix4.CreateScale(Scale) * RotationMatrix * Matrix4.CreateTranslation(Position);
            set
            {
                Position = value.ExtractTranslation();
                Rotation = value.ExtractRotation();
                Scale = value.ExtractScale();
            }
        }

        public Matrix4 RotationMatrix
        {
            get => Matrix4.CreateFromQuaternion(Rotation);
            set => Rotation = value.ExtractRotation();
        }

        public Transform()
        {
            TransformSource = new DefaultTransformSource();
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale) : this()
        {
            TransformSource = new DefaultTransformSource(position, rotation, scale);
        }

        public Transform(ITransformSource transformSource)
        {
            TransformSource = transformSource;
        }
    }
}