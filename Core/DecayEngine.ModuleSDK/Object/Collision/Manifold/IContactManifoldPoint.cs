using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Collision.Manifold
{
    public interface IContactManifoldPoint
    {
        Vector3 LocalPositionA { get; }
        Vector3 LocalPositionB { get; }
        Vector3 WorldPositionA { get; }
        Vector3 WorldPositionB { get; }
        float Impulse { get; }
        float ImpulseLateral { get; }
        float Damping { get; }
        float Stiffness { get; }
        float LinearFriction { get; }
        float RollingFriction { get; }
        float Restitution { get; }
    }
}