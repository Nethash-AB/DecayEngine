using System;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Compound;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Compound
{
    public class CollisionShapeChild : NativeObject, ICollisionShapeChild
    {
        public ICollisionShape Shape
        {
            get
            {
                if (!IsNativeHandleInitialized) return null;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    IntPtr shapeHandle = btCompoundShapeChild_getChildShape(NativeHandle);
                    return CollisionShape.FromNativeHandle(shapeHandle);
                });
            }
        }

        public Vector3 PositionOffset
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCompoundShapeChild_getTransform(NativeHandle, out Matrix4 val);
                    return val.ExtractTranslation();
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCompoundShapeChild_getTransform(NativeHandle, out Matrix4 val);
                    val.Origin = value;
                    btCompoundShapeChild_setTransform(NativeHandle, ref val);

                    ICollisionShapeCompound parentShape = Shape?.Parent;
                    switch (parentShape)
                    {
                        case null:
                            return;
                        case CollisionShapeCompound compoundShape:
                            compoundShape.UpdateChildNode(this, val);
                            break;
                    }
                    parentShape.RecalculateLocalAabb();
                });
            }
        }

        public Quaternion RotationOffset
        {
            get
            {
                if (!IsNativeHandleInitialized) return Quaternion.Identity;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCompoundShapeChild_getTransform(NativeHandle, out Matrix4 val);
                    return val.ExtractRotation();
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCompoundShapeChild_getTransform(NativeHandle, out Matrix4 val);
                    val.Basis = Matrix4.CreateFromQuaternion(value).Basis;
                    btCompoundShapeChild_setTransform(NativeHandle, ref val);

                    ICollisionShapeCompound parentShape = Shape?.Parent;
                    switch (parentShape)
                    {
                        case null:
                            return;
                        case CollisionShapeCompound compoundShape:
                            compoundShape.UpdateChildNode(this, val);
                            break;
                    }
                    parentShape.RecalculateLocalAabb();
                });
            }
        }

        public static CollisionShapeChild FromNativeHandle(IntPtr nativeHandle)
        {
            if (nativeHandle == IntPtr.Zero) return null;
            return NativeHelper.GetManagedObjectFromUserHandle<CollisionShapeChild>(nativeHandle) ?? new CollisionShapeChild(nativeHandle);
        }

        public CollisionShapeChild(IntPtr childShapeHandle)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = childShapeHandle;
            });
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCompoundShapeChild_delete(NativeHandle);
            });
        }
    }
}