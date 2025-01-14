using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.Bullet.Managed.Component.RigidBody;
using DecayEngine.Bullet.Managed.Component.Wheel;
using DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Polyhedral;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Coroutines.Results;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.Bullet.Managed
{
    public static class PhysicsTester
    {
        private const float CarMass = 800f;
        private static IGameObject _groundGo;
        private static IGameObject _carGo;
        private static RigidBodyComponent _chassisRb;

        private static WheelComponent _wheelFrontLeft;
        private static WheelComponent _wheelFrontRight;
        private static WheelComponent _wheelRearLeft;
        private static WheelComponent _wheelRearRight;


        private static ITextSprite _debugTextSprite;
        private static ICameraPersp _cameraPersp;
        private static bool _cameraFollow;
        private static readonly Vector3 CameraTopPosition = new Vector3(0f, 40f, 40f);
        private static readonly Quaternion CameraTopRotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-60f));

        public static void Test(IPhysicsWorld physicsWorld)
        {
            // Camera
            _cameraPersp = GameEngine.ActiveScene.Components.OfType<ICameraPersp>().FirstOrDefault();
            _cameraPersp.FieldOfView = MathHelper.DegreesToRadians(75f);
            _cameraPersp.Transform.Position = CameraTopPosition;
            _cameraPersp.Transform.Rotation = CameraTopRotation;

            // Debug Text Drawer
            _debugTextSprite =
                GameEngine.ActiveScene.Children.FirstOrDefault(go => go.Name == "debug_text_label")?.Components.OfType<ITextSprite>().FirstOrDefault();

            // Ground Go
            _groundGo = GameEngine.CreateGameObject((PrefabResource) null, "ground_go");
            _groundGo.Transform.Position = new Vector3(0f, -0.5f, 0f);
            _groundGo.Active = true;

            // Ground Rb
            CollisionShapeBox groundShape = new CollisionShapeBox(50f, 0.5f, 50f);
            RigidBodyComponent groundRb = new RigidBodyComponent(0f, groundShape);
            _groundGo.AttachComponent(groundRb);
            groundRb.Active = true;
            groundRb.LinearFriction = 1f;

            // Car Go
            _carGo = GameEngine.CreateGameObject((PrefabResource) null, "test_car");
            _carGo.Transform.Position = new Vector3(0f, 2f, 0f);
            _carGo.Transform.Rotation = Quaternion.Identity;
            _carGo.Active = true;

            // Car Rb
            CollisionShapeBox chassisShape = new CollisionShapeBox(1f, 0.5f, 2f);
            RigidBodyComponent chassisRb = new RigidBodyComponent(CarMass, chassisShape)
            {
                CenterOfMassOffset = Vector3.Right * 0f + Vector3.Up * -2f + Vector3.Forward * -1.5f
            };
            _carGo.AttachComponent(chassisRb);
            chassisRb.Active = true;
            _chassisRb = chassisRb;
//            chassisRb.IsKinematic = true;

            // Suspension Test
            Vector3 suspensionRight = _carGo.Transform.Right * chassisShape.Aabb.Max.X;
//            Vector3 suspensionDown = -_carGo.Transform.Up * chassisShape.Aabb.Max.Y;
            Vector3 suspensionDown = _carGo.Transform.Up * chassisShape.Aabb.Max.Y * 1f;
            Vector3 suspensionForward = _carGo.Transform.Forward * chassisShape.Aabb.Max.Z;

            WheelSuspensionComponent suspensionFrontLeft =
                CreateSuspension(chassisRb, -suspensionRight + suspensionDown + suspensionForward);
            WheelSuspensionComponent suspensionFrontRight =
                CreateSuspension(chassisRb, suspensionRight + suspensionDown + suspensionForward);
            WheelSuspensionComponent suspensionRearLeft =
                CreateSuspension(chassisRb, -suspensionRight + suspensionDown - suspensionForward);
            WheelSuspensionComponent suspensionRearRight =
                CreateSuspension(chassisRb, suspensionRight + suspensionDown - suspensionForward);

            _wheelFrontLeft = CreateWheel(suspensionFrontLeft, true);
            _wheelFrontRight = CreateWheel(suspensionFrontRight, true);
            _wheelRearLeft = CreateWheel(suspensionRearLeft, false);
            _wheelRearRight = CreateWheel(suspensionRearRight, false);

            // Input
            IInputAction testAction1 = GameEngine.CreateInputAction("testAction1");
            testAction1.AddKeyboardTrigger(KeyboardScanCode.Space, 0f);
            testAction1.OnDigitalDeactivate += action =>
            {
                _carGo.Transform.Position = new Vector3(0f, 2f, 0f);
                _carGo.Transform.Rotation = Quaternion.FromDirection(Vector3.Forward);
                chassisRb.ClearForces();
            };

            IInputAction testAction2 = GameEngine.CreateInputAction("testAction2");
            testAction2.AddKeyboardTrigger(KeyboardScanCode.W, 1f);
            testAction2.AddKeyboardTrigger(KeyboardScanCode.S, -1f);
            testAction2.OnAnalogUpdate += (action, value) =>
            {
                const float maxEngineForce = 5000f;
                const float maxBrakeForce = 5000f;
                float forwardVelocity = Vector3.Dot(_carGo.Transform.Forward, _chassisRb.LinearVelocity);

                float engineForce = 0f;
                float brakeForce = 0f;
                if (value > 0f)
                {
                    if (forwardVelocity > -2f)
                    {
                        engineForce = value * maxEngineForce;
                        brakeForce = 0f;
                    }
                    else
                    {
                        engineForce = 0f;
                        brakeForce = Math.Abs(value) * maxBrakeForce;
                    }
                }
                else if (value < 0f)
                {
                    if (forwardVelocity < 2f)
                    {
                        engineForce = value * maxEngineForce;
                    }
                    else
                    {
                        engineForce = 0f;
                        brakeForce = Math.Abs(value) * maxBrakeForce;
                    }
                }

                _wheelRearLeft.EngineForce = engineForce;
                _wheelRearRight.EngineForce = engineForce;

                _wheelRearLeft.BrakeForce = brakeForce;
                _wheelRearRight.BrakeForce = brakeForce;
            };

            IInputAction testAction3 = GameEngine.CreateInputAction("testAction3");
            testAction3.AddKeyboardTrigger(KeyboardScanCode.D, -1f);
            testAction3.AddKeyboardTrigger(KeyboardScanCode.A, 1f);
            testAction3.OnAnalogUpdate += (action, value) =>
            {
                _wheelFrontLeft.Steering = MathHelper.DegreesToRadians(value * 40f);
                _wheelFrontRight.Steering = MathHelper.DegreesToRadians(value * 40f);
            };

            IInputAction testAction4 = GameEngine.CreateInputAction("testAction4");
            testAction4.AddKeyboardTrigger(KeyboardScanCode.Return, 0f);
            testAction4.OnDigitalDeactivate += action =>
            {
                if (_cameraFollow)
                {
                    _cameraFollow = false;

                    _cameraPersp.Transform.Position = CameraTopPosition;
                    _cameraPersp.Transform.Rotation = CameraTopRotation;
                }
                else
                {
                    _cameraFollow = true;
                }
            };

            // Debug
            GameEngine.CreateCoroutine(UpdateCamera, CoroutineContext.Render).Run();
            GameEngine.CreateCoroutine(UpdateDebugText, CoroutineContext.Render).Run();
        }

        private static WheelSuspensionComponent CreateSuspension(IRigidBody chassisRb, Vector3 connectionPointLocal)
        {
            WheelSuspensionComponent suspensionComponent = new WheelSuspensionComponent(chassisRb, connectionPointLocal)
            {
                RestLength = 1.5f,
                MaxTravelCompression = 0.25f,
                MaxTravelRelaxation = 1.5f,
                MaxForce = 6000f,
                Stiffness = 50f,
                DampingCompression = 0.3f,
                DampingRelaxation = 0.5f
            };
            chassisRb.PhysicsWorld.AddUpdateable(suspensionComponent);
            _carGo.AttachComponent(suspensionComponent);
            suspensionComponent.Active = true;

            return suspensionComponent;
        }

        private static WheelComponent CreateWheel(WheelSuspensionComponent suspensionComponent, bool isFront)
        {
            WheelComponent wheelComponent = new WheelComponent(suspensionComponent)
            {
                Radius = 0.5f,
                FrictionSlip = new Vector2(isFront ? 0.8f : 0.8f, isFront ? 1.8f : 1.8f),
                RollingFriction = 0.2f,
                SideFrictionStiffness = 30f,
                RollInfluence = 0.2f
            };
            suspensionComponent.ChassisRigidBody.PhysicsWorld.AddUpdateable(wheelComponent);
            _carGo.AttachComponent(wheelComponent);
            wheelComponent.Active = true;

            return wheelComponent;
        }

        private static IEnumerator<IYieldResult> UpdateCamera()
        {
            while (_chassisRb != null && _chassisRb.Active && _cameraPersp != null && _cameraPersp.Active)
            {
                if (_cameraFollow)
                {
                    _cameraPersp.Transform.Position = _carGo.Transform.Position - _carGo.Transform.Forward * 4f + _carGo.Transform.Up * 2f;
                    _cameraPersp.Transform.Rotation =
                        Quaternion.FromDirection(_carGo.Transform.Forward) *
                        Quaternion.FromAxisAngle(Vector3.Right, MathHelper.DegreesToRadians(-15f));
                }
                else
                {
                    _cameraPersp.Transform.Rotation = Quaternion.FromDirection(_carGo.Transform.Position - _cameraPersp.Transform.Position);
                }

                yield return new WaitForNextTick();
            }
            yield return null;
        }

        private static IEnumerator<IYieldResult> UpdateDebugText()
        {
            while (_chassisRb != null && _chassisRb.Active && _debugTextSprite != null && _debugTextSprite.Active)
            {
                string text =
                    $"Forward Velocity: {Vector3.Dot(_carGo.Transform.Forward, _chassisRb.LinearVelocity):F3}\n" +
                    $"SkidFactor FL: (X: {_wheelFrontLeft.SkidFactor.X:F2}, Y: {_wheelFrontLeft.SkidFactor.Y:F2})\n" +
                    $"SkidFactor FR: (X: {_wheelFrontRight.SkidFactor.X:F2}, Y: {_wheelFrontRight.SkidFactor.Y:F2})\n" +
                    $"SkidFactor RL: (X: {_wheelRearLeft.SkidFactor.X:F2}, Y: {_wheelRearLeft.SkidFactor.Y:F2})\n" +
                    $"SkidFactor RR: (X: {_wheelRearRight.SkidFactor.X:F2}, Y: {_wheelRearRight.SkidFactor.Y:F2})";

                _debugTextSprite.Text = text;
//                _cameraPersp.DebugDrawer.AddLine(
//                    _chassisRb.CenterOfMassOffset * _carGo.Transform.TransformMatrix,
//                    _carGo.Transform.Position,
//                    new Vector4(0f, 1f, 1f, 1f)
//                );

//                _cameraPersp.DebugDrawer.AddCrosshair(
//                    _carGo.Transform.Position,
//                    _carGo.Transform.Rotation,
//                    Vector3.One * 0.25f
//                );

                Matrix4 comTransform = _chassisRb.CenterOfMass;
                Vector3 comPosWs = comTransform.ExtractTranslation();
                Quaternion comRotWs = comTransform.ExtractRotation();
                _cameraPersp.DebugDrawer.AddLine(
                    comPosWs,
                    _carGo.Transform.Position,
                    new Vector4(0f, 1f, 1f, 1f)
                );

                _cameraPersp.DebugDrawer.AddCrosshair(
                    comPosWs,
                    comRotWs,
                    Vector3.One * 0.25f
                );

                yield return new WaitForNextTick();
            }

            yield return null;
        }
    }
}