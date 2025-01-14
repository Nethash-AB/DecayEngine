using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Collision;

namespace DecayEngine.ModuleSDK.Object.RayTrace
{
    public interface IRayTraceResult
    {
        Vector3 RaySource { get; }
        Vector3 RayTarget { get; }

        ICollisionObject HitCollisionObject { get; }
        float HitFraction { get; }
        Vector3 HitPointWorld { get; }
        Vector3 HitNormalWorld { get; }
    }
}