using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.Filter;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using DecayEngine.ModuleSDK.Object.Collision.Shape;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.GameObject;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Component.Collision
{
    public abstract class CollisionObject : NativeObject, ICollisionObject
    {
        private IGameObject _parent;

        private IPhysicsWorld _physicsWorld;
        private IntPtr _broadphaseProxy;

        private ICollisionGroup _collisionGroup;

        private readonly List<IContactManifold> _contactManifolds;

        private CollisionFlags CollisionFlags
        {
            get => !Active ?
                CollisionFlags.None :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getCollisionFlags(NativeHandle));
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_setCollisionFlags(NativeHandle, value));
            }
        }

        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public virtual Type ExportType => null; // ToDo: Implement CollisionObjectExport.

        public bool Active
        {
            get => Parent != null && IsNativeHandleInitialized && PhysicsWorld != null;
            set
            {
                if (!Active && value)
                {
                    Load();
                }
                else if (Active && !value)
                {
                    Unload();
                }
            }
        }

        public IPhysicsWorld PhysicsWorld
        {
            get => _physicsWorld;
            set
            {
                if (!IsNativeHandleInitialized) return;

                _physicsWorld = value;

                if (value != null)
                {
                    _broadphaseProxy = btCollisionObject_getBroadphaseHandle(NativeHandle);
                    btBroadphaseProxy_setClientObject(_broadphaseProxy, NativeHandle);
                }
                else if (_broadphaseProxy != IntPtr.Zero)
                {
                    btBroadphaseProxy_setClientObject(_broadphaseProxy, IntPtr.Zero);
                    _broadphaseProxy = IntPtr.Zero;
                }
            }
        }

        public abstract ICollisionShape CollisionShape { get; set; }

        public IEnumerable<ICollisionObject> IgnoredCollisionObjects
        {
            get
            {
                if (PhysicsWorld == null) return new List<ICollisionObject>();
                return PhysicsWorld.IgnoredCollisionObjectPairs
                    .Where(pair => pair.CollisionObjectA == this || pair.CollisionObjectB == this)
                    .Select(pair => pair.CollisionObjectA == this ? pair.CollisionObjectB : pair.CollisionObjectA);
            }
        }

        public ICollisionGroup CollisionGroup
        {
            get
            {
                if (PhysicsWorld == null) return null;
                return _collisionGroup ?? PhysicsWorld.CollisionGroups.FirstOrDefault();
            }
            set
            {
                if (PhysicsWorld == null || !PhysicsWorld.CollisionGroups.Contains(value)) return;
                _collisionGroup = value;
            }
        }

        public CollisionFilterDelegate CollisionFilter { get; set; }

        public IEnumerable<IContactManifold> ContactManifolds => _contactManifolds;

        public float Stiffness
        {
            get => !Active ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getContactStiffness(NativeHandle));
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCollisionObject_setContactStiffnessAndDamping(NativeHandle, value, LinearDamping);
                });
            }
        }

        public Vector3 AnisotropicFriction
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCollisionObject_getAnisotropicFriction(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCollisionObject_setAnisotropicFriction(NativeHandle, ref value, AnisotropicFrictionFlags.Friction);
                });
            }
        }

        public float LinearFriction
        {
            get => !Active ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getFriction(NativeHandle));
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_setFriction(NativeHandle, value));
            }
        }

        public float SpinningFriction
        {
            get => !Active ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getSpinningFriction(NativeHandle));
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_setSpinningFriction(NativeHandle, value));
            }
        }

        public float RollingFriction
        {
            get => !Active ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getRollingFriction(NativeHandle));
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_setRollingFriction(NativeHandle, value));
            }
        }

        public float Restitution
        {
            get => !Active ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getRestitution(NativeHandle));
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_setRestitution(NativeHandle, value));
            }
        }

        public abstract Vector3 AngularVelocity { get; set; }
        public abstract Vector3 LinearVelocity { get; set; }

        public abstract float AngularDamping { get; set; }

        public float LinearDamping
        {
            get => !Active ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_getContactDamping(NativeHandle));
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCollisionObject_setContactStiffnessAndDamping(NativeHandle, Stiffness, value);
                });
            }
        }

        public bool IsTrigger
        {
            get => Active && CollisionFlags.HasFlag(CollisionFlags.NoContactResponse);
            set
            {
                if (!IsTrigger && value)
                {
                    CollisionFlags |= CollisionFlags.NoContactResponse;
                }
                else if(IsTrigger && !value)
                {
                    CollisionFlags &= ~CollisionFlags.NoContactResponse;
                }
            }
        }

        public bool IsStatic
        {
            get => !Active || Active && CollisionFlags.HasFlag(CollisionFlags.StaticObject);
            set
            {
                if (!IsStatic && value)
                {
                    CollisionFlags |= CollisionFlags.StaticObject;
                }
                else if(IsStatic && !value)
                {
                    CollisionFlags &= ~CollisionFlags.StaticObject;
                }
            }
        }

        public bool IsKinematic
        {
            get => Active && CollisionFlags.HasFlag(CollisionFlags.KinematicObject);
            set
            {
                if (!IsKinematic && value)
                {
                    CollisionFlags |= CollisionFlags.KinematicObject;
                }
                else if(IsKinematic && !value)
                {
                    CollisionFlags &= ~CollisionFlags.KinematicObject;
                }
            }
        }

        public bool HasContactResponse
        {
            get => Active && !CollisionFlags.HasFlag(CollisionFlags.NoContactResponse);
            set
            {
                if (!HasContactResponse && value)
                {
                    CollisionFlags &= CollisionFlags.NoContactResponse;
                }
                else if (HasContactResponse && !value)
                {
                    CollisionFlags |= CollisionFlags.NoContactResponse;
                }
            }
        }

        public event Action<IContactManifold> OnStartedColliding;
        public event Action<IContactManifold> OnStoppedColliding;
        public event Action<float, ICollisionObject> OnPreUpdate;
        public event Action<float, ICollisionObject> OnPostUpdate;

        public static CollisionObject FromNativeHandle(IntPtr nativeHandle)
        {
            return nativeHandle == IntPtr.Zero
                ? null
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => NativeHelper.GetManagedObjectFromUserHandle<CollisionObject>(nativeHandle));
        }

        public static CollisionObject FromNativeBroadphaseHandle(IntPtr nativeHandle)
        {
            if (nativeHandle == IntPtr.Zero) return null;

            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                IntPtr clientObjectHandle = btBroadphaseProxy_getClientObject(nativeHandle);
                return FromNativeHandle(clientObjectHandle);
            });
        }

        protected CollisionObject()
        {
            _contactManifolds = new List<IContactManifold>();
        }

        public override void Destroy()
        {
            if (Destroyed) return;

            Unload();

            SetParent(null);

            _contactManifolds.Clear();

            base.Destroy();
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

        internal void AddContactManifold(IContactManifold contactManifold)
        {
            if (_contactManifolds.Contains(contactManifold)) return;
            _contactManifolds.Add(contactManifold);
            OnStartedColliding?.Invoke(contactManifold);
        }

        internal void RemoveContactManifold(IContactManifold contactManifold)
        {
            if (!_contactManifolds.Contains(contactManifold)) return;
            _contactManifolds.Remove(contactManifold);
            OnStoppedColliding?.Invoke(contactManifold);
        }

        protected Matrix4 GetInterpolatedWorldTransform()
        {
            if (!IsNativeHandleInitialized) return Matrix4.Identity;

            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCollisionObject_getInterpolationWorldTransform(NativeHandle, out Matrix4 transform);
                return transform;
            });
        }

        protected void SetInterpolatedWorldTransform(Matrix4 transform)
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionObject_setInterpolationWorldTransform(NativeHandle, ref transform));
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                IntPtr userPtr = btCollisionObject_getUserPointer(NativeHandle);
                NativeHelper.FreeNativeHandle(userPtr);
            });
        }

        protected abstract void Load();
        protected abstract void Unload();
    }
}