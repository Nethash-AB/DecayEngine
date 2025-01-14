using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.Bullet.Managed.BulletInterop.Enums;
using DecayEngine.Bullet.Managed.Component.Collision;
using DecayEngine.Bullet.Managed.Object.Collision.Shape.Compound;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Math;
using DecayEngine.ModuleSDK.Object.Collision.Shape;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Compound;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.Transform;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Component.RigidBody
{
    public class RigidBodyComponent : CollisionObject, IRigidBody
    {
        private readonly ConcurrentQueue<Action> _pendingForces;
        private RigidBodyConstructionInfo _rigidBodyConstructionInfo;
        private CollisionShapeCompound _compoundShape;

        public override ICollisionShape CollisionShape
        {
            get => GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => _compoundShape.Children.FirstOrDefault()?.Shape);
            set =>
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    List<ICollisionShapeChild> childrenShapes = _compoundShape.Children.ToList();
                    if (childrenShapes.Any(cs => cs.Shape == value)) return;

                    if (childrenShapes.Count > 0)
                    {
                        _compoundShape.RemoveAllChildren();
                    }
                    _compoundShape.AddChild(value, Vector3.Zero, Quaternion.Identity);
                });
        }

        public Vector3 CenterOfMassOffset
        {
            get => GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => _compoundShape.Children.FirstOrDefault()?.PositionOffset ?? Vector3.Zero);
            set =>
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    ICollisionShapeChild childShape = _compoundShape.Children.FirstOrDefault();
                    if (childShape == null) return;

                    childShape.PositionOffset = value.Negated;
                });
        }

        public Matrix4 CenterOfMass =>
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btRigidBody_getCenterOfMassTransform(NativeHandle, out Matrix4 comTransform);
                return comTransform;
            });

        public Aabb Aabb =>
            !Active
                ? new Aabb()
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getAabb(NativeHandle, out Vector3 min, out Vector3 max);
                    return new Aabb(min, max);
                });

        public override float AngularDamping
        {
            get => !Active ?
                _rigidBodyConstructionInfo.AngularDamping :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_getAngularDamping(NativeHandle));
            set
            {
                if (!Active)
                {
                    _rigidBodyConstructionInfo.AngularDamping = value;
                }
                else
                {
                    GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        btRigidBody_setDamping(NativeHandle, LinearDamping, value));
                }
            }
        }

        public Vector3 AngularFactor
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getAngularFactor(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_setAngularFactor(NativeHandle, ref value);
                });
            }
        }

        public Vector3 LinearFactor
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getLinearFactor(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_setLinearFactor(NativeHandle, ref value);
                });
            }
        }

        public override Vector3 AngularVelocity
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getAngularVelocity(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_setAngularVelocity(NativeHandle, ref value));
            }
        }

        public override Vector3 LinearVelocity
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getLinearVelocity(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_setLinearVelocity(NativeHandle, ref value));
            }
        }

        public float Mass
        {
            get
            {
                if (!Active) return _rigidBodyConstructionInfo.Mass;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    float invMass = btRigidBody_getInvMass(NativeHandle);
                    return !invMass.IsZero() ? 1f / invMass : 0f;
                });
            }
            set
            {
                if (!Active)
                {
                    _rigidBodyConstructionInfo.Mass = value;
                }
                else
                {
                    GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                    {
                        if (value > MathHelper.Epsilon)
                        {
                            IPhysicsWorld physicsWorld = PhysicsWorld;
                            physicsWorld?.RemoveUpdateable(this);

                            Vector3 localInertia = CollisionShape.CalculateLocalInertia(value);
                            btRigidBody_setMassProps(NativeHandle, value, ref localInertia);

                            if (IsStatic)
                            {
                                IsStatic = false;
                            }

                            physicsWorld?.AddUpdateable(this);
                        }
                        else if (value < -MathHelper.Epsilon)
                        {
                            throw new ArgumentException("Mass cannot be negative", nameof(Mass));
                        }
                        else
                        {
                            IPhysicsWorld physicsWorld = PhysicsWorld;
                            physicsWorld?.RemoveUpdateable(this);

                            Vector3 localInertia = Vector3.Zero;
                            btRigidBody_setMassProps(NativeHandle, value, ref localInertia);

                            if (!IsStatic)
                            {
                                IsStatic = true;
                            }

                            physicsWorld?.AddUpdateable(this);
                        }
                    });
                }
            }
        }

        public float InvMass
        {
            get => !Active ?
                _rigidBodyConstructionInfo.InvMass :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_getInvMass(NativeHandle));
            set
            {
                if (value > MathHelper.Epsilon)
                {
                    Mass = 1f / value;
                }
                else if (value < -MathHelper.Epsilon)
                {
                    throw new ArgumentException("Mass cannot be negative", nameof(InvMass));
                }
                else
                {
                    Mass = 0f;
                }
            }
        }

        public Vector3 Gravity
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getGravity(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_setGravity(NativeHandle, ref value);
                });
            }
        }

        public Vector3 InvInertiaDiagLocal
        {
            get
            {
                if (!Active) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getInvInertiaDiagLocal(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!Active) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_setInvInertiaDiagLocal(NativeHandle, ref value);
                });
            }
        }

        public Vector3 LocalInertia
        {
            get
            {
                if (!Active) return _rigidBodyConstructionInfo.LocalInertia;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btRigidBody_getLocalInertia(NativeHandle, out Vector3 val);
                    return val;
                });
            }
        }

        public RigidBodyComponent(float mass, ICollisionShape collisionShape)
        {
            _pendingForces = new ConcurrentQueue<Action>();

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                _compoundShape = new CollisionShapeCompound();
                _compoundShape.AddChild(collisionShape, Vector3.Zero, Quaternion.Identity);

                _rigidBodyConstructionInfo = new RigidBodyConstructionInfo(this, mass, _compoundShape);
            });
        }

        public override void PhysicsPreUpdate(float deltaTime)
        {
            base.PhysicsPreUpdate(deltaTime);

            while (!_pendingForces.IsEmpty)
            {
                if (!_pendingForces.TryDequeue(out Action physicsAction)) continue;

                physicsAction();
            }
        }

        public void ApplyImpulse(Vector3 impulse, Vector3 relPos)
        {
            if (!Active) return;

            if (PhysicsWorld.PhysicsWorldState == PhysicsWorldState.PreSubStep)
            {
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_applyImpulse(NativeHandle, ref impulse, ref relPos));
            }
            else
            {
                _pendingForces.Enqueue(() => btRigidBody_applyImpulse(NativeHandle, ref impulse, ref relPos));
            }
        }

        public void ApplyCentralImpulse(Vector3 impulse)
        {
            if (!Active) return;

            if (PhysicsWorld.PhysicsWorldState == PhysicsWorldState.PreSubStep)
            {
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_applyCentralImpulse(NativeHandle, ref impulse));
            }
            else
            {
                _pendingForces.Enqueue(() => btRigidBody_applyCentralImpulse(NativeHandle, ref impulse));
            }
        }

        public void ApplyTorqueImpulse(Vector3 torque)
        {
            if (!Active) return;

            if (PhysicsWorld.PhysicsWorldState == PhysicsWorldState.PreSubStep)
            {
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_applyTorqueImpulse(NativeHandle, ref torque));
            }
            else
            {
                _pendingForces.Enqueue(() => btRigidBody_applyTorqueImpulse(NativeHandle, ref torque));
            }
        }

        public void ClearForces()
        {
            if (!Active) return;

            if (PhysicsWorld.PhysicsWorldState == PhysicsWorldState.PreSubStep)
            {
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    LinearVelocity = Vector3.Zero;
                    AngularVelocity = Vector3.Zero;
                    btRigidBody_clearForces(NativeHandle);
                });
            }
            else
            {
                _pendingForces.Enqueue(() =>
                {
                    LinearVelocity = Vector3.Zero;
                    AngularVelocity = Vector3.Zero;
                    btRigidBody_clearForces(NativeHandle);
                });
            }
        }

        public Vector3 GetVelocityInLocalPoint(Vector3 relativePosition)
        {
            if (!Active) return Vector3.Zero;

            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btRigidBody_getVelocityInLocalPoint(NativeHandle, ref relativePosition, out Vector3 val);
                return val;
            });
        }

        public float ComputeImpulseDenominator(Vector3 position, Vector3 normal)
        {
            return !Active ?
                1f :
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btRigidBody_computeImpulseDenominator(NativeHandle, ref position, ref normal));
        }

        protected override void Load()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                _rigidBodyConstructionInfo.MotionState.CopyStateFrom(Parent.Transform.TransformSource);
                Parent.Transform.TransformSource = _rigidBodyConstructionInfo.MotionState;

                NativeHandle = btRigidBody_new(_rigidBodyConstructionInfo.NativeHandle);

                btRigidBody_setSleepingThresholds(NativeHandle, 0f, 0f);
                btCollisionObject_setActivationState(NativeHandle, ActivationState.DisableDeactivation);

                GameEngine.PhysicsEngine.PhysicsWorld.AddUpdateable(this); // ToDo: change this to owner physics world after threading refactor.
            });
        }

        protected override void Unload()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                GameEngine.PhysicsEngine.PhysicsWorld.RemoveUpdateable(this);
                btCollisionObject_delete(NativeHandle);
                NativeHandle = IntPtr.Zero;

                DefaultTransformSource newTransformSource =
                    new DefaultTransformSource();
                newTransformSource.CopyStateFrom(Parent.Transform.TransformSource);
                Parent.Transform.TransformSource = newTransformSource;
            });
        }
    }
}