using System;
using System.IO;
using DecayEngine.DecPakLib.Loader;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Package
{
    [ProtoContract(AsReferenceDefault = true)]
    public class DataPackage
    {
        [ProtoMember(1)]
        public Uri RelativePath { get; set; }
        [ProtoMember(2)]
        public long Size { get; set; }
        [ProtoIgnore]
        public bool Finalized { get; set; }

        [ProtoIgnore]
        public IPackageStreamer PackageStreamer { get; set; }

        public Stream Read()
        {
            if (PackageStreamer == null || !PackageStreamer.CanRead) return null;

            return PackageStreamer.Read(RelativePath);
        }

        public Stream Write()
        {
            if (PackageStreamer == null || !PackageStreamer.CanWrite) return null;

            return PackageStreamer.Write(RelativePath);
        }
    }
}