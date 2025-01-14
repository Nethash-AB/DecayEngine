using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum RigidBodyFlags
    {
        None = 0,
        DisableWorldGravity = 1,
        EnableGyroscopicForceExplicit = 2,
        EnableGyroscopicForceImplicitWorld = 4,
        EnableGyroscopicForceImplicitBody = 8,
        EnableGyroscopicForce = EnableGyroscopicForceImplicitBody
    }
}