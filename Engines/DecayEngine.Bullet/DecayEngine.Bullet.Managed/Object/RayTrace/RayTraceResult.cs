using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.RayTrace;

namespace DecayEngine.Bullet.Managed.Object.RayTrace
{
    public class RayTraceResult : IRayTraceResult
    {
        public Vector3 RaySource { get; }
        public Vector3 RayTarget { get; }

        public ICollisionObject HitCollisionObject { get; }
        public float HitFraction { get; }
        public Vector3 HitPointWorld { get; }
        public Vector3 HitNormalWorld { get; }

        public RayTraceResult(
            Vector3 raySource, Vector3 rayTarget,
            ICollisionObject hitCollisionObject,
            float hitFraction, Vector3 hitPointWorld, Vector3 hitNormalWorld)
        {
            RaySource = raySource;
            RayTarget = rayTarget;

            HitCollisionObject = hitCollisionObject;
            HitFraction = hitFraction;
            HitPointWorld = hitPointWorld;
            HitNormalWorld = hitNormalWorld;
        }
    }
}