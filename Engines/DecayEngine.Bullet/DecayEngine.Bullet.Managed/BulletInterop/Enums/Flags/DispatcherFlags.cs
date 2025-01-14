using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum DispatcherFlags
    {
        None = 0,
        StaticStaticReported = 1,
        UseRelativeContactBreakingThreshold = 2,
        DisableContactPoolDynamicAllocation = 4
    }
}