using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Transform
{
    public class DefaultTransformSource : ITransformSource
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public DefaultTransformSource()
        {
            Position = Vector3.Zero;
            Rotation = Quaternion.Identity;
            Scale = Vector3.One;
        }

        public DefaultTransformSource(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public void CopyStateFrom(ITransformSource source)
        {
            Position = source.Position;
            Rotation = source.Rotation;
            Scale = source.Scale;
        }
    }
}