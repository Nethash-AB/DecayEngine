using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.Bullet.Managed.Component.Wheel
{
    public class WheelComponent : IPhysicsUpdateable, IComponent
    {
        private IGameObject _parent;
        private bool _active;

        private Transform _steeringTransform;
        private Vector3 _currentForwardImpulse;
        private Vector3 _currentForwardImpulsePositionRelative;
        private Vector3 _currentSideImpulse;
        private Vector3 _currentSideImpulsePositionRelative;
        private Vector2 _currentSkidFactor;
        private float _currentRotation;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => null;

        public bool Active
        {
            get => Parent != null && PhysicsWorld != null && WheelSuspension != null && WheelSuspension.Active && _active;
            set
            {
                if (!Active && value && Parent != null && PhysicsWorld != null && WheelSuspension != null && WheelSuspension.Active)
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

        public WheelSuspensionComponent WheelSuspension { get; set; }

        public Transform TargetWorldTransform { get; }

        public float Radius { get; set; }
        public Vector2 FrictionSlip { get; set; }
        public float RollingFriction { get; set; }
        public float SideFrictionStiffness { get; set; }
        public float RollInfluence { get; set; }

        public float Steering { get; set; }
        public float EngineForce { get; set; }
        public float BrakeForce { get; set; }

        public bool SlidingRolling { get; private set; }
        public bool SlidingSide { get; private set; }
        public Vector2 SkidFactor => _currentSkidFactor;
        public float DeltaRotation { get; private set; }

        public WheelComponent(WheelSuspensionComponent wheelSuspension)
        {
            _steeringTransform = new Transform();
            TargetWorldTransform = new Transform();
            WheelSuspension = wheelSuspension;
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

        ~WheelComponent()
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

            UpdateTransform();
            ApplyFriction(deltaTime);
            ApplyRotation(deltaTime);
        }

        public void PhysicsPostUpdate(float deltaTime)
        {
            if (!Active) return;

            DrawDebug();
        }

        private void UpdateTransform()
        {
            _steeringTransform.Rotation =
                WheelSuspension.TargetWorldTransform.Rotation *
                Quaternion.FromAxisAngle(Vector3.Up, Steering);

            TargetWorldTransform.Position = WheelSuspension.TargetWorldTransform.Position;
            TargetWorldTransform.Rotation =
                _steeringTransform.Rotation *
                Quaternion.FromAxisAngle(Vector3.Right, _currentRotation);
//            TargetWorldTransform.Rotation =
//                WheelSuspension.TargetWorldTransform.Rotation *
//                Quaternion.FromAxisAngle(WheelSuspension.TargetWorldTransform.Up, Steering);
//            TargetWorldTransform.Rotation =
//                WheelSuspension.TargetWorldTransform.Rotation *
//                Quaternion.FromAxisAngle(WheelSuspension.TargetWorldTransform.Up, Steering) *
//                Quaternion.FromAxisAngle(WheelSuspension.TargetWorldTransform.Right, _currentRotation);
        }

        private void ApplyFriction(float deltaTime)
        {
            Transform chassisTransform = WheelSuspension.ChassisRigidBody.Parent.Transform;
            Vector3 contactPosition = WheelSuspension.TargetWorldTransform.Position;
            Vector3 positionRelative = contactPosition - chassisTransform.Position;

            if (!(WheelSuspension.GroundObject is IRigidBody groundObject))
            {
                _currentForwardImpulse = Vector3.Zero;
                _currentForwardImpulsePositionRelative = positionRelative;

                _currentSideImpulse = Vector3.Zero;
                _currentSideImpulsePositionRelative = positionRelative;

                SlidingRolling = false;
                SlidingSide = false;
                _currentSkidFactor = Vector2.One;
            }
            else
            {
                Vector3 axleDirection = _steeringTransform.Right.Normalized;
                Vector3 forwardDirection = _steeringTransform.Forward.Normalized;

                float sideImpulseLength = ResolveSingleBilateral(
                    WheelSuspension.ChassisRigidBody, contactPosition,
                    groundObject, contactPosition,
                    axleDirection
                ) * SideFrictionStiffness;

                float maxBrakingImpulseLength;
                if (BrakeForce > 0f)
                {
                    maxBrakingImpulseLength = BrakeForce;
                }
                else
                {
                    maxBrakingImpulseLength = 0f;
//                    Vector3 velocity = WheelSuspension.ChassisRigidBody.GetVelocityInLocalPoint(positionRelative);
//                    maxBrakingImpulseLength = Math.Abs(
//                        velocity.Length * groundObject.LinearFriction *
//                        WheelSuspension.ChassisRigidBody.Mass *
//                        RollingFriction *
//                        0.1f
//                    ).Fixed();
                }

                float forwardImpulseLength = CalculateRollingFriction(
                    WheelSuspension.ChassisRigidBody, groundObject,
                    contactPosition, forwardDirection, maxBrakingImpulseLength
                );
                forwardImpulseLength += EngineForce;

                forwardImpulseLength *= deltaTime;
                sideImpulseLength *= deltaTime;

                // Rolling skidding
                float maxImpulseRollingLength = WheelSuspension.CurrentForce * deltaTime * FrictionSlip.Y;
                float forwardImpulseLengthAbs = Math.Abs(forwardImpulseLength);
                if (forwardImpulseLengthAbs > maxImpulseRollingLength)
                {
                    SlidingRolling = true;
                    _currentSkidFactor.Y = maxImpulseRollingLength / forwardImpulseLengthAbs;
                    if (_currentSkidFactor.Y < 0.25f) _currentSkidFactor.Y = 0.25f;
                    if (!forwardImpulseLength.IsZero())
                    {
                        forwardImpulseLength *= _currentSkidFactor.Y;
                    }
                }
                else
                {
                    SlidingRolling = false;
                    _currentSkidFactor.Y = 1f;
                }

                // Side skidding
                float maxImpulseSideLength = WheelSuspension.CurrentForce * deltaTime * FrictionSlip.X;
                float sideImpulseLengthAbs = Math.Abs(sideImpulseLength * 2.25f);
                if (sideImpulseLengthAbs > maxImpulseSideLength)
                {
                    SlidingSide = true;
                    _currentSkidFactor.X = maxImpulseSideLength / sideImpulseLengthAbs;
                    if (_currentSkidFactor.X < 0.25f) _currentSkidFactor.X = 0.25f;
                    if (!sideImpulseLength.IsZero())
                    {
                        sideImpulseLength *= _currentSkidFactor.X;
                    }
                }
                else
                {
                    SlidingSide = false;
                    _currentSkidFactor.X = 1f;
                }

                if (!forwardImpulseLength.IsZero())
                {
                    _currentForwardImpulse = forwardDirection * forwardImpulseLength;
                    _currentForwardImpulsePositionRelative = positionRelative;
                    WheelSuspension.ChassisRigidBody.ApplyImpulse(_currentForwardImpulse, _currentForwardImpulsePositionRelative);
                }

                if (!sideImpulseLength.IsZero())
                {
                    Vector3 chasisUp = chassisTransform.Up;

                    _currentSideImpulse = axleDirection * sideImpulseLength;
                    float dot = Vector3.Dot(chasisUp, positionRelative);
                    _currentSideImpulsePositionRelative = positionRelative - chasisUp * (dot * (1f - RollInfluence));

                    WheelSuspension.ChassisRigidBody.ApplyImpulse(_currentSideImpulse, _currentSideImpulsePositionRelative);

                    if (!groundObject.IsStatic && !groundObject.IsKinematic)
                    {
                        Vector3 positionRelativeGround = contactPosition - groundObject.Parent.Transform.Position;
                        groundObject.ApplyImpulse(-_currentSideImpulse, positionRelativeGround);
                    }
                }
            }
        }

        private void ApplyRotation(float deltaTime)
        {
            if (WheelSuspension.GroundObject == null)
            {
                _currentRotation += DeltaRotation;
            }
            else
            {
                Transform chassisTransform = WheelSuspension.ChassisRigidBody.Parent.Transform;
                Vector3 positionRelative = WheelSuspension.ConnectionPointLocal * chassisTransform.TransformMatrix - chassisTransform.Position;
                Vector3 velocityAtContactPoint = WheelSuspension.ChassisRigidBody.GetVelocityInLocalPoint(positionRelative);

                Vector3 chassisForward = chassisTransform.Forward;
                Vector3 surfaceNormal = WheelSuspension.TargetWorldTransform.Up;

                float projection = Vector3.Dot(chassisForward, surfaceNormal);
                chassisForward -= surfaceNormal * projection;
                projection = Vector3.Dot(chassisForward, velocityAtContactPoint);

                DeltaRotation = projection * deltaTime / Radius;
                _currentRotation += DeltaRotation;
            }

            DeltaRotation *= 0.99f;
        }

        private static float ResolveSingleBilateral(IRigidBody rigidBody1, Vector3 position1, IRigidBody rigidBody2, Vector3 position2, Vector3 normal)
        {
            float normalLengthSqr = normal.LengthSquared;
            if (normalLengthSqr > 1.1f)
            {
                return 0f;
            }

            Vector3 relativePosition1 = position1 - rigidBody1.Parent.Transform.Position;
            Vector3 relativePosition2 = position2 - rigidBody2.Parent.Transform.Position;

            Vector3 velocity1 = rigidBody1.GetVelocityInLocalPoint(relativePosition1);
            Vector3 velocity2 = rigidBody2.GetVelocityInLocalPoint(relativePosition2);
            Vector3 velocity = velocity1 - velocity2;

            Matrix4 worldMatrix1 = Matrix4.Transpose(rigidBody1.Parent.Transform.RotationMatrix);
            Matrix4 worldMatrix2 = Matrix4.Transpose(rigidBody2.Parent.Transform.RotationMatrix);
            Vector3 j1 = Vector3.Cross(relativePosition1, normal) * worldMatrix1;
            Vector3 j2 = Vector3.Cross(relativePosition2, -normal) * worldMatrix2;
            Vector3 minvJt1 = rigidBody1.InvInertiaDiagLocal * j1;
            Vector3 minvJt2 = rigidBody2.InvInertiaDiagLocal * j2;
            float dot1 = Vector3.Dot(minvJt1, j1);
            float dot2 = Vector3.Dot(minvJt2, j2);
            float jacDiagAb = rigidBody1.InvMass + dot1 + rigidBody2.InvMass + dot2;
            float jacDiagAbInverse = 1f / jacDiagAb;

            float relativeVelocity = Vector3.Dot(normal, velocity);

            const float contactDamping = 0.2f;
            return -contactDamping * relativeVelocity * jacDiagAbInverse;
        }

        private static float CalculateRollingFriction(IRigidBody rigidBody1, IRigidBody rigidBody2,
            Vector3 contactPosition, Vector3 frictionDirection, float maxImpulse)
        {
            float denominator1 = rigidBody1.ComputeImpulseDenominator(contactPosition, frictionDirection);
            float denominator2 = rigidBody2.ComputeImpulseDenominator(contactPosition, frictionDirection);

            float jacDiagAbInv = 1f / (denominator1 + denominator2);

            Vector3 relativePosition1 = contactPosition - rigidBody1.Parent.Transform.Position;
            Vector3 relativePosition2 = contactPosition - rigidBody2.Parent.Transform.Position;

            Vector3 velocity1 = rigidBody1.GetVelocityInLocalPoint(relativePosition1);
            Vector3 velocity2 = rigidBody2.GetVelocityInLocalPoint(relativePosition2);
            Vector3 velocity = velocity1 - velocity2;

            float velocityRelative = Vector3.Dot(frictionDirection, velocity);

            float j = -velocityRelative * jacDiagAbInv;
            j = Math.Min(j, maxImpulse);
            j = Math.Max(j, -maxImpulse);

            return j;
        }

        private void DrawDebug()
        {
            if (!PhysicsWorld.DrawDebug || PhysicsWorld.DebugCamera?.DebugDrawer == null) return;

            PhysicsWorld.DebugCamera.DebugDrawer.AddLine(
                WheelSuspension.ChassisRigidBody.Parent.Transform.Position + _currentForwardImpulsePositionRelative,
                WheelSuspension.ChassisRigidBody.Parent.Transform.Position + _currentForwardImpulsePositionRelative + _currentForwardImpulse * 0.05f,
                new Vector4(1f, 0f, 1f, 1f)
            );

            PhysicsWorld.DebugCamera.DebugDrawer.AddLine(
                WheelSuspension.ChassisRigidBody.Parent.Transform.Position + _currentSideImpulsePositionRelative,
                WheelSuspension.ChassisRigidBody.Parent.Transform.Position + _currentSideImpulsePositionRelative + _currentSideImpulse * 0.05f,
                new Vector4(1f, 0f, 1f, 1f)
            );

            PhysicsWorld.DebugCamera.DebugDrawer.AddLine(
                TargetWorldTransform.Position,
                TargetWorldTransform.Position + TargetWorldTransform.Right,
                new Vector4(1f, 0f, 1f, 1f)
            );

            PhysicsWorld.DebugCamera.DebugDrawer.AddLine(
                TargetWorldTransform.Position,
                TargetWorldTransform.Position + _steeringTransform.Forward,
                new Vector4(1f, 0f, 1f, 1f)
            );

            PhysicsWorld.DebugCamera.DebugDrawer.AddWireframeBox(
                TargetWorldTransform.Position,
                TargetWorldTransform.Rotation,
                Vector3.One * 0.25f,
                new Vector4(1f, 0f, 0f, 1f)
            );
        }
    }
}