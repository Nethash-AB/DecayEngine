using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic;
using DecayEngine.DecPakLib.DataStructure.Mesh;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Polyhedral;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Polyhedral
{
    public class CollisionShapeConvexHull : CollisionShapePolyhedral, ICollisionShapeConvexHull
    {
        public IEnumerable<Vector3> Points
        {
            get
            {
                if (!IsNativeHandleInitialized) return new List<Vector3>();

                return new NativeReadOnlyList<Vector3>(
                    index => {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        {
                            btConvexHullShape_getScaledPoint(NativeHandle, index, out Vector3 vertex);
                            return vertex / LocalScale;
                        });
                    },
                    () =>
                    {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConvexHullShape_getNumPoints(NativeHandle));
                    }
                );
            }
        }

        public CollisionShapeConvexHull(IEnumerable<Vector3> points, int length = int.MaxValue)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btConvexHullShape_new();

                if (points == null) return;

                using (IEnumerator<Vector3> pointEnumerator = points.GetEnumerator())
                {
                    int i = 0;
                    while (pointEnumerator.MoveNext() && i < length)
                    {
                        AddPoint(pointEnumerator.Current, false);
                        i++;
                    }
                }

                RecalculateLocalAabb();
            });
        }

        public CollisionShapeConvexHull(MeshDataStructure meshData) : this(meshData.VertexPositions.Cast<Vector3>())
        {
        }

        public CollisionShapeConvexHull() : this(null, 0)
        {
        }

        public void AddPoint(Vector3 point, bool recalculateLocalAabb = true)
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConvexHullShape_addPoint(NativeHandle, ref point, recalculateLocalAabb));
        }

        public void Optimize()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConvexHullShape_optimizeConvexHull(NativeHandle));
        }

        public void RecalculateLocalAabb()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btPolyhedralConvexAabbCachingShape_recalcLocalAabb(NativeHandle));
        }
    }
}