using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Math;

namespace DecayEngine.StubEngines.Physics.Component.RigidBody
{
    public class StubRigidBodyComponent : StubCollisionObject, IRigidBody
    {
        public Vector3 CenterOfMassOffset
        {
            get => Vector3.Zero;
            set {}
        }

        public Aabb Aabb => new Aabb();

        public Vector3 AngularFactor
        {
            get => Vector3.Zero;
            set {}
        }

        public Vector3 LinearFactor
        {
            get => Vector3.Zero;
            set {}
        }

        public float Mass
        {
            get => 0f;
            set {}
        }

        public float InvMass
        {
            get => 0f;
            set {}
        }

        public Vector3 Gravity
        {
            get => Vector3.Zero;
            set {}
        }

        public Vector3 InvInertiaDiagLocal
        {
            get => Vector3.Zero;
            set {}
        }

        public Vector3 LocalInertia => Vector3.Zero;

        public void ApplyImpulse(Vector3 impulse, Vector3 relPos)
        {
        }

        public void ApplyCentralImpulse(Vector3 impulse)
        {
        }

        public void ApplyTorqueImpulse(Vector3 torque)
        {
        }

        public void ClearForces()
        {
        }

        public Vector3 GetVelocityInLocalPoint(Vector3 relativePosition)
        {
            return Vector3.Zero;
        }

        public float ComputeImpulseDenominator(Vector3 position, Vector3 normal)
        {
            return 1f;
        }
    }
}