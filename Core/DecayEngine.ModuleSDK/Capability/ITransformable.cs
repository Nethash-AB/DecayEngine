using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface ITransformable
    {
        Transform Transform { get; }
        ByReference<Transform> TransformByRef { get; }
        Transform WorldSpaceTransform { get; }
    }
}