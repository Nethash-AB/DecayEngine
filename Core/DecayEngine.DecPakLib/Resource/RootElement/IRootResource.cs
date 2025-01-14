using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.Collider;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.DecPakLib.Resource.RootElement.Scene;
using DecayEngine.DecPakLib.Resource.RootElement.Script;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.DecPakLib.Resource.RootElement.Sound;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement
{
    [ProtoContract]
    [ProtoInclude(2000, typeof(AnimatedMaterialResource))]
    [ProtoInclude(2001, typeof(PrefabResource))]
    [ProtoInclude(2002, typeof(SceneResource))]
    [ProtoInclude(2003, typeof(ScriptResource))]
    [ProtoInclude(2004, typeof(ShaderResource))]
    [ProtoInclude(2005, typeof(ShaderProgramResource))]
    [ProtoInclude(2006, typeof(SoundResource))]
    [ProtoInclude(2007, typeof(SoundBankResource))]
    [ProtoInclude(2008, typeof(Texture2DResource))]
    [ProtoInclude(2009, typeof(Texture3DResource))]
    [ProtoInclude(2010, typeof(Collider2DResource))]
    [ProtoInclude(2011, typeof(FontResource))]
    [ProtoInclude(2012, typeof(PostProcessingPresetResource))]
    [ProtoInclude(2013, typeof(MeshResource))]
    [ProtoInclude(2014, typeof(PbrMaterialResource))]
    public interface IRootResource : IResource
    {
        [ProtoMember(1)]
        string Id { get; set; }
        [ProtoMember(2)]
        string MetaFilePath { get; set; }
        [ProtoMember(3)]
        TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        List<ByReference<DataPointer>> Pointers { get; }
    }
}