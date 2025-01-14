using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.Component.Collision;
using DecayEngine.Bullet.Managed.Debug;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Object.RayTrace;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.RayTrace
{
    internal class RayTraceResultCallback : NativeObject
    {
        private readonly RayTraceCallbackAddSingleResultUnmanagedDelegate _addSingleResultDelegate;
        private readonly RayTraceCallbackNeedsCollisionUnmanagedDelegate _needsCollisionDelegate;

        private readonly Vector3 _raySource;
        private readonly Vector3 _rayTarget;
        private readonly bool _stopAfterFirstHit;
        private readonly List<IRayTraceResult> _results;
        private readonly List<ICollisionObject> _ignoreList;

        public IEnumerable<IRayTraceResult> Results => _results;

        public RayTraceResultCallback(
            Vector3 raySource, Vector3 rayTarget,
            bool stopAfterFirstHit, List<ICollisionObject> ignoreList
        )
        {
            _addSingleResultDelegate = AddSingleResult;
            _needsCollisionDelegate = NeedsCollision;

            _raySource = raySource;
            _rayTarget = rayTarget;
            _stopAfterFirstHit = stopAfterFirstHit;
            _ignoreList = ignoreList;

            _results = new List<IRayTraceResult>();

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btCollisionWorld_RayResultCallbackWrapper_new(
                    Marshal.GetFunctionPointerForDelegate(_addSingleResultDelegate),
                    Marshal.GetFunctionPointerForDelegate(_needsCollisionDelegate)
                );
            });
        }

        private float AddSingleResult(IntPtr rayResult, bool normalInWorldSpace)
        {
            float hitFraction = btCollisionWorld_LocalRayResult_getHitFraction(rayResult);

            IntPtr collisionObjectHandle = btCollisionWorld_LocalRayResult_getCollisionObject(rayResult);
            CollisionObject collisionObject = CollisionObject.FromNativeHandle(collisionObjectHandle);
            btCollisionWorld_LocalRayResult_setCollisionObject(rayResult, collisionObjectHandle);

            btCollisionWorld_LocalRayResult_getHitNormalLocal(rayResult, out Vector3 hitNormalLocal);
            Vector3 hitNormalWorld = normalInWorldSpace ? hitNormalLocal : hitNormalLocal * collisionObject.Parent.WorldSpaceTransform.TransformMatrix;

            Vector3 hitPointWorld = Vector3.Lerp(_raySource, _rayTarget, hitFraction);

            RayTraceResult result = new RayTraceResult(
                _raySource, _rayTarget,
                collisionObject,
                hitFraction, hitPointWorld, hitNormalWorld
            );

            float resultFraction;
            if (_stopAfterFirstHit)
            {
                btCollisionWorld_RayResultCallback_setClosestHitFraction(NativeHandle, hitFraction);

                _results.Clear();
                _results.Add(result);

                resultFraction = hitFraction;
            }
            else
            {
                _results.Insert(0, result);
                resultFraction = btCollisionWorld_RayResultCallback_getClosestHitFraction(NativeHandle);
            }

            return resultFraction;

//            if (_stopAfterFirstHit && _results.Any())
//            {
//                btCollisionWorld_RayResultCallback_setClosestHitFraction(NativeHandle, 1f);
//                return 1f;
//            }
//
//            float hitFraction = btCollisionWorld_LocalRayResult_getHitFraction(rayResult);
//            btCollisionWorld_RayResultCallback_setClosestHitFraction(NativeHandle, hitFraction);
//
//            IntPtr collisionObjectHandle = btCollisionWorld_LocalRayResult_getCollisionObject(rayResult);
//            CollisionObject collisionObject = CollisionObject.FromNativeHandle(collisionObjectHandle);
//            if (collisionObject == null)
//            {
//                return hitFraction;
//            }
//
//            btCollisionWorld_LocalRayResult_getHitNormalLocal(rayResult, out Vector3 hitNormalLocal);
//            Vector3 hitNormalWorld = normalInWorldSpace ? hitNormalLocal : hitNormalLocal * collisionObject.Parent.WorldSpaceTransform.TransformMatrix;
//
//            Vector3 hitPointWorld = Vector3.Lerp(_raySource, _rayTarget, hitFraction);
//
//            if (_debugDrawer != null && _debugDrawer.IsNativeHandleInitialized)
//            {
//                Vector3 rayFractionSource = _results.Any() ? _results.Last().HitPointWorld : _raySource;
//                _debugDrawer.DrawLine(rayFractionSource, hitPointWorld, new Vector3(0f, 1f, 0f));
//            }
//
//            _results.Add(
//                new RayTraceResult(
//                    _raySource, _rayTarget,
//                    collisionObject,
//                    hitFraction, hitPointWorld, hitNormalWorld
//                )
//            );
//
//            return hitFraction;
        }

        private bool NeedsCollision(IntPtr proxy)
        {
            CollisionObject collisionObject = CollisionObject.FromNativeBroadphaseHandle(proxy);
            return collisionObject != null &&
                   !_ignoreList.Contains(collisionObject) &&
                   btCollisionWorld_RayResultCallbackWrapper_needsCollision(NativeHandle, proxy);
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCollisionWorld_RayResultCallback_delete(NativeHandle);
            });
        }
    }
}