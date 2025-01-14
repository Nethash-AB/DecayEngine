using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum DebugDrawFlags
    {
        None = 0,
        DrawWireframe = 1,
        DrawAabb = 2,
        DrawFeaturesText = 4,
        DrawContactPoints = 8,
        NoDeactivation = 16,
        NoHelpText = 32,
        DrawText = 64,
        ProfileTimings = 128,
        EnableSatComparison = 256,
        DisableBulletLcp = 512,
        EnableCcd = 1024,
        DrawConstraints = 1 << 11,
        DrawConstraintLimits = 1 << 12,
        DrawFastWireframe = 1 << 13,
        DrawNormals = 1 << 14,
        DrawFrames = 1 << 15,
        All = DrawWireframe | DrawAabb | DrawFeaturesText | DrawContactPoints | DrawText |
              DrawConstraints | DrawConstraintLimits | DrawFastWireframe | DrawNormals | DrawFrames,
        MaxDebugDrawMode
    }
}