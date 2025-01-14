using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Math.Exports;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.RigidBody
{
    [ScriptExportClass("RigidBody", "Represents a RigidBody Component.")]
    public class RigidBodyExport : ExportableManagedObject<IRigidBody>, IComponentExport
    {
        public bool Active
        {
            get => Reference.Active;
            set => Reference.Active = value;
        }

        public bool ActiveInGraph => Reference.ActiveInGraph();

        public string Name
        {
            get => Reference.Name;
            set => Reference.Name = value;
        }

//        public string Id => Reference.Resource.Id; // ToDo: Uncomment this once collider resource is implemented.
        public string Id => null;

        public object Parent => Reference.Parent != null ? new GameObjectExport(Reference.Parent) : null;

        public override int Type => (int) ManagedExportType.RigidBody;

        [ScriptExportProperty("The angular damping of the `RigidBody`.")]
        public float AngularDamping
        {
            get => Reference.AngularDamping;
            set => Reference.AngularDamping = value;
        }

        [ScriptExportProperty("The angular factor of the `RigidBody`.")]
        public Vector3Export AngularFactor
        {
            get => Reference.AngularFactor;
            set => Reference.AngularFactor = value;
        }

        [ScriptExportProperty("The angular velocity of the `RigidBody`.")]
        public Vector3Export AngularVelocity
        {
            get => Reference.AngularVelocity;
            set => Reference.AngularVelocity = value;
        }

        [ScriptExportProperty("The gravity force of the `RigidBody`.")]
        public Vector3Export Gravity
        {
            get => Reference.Gravity;
            set => Reference.Gravity = value;
        }

        [ScriptExportProperty("The linear damping of the `RigidBody`.")]
        public float LinearDamping
        {
            get => Reference.LinearDamping;
            set => Reference.LinearDamping = value;
        }

        [ScriptExportProperty("The linear factor of the `RigidBody`.")]
        public Vector3Export LinearFactor
        {
            get => Reference.LinearFactor;
            set => Reference.LinearFactor = value;
        }

        [ScriptExportProperty("The linear velocity of the `RigidBody`.")]
        public Vector3Export LinearVelocity
        {
            get => Reference.LinearVelocity;
            set => Reference.LinearVelocity = value;
        }

        [ScriptExportProperty("The local inertia of the `RigidBody`.")]
        public Vector3Export LocalInertia => Reference.LocalInertia;

        [ScriptExportProperty("The mass of the `RigidBody`.")]
        public float Mass
        {
            get => Reference.Mass;
            set => Reference.Mass = value;
        }

        [ScriptExportProperty("Whether the `RigidBody` is a trigger." +
            "\nTrigger rigidbodies detect collisions but don't react to them.")]
        public bool IsTrigger
        {
            get => Reference.IsTrigger;
            set => Reference.IsTrigger = value;
        }

        [ScriptExportProperty("Whether the `RigidBody` is static." +
            "\nStatic rigidbodies can't move but still react to collisions." +
            "\nStatic rigidbodies can be concave.")]
        public bool IsStatic
        {
            get => Reference.IsStatic;
            set => Reference.IsStatic = value;
        }

        [ScriptExportProperty("Whether the `RigidBody` is kinematic." +
            "\nThe movement of Kinematic rigidbodies is controlled by code (forces are not applied)." +
            "\nKinematic rigidbodies detect collisions but don't react to them.")]
        public bool IsKinematic
        {
            get => Reference.IsKinematic;
            set => Reference.IsKinematic = value;
        }

        [ScriptExportProperty("An `EventHandler` that fires when the `RigidBody` starts colliding with another physics object.")]
        public EventExport<CollisionStartedDelegateExport> OnStartedColliding { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the `RigidBody` stops colliding with another physics object.")]
        public EventExport<CollisionStartedDelegateExport> OnStoppedColliding { get; }

        [ScriptExportMethod("Applies an impulse to the `RigidBody` from its center of mass.")]
        public void ApplyCentralImpulse(
            [ScriptExportParameter("The impulse to apply to the `RigidBody`.")] Vector3Export impulse
        )
        {
            Reference.ApplyCentralImpulse(impulse);
        }

        [ScriptExportMethod("Applies an impulse to the `RigidBody` from the specified relative position.")]
        public void ApplyImpulse(
            [ScriptExportParameter("The impulse to apply to the `RigidBody`.")] Vector3Export impulse,
            [ScriptExportParameter("The position to apply the impulse from.")] Vector3Export relPos
        )
        {
            Reference.ApplyImpulse(impulse, relPos);
        }

        [ScriptExportMethod("Applies a torque impulse to the `RigidBody`.")]
        public void ApplyTorqueImpulse(
            [ScriptExportParameter("The torque to apply to the `RigidBody`.")] Vector3Export torque
        )
        {
            Reference.ApplyTorqueImpulse(torque);
        }

        [ScriptExportMethod("Returns the aabb bounds of the `RigidBody`.")]
        [return: ScriptExportReturn("The aabb bounds of the `RigidBody`.")]
        public AabbExport GetAabb()
        {
            return new AabbExport(Reference.Aabb);
        }

        [ExportCastConstructor]
        internal RigidBodyExport(ByReference<IRigidBody> referencePointer) : base(referencePointer)
        {
            OnStartedColliding = new EventExport<CollisionStartedDelegateExport>();
            Reference.OnStartedColliding += data => OnStartedColliding.Fire();
        }

        [ExportCastConstructor]
        internal RigidBodyExport(IRigidBody value) : base(value)
        {
            OnStoppedColliding = new EventExport<CollisionStartedDelegateExport>();
            Reference.OnStoppedColliding += data => OnStoppedColliding.Fire();
        }
    }
}