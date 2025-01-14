using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.World;

namespace DecayEngine.ModuleSDK.Engine.Physics
{
    public interface IPhysicsEngine : IMultiThreadedEngine
    {
        IPhysicsWorld PhysicsWorld { get; }
    }

    public interface IPhysicsEngine<in TOptions> : IPhysicsEngine, IMultiThreadedEngine<TOptions> where TOptions : IEngineOptions
    {
    }
}