using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.RayTrace;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.Bullet.Managed.Component.Wheel
{
    public class WheelSuspensionComponent : IPhysicsUpdateable, IComponent
    {
        private IGameObject _parent;
        private bool _active;

        private Vector3 _currentSourcePosition;
        private Vector3 _currentDirection;

        private Vector3 _currentImpulse;
        private Vector3 _currentImpulsePositionRelative;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => null;

        public bool Active
        {
            get => Parent != null && PhysicsWorld != null && ChassisRigidBody != null && ChassisRigidBody.Active && _active;
            set
            {
                if (!Active && value && Parent != null && PhysicsWorld != null && ChassisRigidBody != null && ChassisRigidBody.Active)
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

        public IRigidBody ChassisRigidBody { get; set; }
        public ICollisionObject GroundObject { get; private set; }

        public Vector3 ConnectionPointLocal { get; set; }
        public Transform TargetWorldTransform { get; }

        public float RestLength { get; set; }
        public float MaxTravelCompression { get; set; }
        public float MaxTravelRelaxation { get; set; }
        public float MaxForce { get; set; }
        public float Stiffness { get; set; }
        public float DampingCompression { get; set; }
        public float DampingRelaxation { get; set; }

        public float CurrentLength { get; private set; }
        public float CurrentForce { get; private set; }

        public WheelSuspensionComponent(IRigidBody chassisRigidBody, Vector3 connectionPointLocal)
        {
            TargetWorldTransform = new Transform();
            ChassisRigidBody = chassisRigidBody;
            ConnectionPointLocal = connectionPointLocal;
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

        ~WheelSuspensionComponent()
        {
            Destroy();
        }

        public void Destroy()
        {
            SetParent(null);

            Destroyed = true;
        }

        public void PhysicsPreUpdate(float deltaTime)
        {
            if (!Active) return;

            RayCast();
            ApplyImpulse(deltaTime);
        }

        public void PhysicsPostUpdate(float deltaTime)
        {
            if (!Active) return;

            DrawDebug();
        }

        private void RayCast()
        {
            Transform chassisTransform = ChassisRigidBody.Parent.Transform;

            _currentSourcePosition = ConnectionPointLocal * chassisTransform.TransformMatrix;
            _currentDirection = -chassisTransform.Up * chassisTransform.RotationMatrix;

            Vector3 raySource = _currentSourcePosition;
            Vector3 rayTarget = raySource + _currentDirection * RestLength;

            Matrix4 targetTransform = Matrix4.Identity;
            targetTransform.Right = chassisTransform.Right;
            targetTransform.Forward = chassisTransform.Forward;

            IRayTraceResult closestResult = PhysicsWorld.RayTrace(
                raySource, rayTarget, true,
                new List<ICollisionObject> {ChassisRigidBody})
                .FirstOrDefault();
            if (closestResult != null && closestResult.HitCollisionObject.HasContactResponse &&
                Vector3.Dot(closestResult.HitNormalWorld, chassisTransform.Up) > 0.1f
            )
            {
                GroundObject = closestResult.HitCollisionObject;

                CurrentLength = closestResult.HitFraction * RestLength;

                float minLength = RestLength - MaxTravelCompression;
                float maxLength = RestLength + MaxTravelRelaxation;
                if (CurrentLength > maxLength)
                {
                    CurrentLength = maxLength;
                }
                else if (CurrentLength < minLength)
                {
                    CurrentLength = minLength;
                }

                // ToDo: But why in hell does Bullet return weird normals sometimes, it looks 100% random.
//                targetTransform.Up = closestResult.HitNormalWorld.Normalized;
                targetTransform.Up = chassisTransform.Up;
                targetTransform.Origin = closestResult.HitPointWorld;
            }
            else
            {
                GroundObject = null;
                CurrentLength = RestLength;

//                targetTransform.Up = chassisTransform.Up;
                targetTransform.Up = TargetWorldTransform.Up;
                targetTransform.Origin = raySource + _currentDirection * CurrentLength;
            }

            TargetWorldTransform.TransformMatrix = targetTransform;
            _currentImpulsePositionRelative = TargetWorldTransform.Position - chassisTransform.Position;
        }

        private void ApplyImpulse(float deltaTime)
        {
            if (GroundObject == null)
            {
                CurrentForce = 0f;
                _currentImpulse = Vector3.Zero;
                return;
            }

            Vector3 surfaceNormal = TargetWorldTransform.Up;

            float denominator = Vector3.Dot(surfaceNormal, _currentDirection);
            Vector3 velocityAtContactPoint = ChassisRigidBody.GetVelocityInLocalPoint(_currentImpulsePositionRelative);
            float projectedVelocity = Vector3.Dot(surfaceNormal, velocityAtContactPoint);

            float relativeVelocity;
            float clippedInverseContactDot;
            if (denominator >= -0.1f)
            {
                relativeVelocity = 0f;
                clippedInverseContactDot = 10f; // 1f / 0.1f
            }
            else
            {
                float inverse = -1f / denominator;
                relativeVelocity = projectedVelocity * inverse;
                clippedInverseContactDot = inverse;
            }

            CurrentForce = Stiffness * (RestLength - CurrentLength) * clippedInverseContactDot;
            if (relativeVelocity < 0f)
            {
                CurrentForce -= DampingCompression * 2f * (float) Math.Sqrt(Stiffness) * relativeVelocity;
            }
            else
            {
                CurrentForce -= DampingRelaxation * 2f * (float) Math.Sqrt(Stiffness) * relativeVelocity;
            }
            CurrentForce *= ChassisRigidBody.Mass;

            if (CurrentForce < 0f)
            {
                CurrentForce = 0f;
            }

            if (CurrentForce > MaxForce)
            {
                CurrentForce = MaxForce;
            }

            _currentImpulse = surfaceNormal * CurrentForce * deltaTime;
            ChassisRigidBody.ApplyImpulse(_currentImpulse, _currentImpulsePositionRelative);
        }

        private void DrawDebug()
        {
            if (!PhysicsWorld.DrawDebug || PhysicsWorld.DebugCamera?.DebugDrawer == null) return;

            // ToDo: Renable this
//            PhysicsWorld.DebugCamera.DebugDrawer.AddCrosshair(
//                TargetWorldTransform.Position,
//                TargetWorldTransform.Rotation,
//                Vector3.One * 0.125f
//            );

            if (GroundObject == null) return;

            PhysicsWorld.DebugCamera.DebugDrawer.AddLine(
                ChassisRigidBody.Parent.Transform.Position + _currentImpulsePositionRelative,
                ChassisRigidBody.Parent.Transform.Position + _currentImpulsePositionRelative + _currentImpulse / 100f,
                new Vector4(0f, 0f, 1f, 1f)
            );
        }
    }
}