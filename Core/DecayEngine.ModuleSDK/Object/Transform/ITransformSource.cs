using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Transform
{
    public interface ITransformSource
    {
        Vector3 Position { get; set; }
        Quaternion Rotation { get; set; }
        Vector3 Scale { get; set; }

        void CopyStateFrom(ITransformSource source);
    }
}