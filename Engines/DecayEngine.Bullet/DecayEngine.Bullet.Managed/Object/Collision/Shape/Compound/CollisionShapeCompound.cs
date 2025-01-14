using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Compound;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Compound
{
    public class CollisionShapeCompound : CollisionShape, ICollisionShapeCompound
    {
        private NativeReadOnlyList<CollisionShapeChild> _children;

        public int ChildrenCount => _children.Count;
        public IEnumerable<ICollisionShapeChild> Children => _children;

        public CollisionShapeCompound()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btCompoundShape_new(true, 0);
                CreateNativeChildArray();
            });
        }

        public override void Destroy()
        {
            if (Destroyed) return;

            List<ICollisionShapeChild> childrenList = Children.ToList();
            foreach (ICollisionShapeChild child in childrenList)
            {
                child?.Shape?.Destroy();
            }

            RemoveAllChildren();

            base.Destroy();
        }

        public void AddChild(ICollisionShape child, Vector3 positionOffset, Quaternion rotationOffset)
        {
            if (!(child is CollisionShape collisionShape)) return;
            if (!IsNativeHandleInitialized || !collisionShape.IsNativeHandleInitialized || ContainsShape(child)) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                Matrix4 childTransformMatrix = Matrix4.Identity;
                childTransformMatrix.Basis = Matrix4.CreateFromQuaternion(rotationOffset).Basis;
                childTransformMatrix.Origin = positionOffset;
                btCompoundShape_addChildShape(NativeHandle, ref childTransformMatrix, collisionShape.NativeHandle);
                collisionShape.Parent = this;
                RecalculateLocalAabb();
            });
        }

        public void RemoveChild(ICollisionShape child)
        {
            if (!(child is CollisionShape collisionShape)) return;
            if (!IsNativeHandleInitialized || !collisionShape.IsNativeHandleInitialized || !ContainsShape(child)) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCompoundShape_removeChildShape(NativeHandle, collisionShape.NativeHandle);
                collisionShape.Parent = null;
                RecalculateLocalAabb();
            });
        }

        public void RemoveAllChildren()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                int childCount = ChildrenCount;
                for (int i = childCount - 1; i >= 0; i++)
                {
                    CollisionShapeChild childShape = _children[i];
                    if (childShape.Shape is CollisionShape collisionShape && collisionShape.Parent != null)
                    {
                        collisionShape.Parent = null;
                    }

                    btCompoundShape_removeChildShapeByIndex(NativeHandle, i);
                }

                RecalculateLocalAabb();
            });
        }

        public override void RecalculateLocalAabb()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (IsNativeHandleInitialized)
                {
                    btCompoundShape_recalculateLocalAabb(NativeHandle);
                }
                base.RecalculateLocalAabb();
            });
        }

        internal void UpdateChildNode(CollisionShapeChild childShape, Matrix4 newTransform)
        {
            if (!(childShape.Shape is CollisionShape collisionShape)) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCollisionShape_getAabb(collisionShape.NativeHandle, ref newTransform, out Vector3 aabbMin, out Vector3 aabbMax);

                IntPtr dynamicTreeHandle = btCompoundShape_getDynamicAabbTree(NativeHandle);
                if (dynamicTreeHandle == IntPtr.Zero) return;

                IntPtr boundsHandle = btDbvtAabbMm_FromMM(ref aabbMin, ref aabbMax);
                if (boundsHandle == IntPtr.Zero) return;

                IntPtr childNodeHandle = btCompoundShapeChild_getNode(childShape.NativeHandle);
                if (childNodeHandle == IntPtr.Zero) return;

                btDbvt_update(dynamicTreeHandle, childNodeHandle, boundsHandle);

                btDbvtAabbMm_delete(boundsHandle);
            });
        }

        private bool ContainsShape(ICollisionShape shape)
        {
            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                return Children.Any(child => child?.Shape == shape);
            });
        }

        private void CreateNativeChildArray()
        {
            _children = new NativeReadOnlyList<CollisionShapeChild>(
                index =>
                {
                    if (!IsNativeHandleInitialized) return null;

                    IntPtr childArrayHandle = btCompoundShape_getChildList(NativeHandle);
                    IntPtr childShapeHandle = btCompoundShapeChild_array_at(childArrayHandle, index);
                    return CollisionShapeChild.FromNativeHandle(childShapeHandle);
                },
                () => !IsNativeHandleInitialized ? 0 : btCompoundShape_getNumChildShapes(NativeHandle)
            );
        }
    }
}