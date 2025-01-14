using System.Collections.Generic;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Math;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Polyhedral;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Polyhedral
{
    public abstract class CollisionShapePolyhedral : CollisionShapeConvex, ICollisionShapePolyhedral
    {
        public IEnumerable<Edge> Edges
        {
            get
            {
                if (!IsNativeHandleInitialized) return new List<Edge>();

                return new NativeReadOnlyList<Edge>(
                    index => {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        {
                            btPolyhedralConvexShape_getEdge(NativeHandle, index, out Vector3 pointA, out Vector3 pointB);
                            return new Edge(pointA, pointB);
                        });
                    },
                    () =>
                    {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btPolyhedralConvexShape_getNumEdges(NativeHandle));
                    }
                );
            }
        }

        public IEnumerable<PlaneNormal> Planes
        {
            get
            {
                if (!IsNativeHandleInitialized) return new List<PlaneNormal>();

                return new NativeReadOnlyList<PlaneNormal>(
                    index => {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        {
                            btPolyhedralConvexShape_getPlane(NativeHandle, out Vector3 planeNormal, out Vector3 planeSupport, index);
                            return new PlaneNormal(planeNormal, planeSupport);
                        });
                    },
                    () =>
                    {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btPolyhedralConvexShape_getNumPlanes(NativeHandle));
                    }
                );
            }
        }

        public IEnumerable<Vector3> Vertices
        {
            get
            {
                if (!IsNativeHandleInitialized) return new List<Vector3>();

                return new NativeReadOnlyList<Vector3>(
                    index => {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                        {
                            btPolyhedralConvexShape_getVertex(NativeHandle, index, out Vector3 vertex);
                            return vertex;
                        });
                    },
                    () =>
                    {
                        return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btPolyhedralConvexShape_getNumVertices(NativeHandle));
                    }
                );
            }
        }

        public bool IsInside(Vector3 point, float tolerance)
        {
            return IsNativeHandleInitialized &&
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btPolyhedralConvexShape_isInside(NativeHandle, ref point, tolerance));
        }
    }
}