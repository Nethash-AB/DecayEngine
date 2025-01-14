using System;
using System.Collections.Generic;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Dispatcher
{
    public class CollisionDispatcher : NativeObject
    {
//        private readonly NearCallbackDelegate _nearCallbackDelegate;
        private NativeReadOnlyList<ContactManifold> _contactManifolds;

        public CollisionConfiguration CollisionConfiguration { get; private set; }
        public IEnumerable<IContactManifold> ContactManifolds => _contactManifolds;

        public CollisionDispatcher()
        {
//            _nearCallbackDelegate = NearCallback;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                CollisionConfiguration = new CollisionConfiguration();

                NativeHandle = btCollisionDispatcher_new(CollisionConfiguration.NativeHandle);
                CreateNativeManifoldArray();
//                btCollisionDispatcher_setNearCallback(NativeHandle, Marshal.GetFunctionPointerForDelegate(_nearCallbackDelegate));
            });
        }

        public override void Destroy()
        {
            if (Destroyed) return;

            CollisionConfiguration.Destroy();
            CollisionConfiguration = null;

            base.Destroy();
        }

        private void CreateNativeManifoldArray()
        {
            _contactManifolds = new NativeReadOnlyList<ContactManifold>(
                index =>
                {
                    if (!IsNativeHandleInitialized) return null;

                    IntPtr manifoldHandle = btDispatcher_getManifoldByIndexInternal(NativeHandle, index);
                    return ContactManifold.FromNativeHandle(manifoldHandle);
                },
                () => !IsNativeHandleInitialized ? 0 : btDispatcher_getNumManifolds(NativeHandle));
        }

        // ToDo: Explore additional usages of this callback.
//        private void NearCallback(IntPtr collisionPair, IntPtr dispatcher, IntPtr dispatchInfo)
//        {
//            if (dispatcher != NativeHandle)
//            {
//                btCollisionDispatcher_defaultNearCallback(collisionPair, dispatcher, dispatchInfo);
//                return;
//            }
//
//            IntPtr proxyHandleA = btBroadphasePair_getPProxy0(collisionPair);
//            if (proxyHandleA == IntPtr.Zero) return;
//            CollisionObject collisionObjectA = CollisionObject.FromNativeBroadphaseHandle(proxyHandleA);
//            if (collisionObjectA == null) return;
//
//            IntPtr proxyHandleB = btBroadphasePair_getPProxy1(collisionPair);
//            if (proxyHandleB == IntPtr.Zero) return;
//            CollisionObject collisionObjectB = CollisionObject.FromNativeBroadphaseHandle(proxyHandleB);
//            if (collisionObjectB == null) return;
//
//            btCollisionDispatcher_defaultNearCallback(collisionPair, dispatcher, dispatchInfo);
//        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btDispatcher_delete(NativeHandle);
            });
        }
    }
}