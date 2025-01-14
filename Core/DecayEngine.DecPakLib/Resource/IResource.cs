using DecayEngine.DecPakLib.DataStructure;
using DecayEngine.DecPakLib.DataStructure.Mesh;
using DecayEngine.DecPakLib.Resource.Expression;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet;
using DecayEngine.DecPakLib.Resource.Structure.Component.Font;
using DecayEngine.DecPakLib.Resource.Structure.Component.Script;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource
{
    [ProtoContract]
    [ProtoInclude(1001, typeof(IRootResource))]
    [ProtoInclude(1002, typeof(IExpression))]
    [ProtoInclude(1003, typeof(AnimationFrameElement))]
    [ProtoInclude(1004, typeof(TriangleStructure))]
    [ProtoInclude(1005, typeof(Vector2Structure))]
    [ProtoInclude(1006, typeof(Vector3Structure))]
    [ProtoInclude(1007, typeof(Vector4Structure))]
    [ProtoInclude(1008, typeof(VertexStructure))]
    [ProtoInclude(1009, typeof(TransformStructure))]
    [ProtoInclude(1010, typeof(ScriptInjection))]
    [ProtoInclude(1011, typeof(FontMipMap))]
    [ProtoInclude(1012, typeof(Glyph))]
    [ProtoInclude(1013, typeof(KerningTableEntry))]
    [ProtoInclude(1014, typeof(PropertySheetStructure))]
    [ProtoInclude(1015, typeof(IPropertySheetValue))]
    [ProtoInclude(1016, typeof(MeshDataStructure))]
    public interface IResource
    {

    }
}