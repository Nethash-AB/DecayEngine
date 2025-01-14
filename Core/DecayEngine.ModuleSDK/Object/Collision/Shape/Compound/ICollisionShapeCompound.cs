using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Compound
{
    public interface ICollisionShapeChild
    {
        ICollisionShape Shape { get; }
        Vector3 PositionOffset { get; set; }
        Quaternion RotationOffset { get; set; }
    }

    public interface ICollisionShapeCompound : ICollisionShape
    {
        int ChildrenCount { get; }
        IEnumerable<ICollisionShapeChild> Children { get; }

        void AddChild(ICollisionShape child, Vector3 positionOffset, Quaternion rotationOffset);
        void RemoveChild(ICollisionShape child);
        void RemoveAllChildren();
    }
}