using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.Filter;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using DecayEngine.ModuleSDK.Object.Collision.Shape;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.GameObject;

#pragma warning disable 67

namespace DecayEngine.StubEngines.Physics.Component.RigidBody
{
    public abstract class StubCollisionObject : ICollisionObject
    {
        private IGameObject _parent;
        private bool _active;
        private ICollisionGroup _collisionGroup;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public virtual Type ExportType => null; // ToDo: Implement CollisionObjectExport.

        public bool Active
        {
            get => Parent != null && _active && PhysicsWorld != null;
            set
            {
                if (!Active && value)
                {
                    _active = true;
                }
                else if (Active && !value)
                {
                    _active = false;
                }
            }
        }

        public IPhysicsWorld PhysicsWorld { get; set; }

        public ICollisionShape CollisionShape { get; set; }
        public IEnumerable<ICollisionObject> IgnoredCollisionObjects
        {
            get
            {
                if (PhysicsWorld == null) return new List<ICollisionObject>();
                return PhysicsWorld.IgnoredCollisionObjectPairs
                    .Where(pair => pair.CollisionObjectA == this || pair.CollisionObjectB == this)
                    .Select(pair => pair.CollisionObjectA == this ? pair.CollisionObjectA : pair.CollisionObjectB);
            }
        }

        public ICollisionGroup CollisionGroup
        {
            get => _collisionGroup;
            set
            {
                if (PhysicsWorld == null || !PhysicsWorld.CollisionGroups.Contains(value)) return;
                _collisionGroup = value;
            }
        }

        public CollisionFilterDelegate CollisionFilter { get; set; }

        public IEnumerable<IContactManifold> ContactManifolds => new List<IContactManifold>();

        public float Stiffness
        {
            get => 0f;
            set {}
        }

        public Vector3 AnisotropicFriction
        {
            get => Vector3.Zero;
            set {}
        }

        public float LinearFriction
        {
            get => 0f;
            set {}
        }

        public float SpinningFriction
        {
            get => 0f;
            set {}
        }

        public float RollingFriction
        {
            get => 0f;
            set {}
        }

        public float Restitution
        {
            get => 0f;
            set {}
        }

        public Vector3 AngularVelocity
        {
            get => Vector3.Zero;
            set {}
        }

        public Vector3 LinearVelocity
        {
            get => Vector3.Zero;
            set {}
        }

        public float AngularDamping { get; set; }

        public float LinearDamping
        {
            get => 0f;
            set {}
        }

        public bool IsTrigger
        {
            get => false;
            set {}
        }

        public bool IsStatic
        {
            get => true;
            set {}
        }

        public bool IsKinematic
        {
            get => false;
            set {}
        }

        public bool HasContactResponse
        {
            get => false;
            set {}
        }

        public event Action<IContactManifold> OnStartedColliding;
        public event Action<IContactManifold> OnStoppedColliding;

        public event Action<float, ICollisionObject> OnPreUpdate;
        public event Action<float, ICollisionObject> OnPostUpdate;

        public void Destroy()
        {
            if (Destroyed) return;

            SetParent(null);

            Destroyed = true;
        }

        public void SetParent(IGameObject parent)
        {
            _parent?.RemoveComponent(this);

            parent?.AttachComponent(this);
            _parent = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        public virtual void PhysicsPreUpdate(float deltaTime)
        {
            OnPreUpdate?.Invoke(deltaTime, this);
        }

        public virtual void PhysicsPostUpdate(float deltaTime)
        {
            OnPostUpdate?.Invoke(deltaTime, this);
        }
    }
}