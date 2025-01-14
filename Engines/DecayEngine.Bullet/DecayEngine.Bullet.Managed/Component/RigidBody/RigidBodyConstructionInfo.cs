using System;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.Object.Collision.Shape;
using DecayEngine.Bullet.Managed.Object.Transform;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Component.RigidBody
{
    public class RigidBodyConstructionInfo : NativeObject
    {
        public float AdditionalAngularDampingFactor
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAdditionalAngularDampingFactor(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAdditionalAngularDampingFactor(NativeHandle, value));
            }
        }

        public float AdditionalAngularDampingThresholdSquared
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAdditionalAngularDampingThresholdSqr(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAdditionalAngularDampingThresholdSqr(NativeHandle, value));
            }
        }

        public bool AdditionalDamping
        {
            get =>
                IsNativeHandleInitialized &&
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAdditionalDamping(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAdditionalDamping(NativeHandle, value));
            }
        }

        public float AdditionalDampingFactor
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAdditionalDampingFactor(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAdditionalDampingFactor(NativeHandle, value));
            }
        }

        public float AdditionalLinearDampingThresholdSquared
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAdditionalLinearDampingThresholdSqr(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAdditionalLinearDampingThresholdSqr(NativeHandle, value));
            }
        }

        public float AngularDamping
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAngularDamping(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAngularDamping(NativeHandle, value));
            }
        }

        public float AngularSleepingThreshold
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getAngularSleepingThreshold(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setAngularSleepingThreshold(NativeHandle, value));
            }
        }

        public CollisionShape CollisionShape
        {
            get
            {
                if (!IsNativeHandleInitialized) return null;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    IntPtr shapeHandle = btRigidBody_btRigidBodyConstructionInfo_getCollisionShape(NativeHandle);
                    return CollisionShape.FromNativeHandle(shapeHandle);
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_btRigidBodyConstructionInfo_setCollisionShape(NativeHandle, value.NativeHandle);
                });
            }
        }

        public float LinearFriction
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getFriction(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setFriction(NativeHandle, value));
            }
        }

        public float LinearDamping
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getLinearDamping(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setLinearDamping(NativeHandle, value));
            }
        }

        public float LinearSleepingThreshold
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getLinearSleepingThreshold(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setLinearSleepingThreshold(NativeHandle, value));
            }
        }

        public Vector3 LocalInertia
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_btRigidBodyConstructionInfo_getLocalInertia(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_btRigidBodyConstructionInfo_setLocalInertia(NativeHandle, ref value));
            }
        }

        public float Mass
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getMass(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setMass(NativeHandle, value));
            }
        }

        public float InvMass
        {
            get
            {
                float mass = Mass;
                if (mass.IsZero())
                {
                    return 0f;
                }

                return 1f / mass;
            }
        }

        public float Restitution
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getRestitution(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setRestitution(NativeHandle, value));
            }
        }

        public float RollingFriction
        {
            get => !IsNativeHandleInitialized ?
                0f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_getRollingFriction(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    btRigidBody_btRigidBodyConstructionInfo_setRollingFriction(NativeHandle, value));
            }
        }

        public RigidBodyTransformSource MotionState { get; private set; }

        public RigidBodyConstructionInfo(RigidBodyComponent rigidBody, float mass, CollisionShape collisionShape)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                Vector3 localInertia = collisionShape.CalculateLocalInertia(mass);
                RigidBodyTransformSource motionState = new RigidBodyTransformSource(rigidBody);
                MotionState = motionState;

                NativeHandle = btRigidBody_btRigidBodyConstructionInfo_new2(mass, motionState.NativeHandle, collisionShape.NativeHandle, ref localInertia);
            });
        }

        protected override void FreeUnmanagedHandles()
        {
        }
    }
}