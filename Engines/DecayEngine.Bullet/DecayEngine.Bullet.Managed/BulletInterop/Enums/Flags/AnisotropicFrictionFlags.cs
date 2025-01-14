using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum AnisotropicFrictionFlags
    {
        FrictionDisabled = 0,
        Friction = 1,
        RollingFriction = 2
    }
}