using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum ContactPointFlags
    {
        None = 0,
        LateralFrictionInitialized = 1,
        HasContactCfm = 2,
        HasContactErp = 4,
        ContactStiffnessDamping = 8,
        FrictionAnchor = 16
    }
}