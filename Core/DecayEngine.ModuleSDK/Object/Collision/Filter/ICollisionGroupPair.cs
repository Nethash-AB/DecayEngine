namespace DecayEngine.ModuleSDK.Object.Collision.Filter
{
    public interface ICollisionGroupPair
    {
        ICollisionGroup CollisionGroupA { get; }
        ICollisionGroup CollisionGroupB { get; }
    }
}