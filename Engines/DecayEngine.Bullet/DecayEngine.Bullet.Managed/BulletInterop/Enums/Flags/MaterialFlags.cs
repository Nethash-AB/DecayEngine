using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum MaterialFlags
    {
        None = 0,
        DebugDraw = 0x0001,
        Default = DebugDraw
    }
}