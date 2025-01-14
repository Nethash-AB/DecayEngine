using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum CpuFeatures
    {
        None = 0,
        Fma3 = 1,
        Sse41 = 2,
        NeonHpfp = 4
    }
}