using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum ConeTwistFlags
    {
        None = 0,
        LinearCfm = 1,
        LinearErp = 2,
        AngularCfm = 4
    }
}