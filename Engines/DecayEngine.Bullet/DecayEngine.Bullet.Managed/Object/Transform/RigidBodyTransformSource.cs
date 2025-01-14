using System.Runtime.InteropServices;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.Component.RigidBody;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Transform;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Transform
{
    public class RigidBodyTransformSource : NativeObject, ITransformSource
    {
        private readonly MotionStateGetWorldTransformDelegate _getWorldTransform;
        private readonly MotionStateSetWorldTransformDelegate _setWorldTransform;
        private readonly RigidBodyComponent _rigidBody;

        private Vector3 _position;
        private Quaternion _rotation;

        private bool _discardMotionState;

        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_rigidBody.IsStatic) return;

                _discardMotionState = true;
                _position = value;

                if (_rigidBody.Active)
                {
                    Matrix4 interpolationMatrix = GetInterpolationMatrix();
                    GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        btCollisionObject_setWorldTransform(_rigidBody.NativeHandle, ref interpolationMatrix));
                }
                _discardMotionState = false;
            }
        }

        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (_rigidBody.IsStatic) return;

                _discardMotionState = true;
                _rotation = value;

                if (_rigidBody.Active)
                {
                    Matrix4 interpolationMatrix = GetInterpolationMatrix();
                    GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        btCollisionObject_setInterpolationWorldTransform(_rigidBody.NativeHandle, ref interpolationMatrix));
                }
                _discardMotionState = false;
            }
        }

        public Vector3 Scale
        {
            get => _rigidBody.CollisionShape.LocalScale;
            set
            {
                if (_rigidBody.IsStatic) return;

                _rigidBody.CollisionShape.LocalScale = value;
            }
        }

        public RigidBodyTransformSource(RigidBodyComponent rigidBody)
        {
            _getWorldTransform = GetWorldTransform;
            _setWorldTransform = SetWorldTransform;
            _rigidBody = rigidBody;

            _position = Vector3.Zero;
            _rotation = Quaternion.Identity;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btMotionStateWrapper_new(
                    Marshal.GetFunctionPointerForDelegate(_getWorldTransform),
                    Marshal.GetFunctionPointerForDelegate(_setWorldTransform)
                );
            });
        }

        public void CopyStateFrom(ITransformSource source)
        {
            _discardMotionState = true;

            _position = source.Position;
            _rotation = source.Rotation;
            Scale = source.Scale;

            _discardMotionState = false;
        }

        private void GetWorldTransform(out Matrix4 centerOfMassworldTransform)
        {
            centerOfMassworldTransform = GetInterpolationMatrix();
        }

        private void SetWorldTransform(ref Matrix4 centerOfMassworldTransform)
        {
            if (_discardMotionState) return;

            _position = centerOfMassworldTransform.ExtractTranslation() + _rigidBody.CenterOfMassOffset * centerOfMassworldTransform.Basis;
//            _position = centerOfMassworldTransform.ExtractTranslation();
            _rotation = centerOfMassworldTransform.ExtractRotation();
        }

        private Matrix4 GetInterpolationMatrix()
        {
            return Matrix4.Identity *
                   Matrix4.CreateFromQuaternion(Rotation) *
                   Matrix4.CreateTranslation(Position) *
                   Matrix4.CreateTranslation(-_rigidBody.CenterOfMassOffset);
//            return Matrix4.Identity * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position);
        }

        protected override void FreeUnmanagedHandles()
        {
        }
    }
}