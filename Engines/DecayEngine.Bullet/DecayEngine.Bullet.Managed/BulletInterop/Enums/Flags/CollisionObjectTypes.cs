using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum CollisionObjectTypes
    {
        None = 0,
        CollisionObject = 1,
        RigidBody = 2,
        GhostObject = 4,
        SoftBody = 8,
        HfFluid = 16,
        UserType = 32,
        FeatherstoneLink = 64
    }
}