using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Math;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Compound;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape
{
    public interface ICollisionShape : IDestroyable
    {
        ICollisionShapeCompound Parent { get; }
        ByReference<ICollisionShapeCompound> ParentByRef { get; }

        Aabb Aabb { get; }
        Vector3 LocalScale { get; set; }
        float Margin { get; set; }

        void SetParent(ICollisionShapeCompound parent);
        Vector3 CalculateLocalInertia(float mass);
        void RecalculateLocalAabb();
    }
}