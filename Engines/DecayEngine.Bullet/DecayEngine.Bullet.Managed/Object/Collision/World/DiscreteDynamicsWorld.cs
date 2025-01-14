using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic;
using DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags;
using DecayEngine.Bullet.Managed.Component.Collision;
using DecayEngine.Bullet.Managed.Component.RigidBody;
using DecayEngine.Bullet.Managed.Debug;
using DecayEngine.Bullet.Managed.Object.Collision.Broadphase;
using DecayEngine.Bullet.Managed.Object.Collision.ConstraintSolver;
using DecayEngine.Bullet.Managed.Object.Collision.Dispatcher;
using DecayEngine.Bullet.Managed.Object.Collision.Filter;
using DecayEngine.Bullet.Managed.Object.RayTrace;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Object.Collision.Filter;
using DecayEngine.ModuleSDK.Object.Collision.Manifold;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.RayTrace;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.World
{
    public class DiscreteDynamicsWorld : NativeObject, IPhysicsWorld
    {
        private readonly InternalTickCallbackUnmanaged _preSubstepCallback;
        private readonly InternalTickCallbackUnmanaged _postSubstepCallback;

        private readonly List<IPhysicsUpdateable> _managedUpdateables;
        private readonly List<ICollisionGroup> _collisionGroups;
        private readonly List<ICollisionGroupPair> _ignoredCollisionGroupPairs;
        private readonly List<ICollisionObjectPair> _ignoredCollisionObjectPairs;
        private NativeList<CollisionObject> _collisionObjects;

        private CollisionDispatcher _collisionDispatcher;
        private DbvtBroadphase _broadphaseInterface;
        private SequentialImpulseConstraintSolver _constraintSolver;

        private BulletDebugDrawer _debugDrawer;
        private bool _drawDebug;
        private bool _abortStep;

        public bool DrawDebug
        {
            get => _drawDebug && _debugDrawer != null && _debugDrawer.IsNativeHandleInitialized;
            set => _drawDebug = value;
        }
        public ICameraPersp DebugCamera { get; set; }

        public PhysicsWorldState PhysicsWorldState { get; private set; }
        public IEnumerable<IPhysicsUpdateable> PhysicsUpdateables => _managedUpdateables.Concat(_collisionObjects).Where(pu => pu != null);
        public IEnumerable<ICollisionObject> CollisionObjects => _collisionObjects.Where(co => co != null);

        public ICollisionGroup DefaultCollisionGroup => _collisionGroups[0];
        public IEnumerable<ICollisionGroup> CollisionGroups => _collisionGroups;
        public IEnumerable<ICollisionGroupPair> IgnoredCollisionGroupPairs => _ignoredCollisionGroupPairs;
        public IEnumerable<ICollisionObjectPair> IgnoredCollisionObjectPairs => _ignoredCollisionObjectPairs;

        public Vector3 Gravity
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btDynamicsWorld_getGravity(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;

                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btDynamicsWorld_setGravity(NativeHandle, ref value));
            }
        }
        public float FixedTimeStep { get; set; }

        public DiscreteDynamicsWorld()
        {
            _preSubstepCallback = PhysicsPreSubstep;
            _postSubstepCallback = PhysicsPostSubstep;

            _collisionGroups = new List<ICollisionGroup>
            {
                new DefaultCollisionGroup(this)
            };

            _ignoredCollisionGroupPairs = new List<ICollisionGroupPair>();
            _ignoredCollisionObjectPairs = new List<ICollisionObjectPair>();
            _managedUpdateables = new List<IPhysicsUpdateable>();

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                _collisionDispatcher = new CollisionDispatcher();
                _broadphaseInterface = new DbvtBroadphase();
                _constraintSolver = new SequentialImpulseConstraintSolver();

                NativeHandle = btDiscreteDynamicsWorld_new(
                    _collisionDispatcher.NativeHandle,
                    _broadphaseInterface.NativeHandle,
                    _constraintSolver.NativeHandle,
                    _collisionDispatcher.CollisionConfiguration.NativeHandle);

                CreateNativeObjectArray();

                Gravity = new Vector3(0, -10, 0);
                FixedTimeStep = 1f / 60f;

                btDynamicsWorld_setInternalTickCallback(
                    NativeHandle,
                    Marshal.GetFunctionPointerForDelegate(_preSubstepCallback),
                    IntPtr.Zero,
                    true
                );

                btDynamicsWorld_setInternalTickCallback(
                    NativeHandle,
                    Marshal.GetFunctionPointerForDelegate(_postSubstepCallback),
                    IntPtr.Zero,
                    false
                );

                CreateDebugDrawer();

                PhysicsWorldState = PhysicsWorldState.PreStep;
            });
        }

        public override void Destroy()
        {
            if (Destroyed) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                _abortStep = true;

                foreach (CollisionObject collisionObject in _collisionObjects.Reverse())
                {
                    collisionObject.Destroy();
                }

                _collisionObjects.Clear();
                _collisionObjects = null;

                _managedUpdateables.Clear();

                _collisionDispatcher.Destroy();
                _collisionDispatcher = null;

                _broadphaseInterface.Destroy();
                _broadphaseInterface = null;

                _constraintSolver.Destroy();
                _constraintSolver = null;

                base.Destroy();
            });
        }

        public void AddUpdateable(IPhysicsUpdateable physicsUpdateable)
        {
            if (physicsUpdateable is CollisionObject collisionObject)
            {
                if (_collisionObjects.Contains(collisionObject)) return;
                _collisionObjects.Add(collisionObject);
            }
            else
            {
                if (_managedUpdateables.Contains(physicsUpdateable)) return;
                _managedUpdateables.Add(physicsUpdateable);
                physicsUpdateable.PhysicsWorld = this;
            }
        }

        public void RemoveUpdateable(IPhysicsUpdateable physicsUpdateable)
        {
            if (physicsUpdateable is CollisionObject collisionObject)
            {
                if (!_collisionObjects.Contains(collisionObject)) return;
                _collisionObjects.Remove(collisionObject);
            }
            else
            {
                if (!_managedUpdateables.Contains(physicsUpdateable)) return;
                _managedUpdateables.Remove(physicsUpdateable);
            }
        }

        public void AddIgnoredCollisionGroupPair(ICollisionGroup collisionGroupA, ICollisionGroup collisionGroupB)
        {
            if (_ignoredCollisionGroupPairs.Any(pair =>
                pair.CollisionGroupA == collisionGroupA && pair.CollisionGroupB == collisionGroupB ||
                pair.CollisionGroupA == collisionGroupB && pair.CollisionGroupB == collisionGroupA
            )) return;

            _ignoredCollisionGroupPairs.Add(new CollisionGroupPair(collisionGroupA, collisionGroupB));
        }

        public void RemoveIgnoredCollisionGroupPair(ICollisionGroup collisionGroupA, ICollisionGroup collisionGroupB)
        {
            ICollisionGroupPair collisionGroupPair = _ignoredCollisionGroupPairs.FirstOrDefault(pair =>
                pair.CollisionGroupA == collisionGroupA && pair.CollisionGroupB == collisionGroupB ||
                pair.CollisionGroupA == collisionGroupB && pair.CollisionGroupB == collisionGroupA
            );

            if (collisionGroupPair != null)
            {
                _ignoredCollisionGroupPairs.Remove(collisionGroupPair);
            }
        }

        public void AddIgnoredCollisionObjectPair(ICollisionObject collisionObjectA, ICollisionObject collisionObjectB)
        {
            if (_ignoredCollisionObjectPairs.Any(pair =>
                pair.CollisionObjectA == collisionObjectA && pair.CollisionObjectB == collisionObjectB ||
                pair.CollisionObjectA == collisionObjectB && pair.CollisionObjectB == collisionObjectA
            )) return;

            _ignoredCollisionObjectPairs.Add(new CollisionObjectPair(collisionObjectA, collisionObjectB));
        }

        public void RemoveIgnoredCollisionObjectPair(ICollisionObject collisionObjectA, ICollisionObject collisionObjectB)
        {
            ICollisionObjectPair collisionObjectPair = _ignoredCollisionObjectPairs.FirstOrDefault(pair =>
                pair.CollisionObjectA == collisionObjectA && pair.CollisionObjectB == collisionObjectB ||
                pair.CollisionObjectA == collisionObjectB && pair.CollisionObjectB == collisionObjectA
            );

            if (collisionObjectPair != null)
            {
                _ignoredCollisionObjectPairs.Remove(collisionObjectPair);
            }
        }

        public ICollisionGroup AddOrGetCollisionGroup(string name)
        {
            ICollisionGroup existingGroup = _collisionGroups.FirstOrDefault(cg => cg.Name == name);
            if (existingGroup != null) return existingGroup;

            ICollisionGroup collisionGroup = new CollisionGroup(name, this);
            _collisionGroups.Add(collisionGroup);
            return collisionGroup;
        }

        public void RemoveCollisionGroup(ICollisionGroup collisionGroup)
        {
            if (!_collisionGroups.Contains(collisionGroup)) return;
            _collisionGroups.Remove(collisionGroup);
            _ignoredCollisionGroupPairs.RemoveAll(pair => pair.CollisionGroupA == collisionGroup || pair.CollisionGroupB == collisionGroup);
        }

        public IEnumerable<IRayTraceResult> RayTrace(Vector3 fromPosition, Vector3 toPosition,
            bool stopAfterFirstHit = true, List<ICollisionObject> ignoreList = null)
        {
            if (!IsNativeHandleInitialized) return new List<IRayTraceResult>();

            if (ignoreList == null) ignoreList = new List<ICollisionObject>();

            return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                RayTraceResultCallback rayCastResultCallback =
                    new RayTraceResultCallback(fromPosition, toPosition, stopAfterFirstHit, ignoreList);

                btCollisionWorld_rayTest(NativeHandle, ref fromPosition, ref toPosition, rayCastResultCallback.NativeHandle);

                List<IRayTraceResult> results = rayCastResultCallback.Results.ToList();

                if (!DrawDebug || _debugDrawer == null || !_debugDrawer.IsNativeHandleInitialized) return results;

                if (results.Any())
                {
                    Vector3 source = fromPosition;
                    foreach (IRayTraceResult rayTraceResult in results)
                    {
                        _debugDrawer.DrawLine(source, rayTraceResult.HitPointWorld, new Vector3(0f, 1f, 0f));
                        source = rayTraceResult.HitPointWorld;
                    }
                }
                else
                {
                    _debugDrawer.DrawLine(fromPosition, toPosition, new Vector3(1f, 0f, 0f));
                }

                return results;
            });
        }

        public void Step(float timeStep, int maxSubSteps)
        {
            if (!IsNativeHandleInitialized || _abortStep) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                PhysicsWorldState = PhysicsWorldState.PreStep;
                btDynamicsWorld_stepSimulation(NativeHandle, timeStep, maxSubSteps, FixedTimeStep);
                PhysicsWorldState = PhysicsWorldState.PostStep;

                btCollisionWorld_debugDrawWorld(NativeHandle);
            });
        }

        private void PhysicsPreSubstep(IntPtr world, float timeStep)
        {
            if (world != NativeHandle) return;

            PhysicsWorldState = PhysicsWorldState.PreSubStep;

            foreach (IPhysicsUpdateable physicsUpdateable in _managedUpdateables.Where(pu => pu != null && pu.Active).ToList())
            {
                physicsUpdateable.PhysicsPreUpdate(timeStep);
            }

            foreach (IPhysicsUpdateable physicsUpdateable in _collisionObjects.Where(pu => pu != null && pu.Active).ToList())
            {
                physicsUpdateable.PhysicsPreUpdate(timeStep);
            }
        }

        private void PhysicsPostSubstep(IntPtr world, float timeStep)
        {
            if (world != NativeHandle) return;

            PhysicsWorldState = PhysicsWorldState.PostSubStep;

            foreach (CollisionObject collisionObject in _collisionObjects.Where(co => co != null).ToList())
            {
                foreach (IContactManifold contactManifold in collisionObject.ContactManifolds.Reverse().ToList())
                {
                    if (!contactManifold.ContactPoints.Any())
                    {
                        collisionObject.RemoveContactManifold(contactManifold);
                    }
                }
            }

            foreach (IContactManifold contactManifold in _collisionDispatcher.ContactManifolds)
            {
                if (!contactManifold.ContactPoints.Any()) continue;

                ((CollisionObject) contactManifold.CollisionObjectA).AddContactManifold(contactManifold);
                ((CollisionObject) contactManifold.CollisionObjectB).AddContactManifold(contactManifold);
            }

            foreach (IPhysicsUpdateable physicsUpdateable in _managedUpdateables.Where(pu => pu != null && pu.Active).ToList())
            {
                physicsUpdateable.PhysicsPostUpdate(timeStep);
            }

            foreach (IPhysicsUpdateable physicsUpdateable in _collisionObjects.Where(pu => pu != null && pu.Active).ToList())
            {
                physicsUpdateable.PhysicsPostUpdate(timeStep);
            }
        }

        private void CreateNativeObjectArray()
        {
            _collisionObjects = new NativeList<CollisionObject>(
                index =>
                {
                    if (!IsNativeHandleInitialized) return null;

                    IntPtr objectArrayHandle = btCollisionWorld_getCollisionObjectArray(NativeHandle);
                    IntPtr collisionObjectHandle = btAlignedObjectArray_btCollisionObjectPtr_at(objectArrayHandle, index);
                    return CollisionObject.FromNativeHandle(collisionObjectHandle);
                },
                () =>
                {
                    if (!IsNativeHandleInitialized) return 0;

                    IntPtr objectArrayHandle = btCollisionWorld_getCollisionObjectArray(NativeHandle);
                    return btAlignedObjectArray_btCollisionObjectPtr_size(objectArrayHandle);
                },
                element =>
                {
                    if (!IsNativeHandleInitialized) return -1;

                    IntPtr objectArrayHandle = btCollisionWorld_getCollisionObjectArray(NativeHandle);
                    return btAlignedObjectArray_btCollisionObjectPtr_findLinearSearch2(objectArrayHandle, element.NativeHandle);
                },
                element =>
                {
                    if (!IsNativeHandleInitialized || !element.IsNativeHandleInitialized) return;

                    if (element is RigidBodyComponent rigidBody)
                    {
                        btDynamicsWorld_addRigidBody(NativeHandle, rigidBody.NativeHandle);
                    }
                    else
                    {
                        btCollisionWorld_addCollisionObject(NativeHandle, element.NativeHandle);
                    }

                    element.PhysicsWorld = this;
                },
                element =>
                {
                    if (!IsNativeHandleInitialized) return false;

                    if (element is RigidBodyComponent rigidBody)
                    {
                        btDynamicsWorld_removeRigidBody(NativeHandle, rigidBody.NativeHandle);
                    }
                    else
                    {
                        btCollisionWorld_removeCollisionObject(NativeHandle, element.NativeHandle);
                    }

                    element.PhysicsWorld = null;
                    return true;
                }
            );
        }

        private void CreateDebugDrawer()
        {
            _debugDrawer = new BulletDebugDrawer(this, DebugDrawFlags.All & ~DebugDrawFlags.DrawAabb);
            btCollisionWorld_setDebugDrawer(NativeHandle, _debugDrawer.NativeHandle);
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCollisionWorld_delete(NativeHandle);
            });
        }
    }
}