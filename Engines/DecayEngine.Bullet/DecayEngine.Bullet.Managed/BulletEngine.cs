using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.Object.Collision;
using DecayEngine.Bullet.Managed.Object.Collision.World;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.World;
using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.Bullet.Managed
{
    public class BulletEngine : IPhysicsEngine<BulletEngineOptions>
    {
        private BulletEngineOptions _options;
        private DiscreteDynamicsWorld _physicsWorld;

        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public IPhysicsWorld PhysicsWorld => _physicsWorld;

        public BulletEngine()
        {
            ComponentFactories = new List<IComponentFactory>
            {
//                new RigidBodyFactory() // ToDo: Uncomment this once collider resource is implemented.
            };
            ScriptExports = new ScriptExports();
        }

        public Task Init(BulletEngineOptions options)
        {
            _options = options;

            EngineThread = new ManagedEngineThread("Bullet", 128);
            Task initTask = EngineThread.ExecuteOnThreadAsync(InitEngine);

            EngineThread.Run();
            return initTask;
        }

        public Task Shutdown()
        {
            _physicsWorld.Destroy();

            foreach (NativeObject nativeObject in NativeObjectTracker.NativeObjects.Where(no => !no.Destroyed))
            {
                nativeObject.Destroy();
            }
            NativeObjectTracker.Clear();

            EngineThread.Stop();
            _physicsWorld = null;

            GameEngine.LogAppendLine(LogSeverity.Info, "Bullet", "Bullet terminated.");
            return Task.CompletedTask;
        }

        private void InitEngine()
        {
            _physicsWorld = new DiscreteDynamicsWorld
            {
                DrawDebug = _options.DrawDebug
            };

            EngineThread.OnUpdate += Loop;

            GameEngine.OnScenePostload += (scene, isInit) =>
            {
                _physicsWorld.DebugCamera = GameEngine.RenderEngine.Cameras.OfType<ICameraPersp>().FirstOrDefault(c => c.Active && c.DebugDrawer.Active);

                // ToDo: DEBUG REMOVE THIS
                PhysicsTester.Test(_physicsWorld);
                // ToDo: DEBUG REMOVE THIS ^
            };

            GameEngine.LogAppendLine(LogSeverity.Info, "Bullet", $"Bullet loaded. Thread ID: {EngineThread.ThreadId}");
        }

        private void Loop(double deltaTime)
        {
            _physicsWorld.Step((float) deltaTime, 1);
        }
    }
}