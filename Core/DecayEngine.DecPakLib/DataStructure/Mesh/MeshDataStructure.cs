using System.Collections.Generic;
using System.IO;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;
using ProtoBuf.Meta;

namespace DecayEngine.DecPakLib.DataStructure.Mesh
{
    [ProtoContract]
    public class MeshDataStructure
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public List<Vector3Structure> VertexPositions { get; set; }
        [ProtoMember(3)]
        public List<Vector3Structure> VertexNormals { get; set; }
        [ProtoMember(4)]
        public List<Vector3Structure> VertexTangents { get; set; }
        [ProtoMember(5)]
        public List<Vector3Structure> VertexBitangents { get; set; }
        [ProtoMember(6)]
        public List<Vector2Structure> VertexUvCoordinates { get; set; }
        [ProtoMember(7)]
        public List<TriangleStructure> Triangles { get; set; }

        public MemoryStream Serialize()
        {
            MemoryStream ms = new MemoryStream();

            RuntimeTypeModel runtimeTypeModel = TypeModel.Create();
            runtimeTypeModel.AutoCompile = false;
            runtimeTypeModel.AutoAddMissingTypes = true;
            runtimeTypeModel.UseImplicitZeroDefaults = true;
            runtimeTypeModel.AutoAddProtoContractTypesOnly = true;

            runtimeTypeModel.Serialize(ms, this);
            return ms;
        }

        public void Deserialize(Stream stream)
        {
            RuntimeTypeModel runtimeTypeModel = TypeModel.Create();
            runtimeTypeModel.AutoCompile = false;
            runtimeTypeModel.AutoAddMissingTypes = true;
            runtimeTypeModel.UseImplicitZeroDefaults = true;
            runtimeTypeModel.AutoAddProtoContractTypesOnly = true;

            runtimeTypeModel.Deserialize(stream, this, typeof(MeshDataStructure));
        }
    }
}