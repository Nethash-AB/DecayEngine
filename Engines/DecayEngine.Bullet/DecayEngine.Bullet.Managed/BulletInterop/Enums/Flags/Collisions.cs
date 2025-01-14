using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum Collisions
    {
        None = 0,
        RigidSoftMask = 0x000f,
        SdfRigidSoft = 0x0001,
        ClusterConvexRigidSoft = 0x0002,
        SoftSoftMask = 0x0030,
        VertexFaceSoftSoft = 0x0010,
        ClusterClusterSoftSoft = 0x0020,
        ClusterSelf = 0x0040,
        Default = SdfRigidSoft
    }
}