using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum SliderFlags
    {
        None = 0,
        CfmDirLinear = 1,
        ErpDirLinear = 2,
        CfmDirAngular = 4,
        ErpDirAngular = 8,
        CfmOrthoLinear = 16,
        ErpOrthoLinear = 32,
        CfmOrthoAngular = 64,
        ErpOrthoAngular = 128,
        CfmLimitLinear = 512,
        ErpLimitLinear = 1024,
        CfmLimitAngular = 2048,
        ErpLimitAngular = 4096
    }
}