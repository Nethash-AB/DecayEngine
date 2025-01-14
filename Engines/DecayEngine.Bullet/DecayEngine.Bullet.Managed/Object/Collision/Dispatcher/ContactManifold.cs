using System;
using System.Collections.Generic;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic;
using DecayEngine.Bullet.Managed.Component.Collision;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Dispatcher
{
    public class ContactManifold : NativeObject, IContactManifold
    {
        private NativeReadOnlyList<ContactPoint> _contactPoints;

        public ICollisionObject CollisionObjectA
        {
            get
            {
                if (!IsNativeHandleInitialized) return null;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    IntPtr collisionObjectHandle = btPersistentManifold_getBody0(NativeHandle);
                    return CollisionObject.FromNativeHandle(collisionObjectHandle);
                });
            }
        }

        public ICollisionObject CollisionObjectB
        {
            get
            {
                if (!IsNativeHandleInitialized) return null;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    IntPtr collisionObjectHandle = btPersistentManifold_getBody1(NativeHandle);
                    return CollisionObject.FromNativeHandle(collisionObjectHandle);
                });
            }
        }

        public IEnumerable<IContactManifoldPoint> ContactPoints => _contactPoints;

        private ContactManifold(IntPtr nativehandle)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = nativehandle;
                CreateNativeContactPointArray();
            });
        }

        public static ContactManifold FromNativeHandle(IntPtr nativeHandle)
        {
            ContactManifold trackedObject = NativeObjectTracker.Get<ContactManifold>(nativeHandle);
            return trackedObject ?? new ContactManifold(nativeHandle);
        }

        private void CreateNativeContactPointArray()
        {
            _contactPoints = new NativeReadOnlyList<ContactPoint>(
                index =>
                {
                    if (!IsNativeHandleInitialized) return null;

                    IntPtr contactPointHandle = btPersistentManifold_getContactPoint(NativeHandle, index);
                    return ContactPoint.FromNativeHandle(contactPointHandle);
                },
                () => !IsNativeHandleInitialized ? 0 : btPersistentManifold_getNumContacts(NativeHandle)
            );
        }

        protected override void FreeUnmanagedHandles()
        {
        }
    }
}