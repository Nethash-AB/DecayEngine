using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Collider
{
    [ProtoContract]
    public class Collider2DResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>();

        [ProtoMember(4)]
        public float Mass { get; set; }

        [ProtoMember(5)]
        public bool IsTrigger { get; set; }

        [ProtoMember(6)]
        public bool IsStatic { get; set; }

        [ProtoMember(7)]
        public bool IsKinematic { get; set; }

        [ProtoMember(8)]
        public int CollisionMask { get; set; }

        [ProtoMember(9)]
        public float Margin { get; set; }

        [ProtoMember(10)]
        public float FrictionLinear { get; set; }
        [ProtoMember(11)]
        public float FrictionSpinning { get; set; }
        [ProtoMember(12)]
        public float FrictionRolling { get; set; }

        [ProtoMember(13)]
        public Vector3Structure AnisotropicFriction { get; set; }

        [ProtoMember(14)]
        public float DampingLinear { get; set; }
        [ProtoMember(15)]
        public float DampingAngular { get; set; }

        [ProtoMember(16)]
        public float SleepingTheresholdLinear { get; set; }
        [ProtoMember(17)]
        public float SleepingTheresholdAngular { get; set; }

        [ProtoMember(18)]
        public float Restitution { get; set; }

        [ProtoMember(19)]
        public List<VertexStructure> HullVertices { get; set; }
    }
}