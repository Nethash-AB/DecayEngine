using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.StubEngines.Physics
{
    public class StubPhysicsEngine : IPhysicsEngine<StubEngineOptions>
    {
        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public IPhysicsWorld PhysicsWorld => null;

        public StubPhysicsEngine()
        {
            ComponentFactories = new List<IComponentFactory>
            {
//                new StubRigidBodyFactory() // ToDo: Uncomment this once collider resource is implemented.
            };

            ScriptExports = new ScriptExports();
        }

        public Task Init(StubEngineOptions options)
        {
            EngineThread = new ManagedEngineThread("StubPhysics", 128);
            EngineThread.Run();

            return Task.CompletedTask;
        }
        public Task Shutdown()
        {
            EngineThread.Stop();
            return Task.CompletedTask;
        }
    }
}