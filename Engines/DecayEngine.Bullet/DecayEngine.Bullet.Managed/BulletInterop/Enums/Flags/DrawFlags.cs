using System;

namespace DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags
{
    [Flags]
    public enum DrawFlags
    {
        None = 0x00,
        Nodes = 0x01,
        Links = 0x02,
        Faces = 0x04,
        Tetras = 0x08,
        Normals = 0x10,
        Contacts = 0x20,
        Anchors = 0x40,
        Notes = 0x80,
        Clusters = 0x100,
        NodeTree = 0x200,
        FaceTree = 0x400,
        ClusterTree = 0x800,
        Joints = 0x1000,
        Std = Links | Faces | Tetras | Anchors | Notes | Joints,
        StdTetra = Std - Faces - Tetras
    }
}