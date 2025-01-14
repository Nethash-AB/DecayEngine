using System;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Dispatcher
{
    public class ContactPoint : NativeObject, IContactManifoldPoint
    {
        public Vector3 LocalPositionA
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btManifoldPoint_getLocalPointA(NativeHandle, out Vector3 value);
                    return value;
                });
            }
        }

        public Vector3 LocalPositionB
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btManifoldPoint_getLocalPointB(NativeHandle, out Vector3 value);
                    return value;
                });
            }
        }

        public Vector3 WorldPositionA
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btManifoldPoint_getPositionWorldOnA(NativeHandle, out Vector3 value);
                    return value;
                });
            }
        }

        public Vector3 WorldPositionB
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btManifoldPoint_getPositionWorldOnB(NativeHandle, out Vector3 value);
                    return value;
                });
            }
        }

        public float Impulse =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getAppliedImpulse(NativeHandle));

        public float ImpulseLateral =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getAppliedImpulseLateral1(NativeHandle));

        public float Damping =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getCombinedContactDamping1(NativeHandle));

        public float Stiffness =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getCombinedContactStiffness1(NativeHandle));

        public float LinearFriction =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getCombinedFriction(NativeHandle));

        public float RollingFriction =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getCombinedRollingFriction(NativeHandle));

        public float Restitution =>
            !IsNativeHandleInitialized
                ? 0f
                : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btManifoldPoint_getCombinedRestitution(NativeHandle));

        private ContactPoint(IntPtr nativehandle)
        {
            NativeHandle = nativehandle;
        }

        public static ContactPoint FromNativeHandle(IntPtr nativeHandle)
        {
            ContactPoint trackedObject = NativeObjectTracker.Get<ContactPoint>(nativeHandle);
            return trackedObject ?? new ContactPoint(nativeHandle);
        }

        protected override void FreeUnmanagedHandles()
        {
        }
    }
}