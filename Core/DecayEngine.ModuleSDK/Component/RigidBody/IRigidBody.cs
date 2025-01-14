using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Math;

namespace DecayEngine.ModuleSDK.Component.RigidBody
{
    public interface IRigidBody : ICollisionObject
    {
        Vector3 CenterOfMassOffset { get; set; }

        Aabb Aabb { get; }

        Vector3 AngularFactor { get; set; }
        Vector3 LinearFactor { get; set; }

        float Mass { get; set; }
        float InvMass { get; set; }

        Vector3 Gravity { get; set; }

        Vector3 InvInertiaDiagLocal { get; set; }
        Vector3 LocalInertia { get; }

        void ApplyImpulse(Vector3 impulse, Vector3 relPos);
        void ApplyCentralImpulse(Vector3 impulse);
        void ApplyTorqueImpulse(Vector3 torque);
        void ClearForces();
        Vector3 GetVelocityInLocalPoint(Vector3 relativePosition);
        float ComputeImpulseDenominator(Vector3 position, Vector3 normal);
    }
}