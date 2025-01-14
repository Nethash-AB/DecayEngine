using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Object.Collision.Filter;
using DecayEngine.ModuleSDK.Object.RayTrace;

namespace DecayEngine.ModuleSDK.Object.Collision.World
{
    public interface IPhysicsWorld
    {
        bool DrawDebug { get; set; }
        ICameraPersp DebugCamera { get; set; }

        PhysicsWorldState PhysicsWorldState { get; }
        IEnumerable<IPhysicsUpdateable> PhysicsUpdateables { get; }
        IEnumerable<ICollisionObject> CollisionObjects { get; }

        ICollisionGroup DefaultCollisionGroup { get; }
        IEnumerable<ICollisionGroup> CollisionGroups { get; }
        IEnumerable<ICollisionGroupPair> IgnoredCollisionGroupPairs { get; }
        IEnumerable<ICollisionObjectPair> IgnoredCollisionObjectPairs { get; }

        Vector3 Gravity { get; set; }
        float FixedTimeStep { get; set; }

        void AddUpdateable(IPhysicsUpdateable physicsUpdateable);
        void RemoveUpdateable(IPhysicsUpdateable physicsUpdateable);

        void AddIgnoredCollisionGroupPair(ICollisionGroup collisionGroupA, ICollisionGroup collisionGroupB);
        void RemoveIgnoredCollisionGroupPair(ICollisionGroup collisionGroupA, ICollisionGroup collisionGroupB);

        void AddIgnoredCollisionObjectPair(ICollisionObject collisionObjectA, ICollisionObject collisionObjectB);
        void RemoveIgnoredCollisionObjectPair(ICollisionObject collisionObjectA, ICollisionObject collisionObjectB);

        ICollisionGroup AddOrGetCollisionGroup(string name);
        void RemoveCollisionGroup(ICollisionGroup collisionGroup);

        IEnumerable<IRayTraceResult> RayTrace(Vector3 fromPosition, Vector3 toPosition,
            bool stopAfterFirstHit = true, List<ICollisionObject> ignoreList = null);

        void Step(float timeStep, int maxSubSteps);
    }
}