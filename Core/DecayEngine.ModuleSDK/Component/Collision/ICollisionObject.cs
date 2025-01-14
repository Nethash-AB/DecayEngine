using System;
using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.Filter;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using DecayEngine.ModuleSDK.Object.Collision.Shape;

namespace DecayEngine.ModuleSDK.Component.Collision
{
    public interface ICollisionObject : IPhysicsUpdateable, IComponent
    {
        ICollisionShape CollisionShape { get; set; }

        IEnumerable<ICollisionObject> IgnoredCollisionObjects { get; }
        ICollisionGroup CollisionGroup { get; set; }
        CollisionFilterDelegate CollisionFilter { get; set; }

        IEnumerable<IContactManifold> ContactManifolds { get; }

        float Stiffness { get; set; }

        Vector3 AnisotropicFriction { get; set; }
        float LinearFriction { get; set; }
        float SpinningFriction { get; set; }
        float RollingFriction { get; set; }
        float Restitution { get; set; }

        Vector3 AngularVelocity { get; set; }
        Vector3 LinearVelocity { get; set; }

        float AngularDamping { get; set; }
        float LinearDamping { get; set; }

        bool IsTrigger { get; set; }
        bool IsStatic { get; set; }
        bool IsKinematic { get; set; }
        bool HasContactResponse { get; set; }

        event Action<IContactManifold> OnStartedColliding;
        event Action<IContactManifold> OnStoppedColliding;
        event Action<float, ICollisionObject> OnPreUpdate;
        event Action<float, ICollisionObject> OnPostUpdate;
    }
}