using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum SixDofFlags
    {
        None = 0,
        CfmNormal = 1,
        CfmStop = 2,
        ErpStop = 4
    }
}