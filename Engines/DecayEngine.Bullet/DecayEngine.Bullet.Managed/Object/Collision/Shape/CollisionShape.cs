using System;
using System.Linq;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Math;
using DecayEngine.ModuleSDK.Object.Collision.Shape;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Compound;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape
{
    public abstract class CollisionShape : NativeObject, ICollisionShape
    {
        private ICollisionShapeCompound _parent;

        public ICollisionShapeCompound Parent
        {
            get => _parent;
            internal set => _parent = value;
        }
        public ByReference<ICollisionShapeCompound> ParentByRef => () => ref _parent;

        public Aabb Aabb
        {
            get
            {
                if (!IsNativeHandleInitialized) return new Aabb();

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    Matrix4 identity = Matrix4.Identity;
                    btCollisionShape_getAabb(NativeHandle, ref identity, out Vector3 aabbMin, out Vector3 aabbMax);
                    return new Aabb(aabbMin, aabbMax);
                });
            }
        }

        public Vector3 LocalScale
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.One;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCollisionShape_getLocalScaling(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionShape_setLocalScaling(NativeHandle, ref value));
            }
        }

        public float Margin
        {
            get => !IsNativeHandleInitialized ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionShape_getMargin(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCollisionShape_setMargin(NativeHandle, value));
            }
        }

        public static CollisionShape FromNativeHandle(IntPtr nativeHandle)
        {
            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => NativeHelper.GetManagedObjectFromUserHandle<CollisionShape>(nativeHandle));
        }

        public override void Destroy()
        {
            if (Destroyed) return;

            SetParent(null);

            base.Destroy();
        }

        public void SetParent(ICollisionShapeCompound parent)
        {
            Vector3 positionOffset = Vector3.Zero;
            Quaternion rotationOffset = Quaternion.Identity;

            if (_parent != null)
            {
                ICollisionShapeChild childShape = _parent.Children.FirstOrDefault(child => child?.Shape == this);
                if (childShape != null)
                {
                    positionOffset = childShape.PositionOffset;
                    rotationOffset = childShape.RotationOffset;
                }

                _parent.RemoveChild(this);
            }

            parent?.AddChild(this, positionOffset, rotationOffset);
        }

        public Vector3 CalculateLocalInertia(float mass)
        {
            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCollisionShape_calculateLocalInertia(NativeHandle, mass, out Vector3 inertia);
                return inertia;
            });
        }

        public virtual void RecalculateLocalAabb()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                Parent?.RecalculateLocalAabb();
            });
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                IntPtr userPtr = btCollisionShape_getUserPointer(NativeHandle);
                NativeHelper.FreeNativeHandle(userPtr);
            });
        }
    }
}