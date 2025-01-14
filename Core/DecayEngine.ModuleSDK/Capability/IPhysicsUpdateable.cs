using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.World;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IPhysicsUpdateable : IActivable
    {
        IPhysicsWorld PhysicsWorld { get; set; }

        void PhysicsPreUpdate(float deltaTime);
        void PhysicsPostUpdate(float deltaTime);
    }
}