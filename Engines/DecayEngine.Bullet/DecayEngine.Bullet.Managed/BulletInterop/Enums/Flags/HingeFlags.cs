using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum HingeFlags
    {
        None = 0,
        CfmStop = 1,
        ErpStop = 2,
        CfmNormal = 4,
        ErpNormal = 8
    }
}