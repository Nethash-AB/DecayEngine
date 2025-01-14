using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Transform
{
    public class OffsetTransformSource : ITransformSource
    {
        public ITransformSource BaseTransformSource { get; set; }
        public Vector3 OffsetPosition { get; set; }
        public Quaternion OffsetRotation { get; set; }
        public Vector3 OffsetScale { get; set; }

        public Vector3 Position
        {
            get => BaseTransformSource.Position + OffsetPosition;
            set => BaseTransformSource.Position = value - OffsetPosition;
        }

        public Quaternion Rotation
        {
            get => BaseTransformSource.Rotation * OffsetRotation;
            set => BaseTransformSource.Rotation = value * OffsetRotation.Inverted; // ToDo: No idea if this actually works.
        }

        public Vector3 Scale
        {
            get => BaseTransformSource.Scale * OffsetScale;
            set => BaseTransformSource.Scale = value / OffsetScale;
        }

        public OffsetTransformSource(ITransformSource baseTransformSource)
        {
            BaseTransformSource = baseTransformSource;
            OffsetPosition = Vector3.Zero;
            OffsetRotation = Quaternion.Identity;
            OffsetScale = Vector3.One;
        }

        public OffsetTransformSource(ITransformSource baseTransformSource, Vector3 offsetPosition, Quaternion offsetRotation, Vector3 offsetScale)
            : this(baseTransformSource)
        {
            OffsetPosition = offsetPosition;
            OffsetRotation = offsetRotation;
            OffsetScale = offsetScale;
        }

        public void CopyStateFrom(ITransformSource source)
        {
            BaseTransformSource = source;
        }
    }
}