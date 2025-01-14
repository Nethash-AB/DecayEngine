using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum CollisionFlags
    {
        None = 0,
        StaticObject = 1,
        KinematicObject = 2,
        NoContactResponse = 4,
        CustomMaterialCallback = 8,
        CharacterObject = 16,
        DisableVisualizeObject = 32,
        DisableSpuCollisionProcessing = 64,
        HasContactStiffnessDamping = 128,
        HasCustomDebugRenderingColor = 256,
        HasFrictionAnchor = 512,
        HasCollisionSoundTrigger = 1024
    }
}