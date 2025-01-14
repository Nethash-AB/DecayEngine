//using System.Linq;
//using BulletSharp;
//using DecayEngine.Bullet.Managed.Component.RigidBody;
//using DecayEngine.Bullet.Managed.Component.Wheel;
//using DecayEngine.DecPakLib.Math;
//using DecayEngine.DecPakLib.Math.Vector;
//using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
//using DecayEngine.ModuleSDK;
//using DecayEngine.ModuleSDK.Component.Camera;
//using DecayEngine.ModuleSDK.Object.GameObject;
//using DecayEngine.ModuleSDK.Object.Input;
//using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;
//
//namespace DecayEngine.Bullet.Managed
//{
//    public static class CarControllerTester
//    {
//        private static IGameObject _groundGo;
//        private static IGameObject _carGo;
//        private static ICameraPersp _cameraPersp;
//
//        public static void Test(DynamicsWorld dynamicsWorld)
//        {
//            // Camera
//            _cameraPersp = GameEngine.ActiveScene.Components.OfType<ICameraPersp>().FirstOrDefault();
//            _cameraPersp.FieldOfView = MathHelper.DegreesToRadians(75f);
//
//            // Ground Go
//            _groundGo = GameEngine.CreateGameObject((PrefabResource) null, "ground_go");
//            _groundGo.Transform.Position = new Vector3(0f, -2f, 0f);
//            _groundGo.Active = true;
//
//            // Ground Rb
//            RigidBody3DComponent groundRb = new RigidBody3DComponent(new BoxShape(50f, 0.5f, 50f), 0f);
//            _groundGo.AttachComponent(groundRb);
//            groundRb.Active = true;
//            ((RigidBody) groundRb).Friction = 1f;
//
//            // Car Go
//            _carGo = GameEngine.CreateGameObject((PrefabResource) null, "test_car");
//            _carGo.Transform.Position = new Vector3(0f, 2f, -2f);
//            _carGo.Transform.Rotation = Quaternion.Identity;
//            _carGo.Active = true;
//
//            // Car Rb
//            const float carMass = 800f;
//            RigidBody3DComponent chassisRb = new RigidBody3DComponent(new BoxShape(1f, 0.5f, 2f), carMass);
//            _carGo.AttachComponent(chassisRb);
//            chassisRb.Active = true;
//            chassisRb.SynchronizeParentTransform = true;
//
//            // Wheels
//            const float wheelHeight = 0f;
//
//            WheelComponent frontLeftWheel = CreateWheel(
//                _carGo, chassisRb,
//                new Vector3(-1f, wheelHeight, -1.5f),
//                true
//            );
//            dynamicsWorld.AddAction(frontLeftWheel);
//
//            WheelComponent frontRightWheel = CreateWheel(
//                _carGo, chassisRb,
//                new Vector3(1f, wheelHeight, -1.5f),
//                true
//            );
//            dynamicsWorld.AddAction(frontRightWheel);
//
//            WheelComponent rearLeftWheel = CreateWheel(
//                _carGo, chassisRb,
//                new Vector3(-1f, wheelHeight, 1.5f),
//                false
//            );
//            dynamicsWorld.AddAction(rearLeftWheel);
//            WheelComponent rearRightWheel = CreateWheel(
//                _carGo, chassisRb,
//                new Vector3(1f, wheelHeight, 1.5f),
//                false
//            );
//            dynamicsWorld.AddAction(rearRightWheel);
//
//            frontLeftWheel.Active = true;
//            frontRightWheel.Active = true;
//            rearLeftWheel.Active = true;
//            rearRightWheel.Active = true;
//
//            // Input
//            IInputAction testAction1 = GameEngine.CreateInputAction("testAction1");
//            testAction1.AddKeyboardTrigger(KeyboardScanCode.Space, 0f);
//            testAction1.OnDigitalDeactivate += action =>
//            {
//                chassisRb.RigidBodyHandle.ApplyImpulse(BulletSharp.Math.Vector3.UnitX * 1000f, new BulletSharp.Math.Vector3(-1f, 0f, -1.5f));
//            };
//        }
//
//        private static WheelComponent CreateWheel(IGameObject parent, RigidBody3DComponent chassisRb, Vector3 position, bool isFront)
//        {
//            const float wheelRadius = 0.5f;
//            const float suspensionLength = 0.75f;
//            const float stiffness = 20f;
//            const float compression = 0.3f;
//            const float relaxation = 0.5f;
//            const float frictionSlipFront = 3f;
//            const float frictionSlipRear = 2f;
//            const float rollInfluence = 0.2f;
//
//            Vector3 wheelDirection = new Vector3(0f, -1f, 0f);
//
//            IGameObject wheelGo = GameEngine.CreateGameObject((PrefabResource) null, "test_wheel");
//            wheelGo.Transform.Position = position;
//            wheelGo.Active = true;
////            parent.AddChild(wheelGo);
//
//            WheelComponent wheelComponent = new WheelComponent(chassisRb)
//            {
//                IsFrontWheel = isFront,
//                ChassisConnectionPointCs = position,
//                WheelDirectionCs = wheelDirection,
//                WheelAxleCs = new Vector3(1f, 0f, 0f),
//                SuspensionRestLength = suspensionLength,
//                WheelsRadius = wheelRadius,
//                SuspensionStiffness = stiffness,
//                WheelsDampingCompression = compression * 2f * (float) System.Math.Sqrt(stiffness),
//                WheelsDampingRelaxation = relaxation * 2f * (float) System.Math.Sqrt(stiffness),
//                FrictionSlip = isFront ? frictionSlipFront : frictionSlipRear,
//                MaxSuspensionTravel = 1f,
//                MaxSuspensionForce = 6000f,
//                RollInfluence = rollInfluence,
//                WheelTransform = wheelGo.Transform
//            };
//            wheelGo.AttachComponent(wheelComponent);
//            wheelComponent.Active = true;
//
//            return wheelComponent;
//        }
//    }
//}