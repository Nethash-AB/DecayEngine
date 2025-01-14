using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Bundle
{
    [ProtoContract]
    public class ResourceBundle
    {
        [ProtoMember(1)]
        public List<IResource> Resources { get; set; }
        [ProtoMember(2)]
        public TargetPlatform SupportedPlatforms { get; set; }

        [ProtoIgnore]
        public IEnumerable<ByReference<DataPointer>> DataPointers => Resources
            .OfType<IRootResource>()
            .SelectMany(res => res.Pointers)
            .Where(dataPointer => dataPointer != null)
            .GroupBy(dataPointer => dataPointer().SourcePath)
            .Select(g => g.FirstOrDefault());

        [ProtoIgnore]
        public IEnumerable Packages => DataPointers
            .Select(dataPointer => dataPointer?.Invoke()?.Package)
            .Where(package => package != null)
            .GroupBy(package => package.RelativePath)
            .Select(g => g.FirstOrDefault());

        public void Serialize(Stream stream)
        {
            Serialize(stream, this);
        }

        public static void Serialize(Stream stream, ResourceBundle bundle)
        {
            Serializer.Serialize(stream, bundle);
        }

        public static ResourceBundle Deserialize(Stream stream)
        {
            return Serializer.Deserialize<ResourceBundle>(stream);
        }
    }
}