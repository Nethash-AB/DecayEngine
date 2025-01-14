using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum SolverModes
    {
        None = 0,
        RandomizeOrder = 1,
        FrictionSeparate = 2,
        UseWarmStarting = 4,
        Use2FrictionDirections = 16,
        EnableFrictionDirectionCaching = 32,
        DisableVelocityDependentFrictionDirection = 64,
        CacheFriendly = 128,
        Simd = 256,
        InterleaveContactAndFrictionConstraints = 512,
        AllowZeroLengthFrictionDirections = 1024,
        DisableImplicitConeFriction = 2048
    }
}