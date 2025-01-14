using System.Collections.Generic;
using System.IO;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;

namespace DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.SoundBank
{
    public class SoundBankCompiler : IResourceCompiler<SoundBankResource>
    {
        public Stream Compile(IResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            if (!(resource is SoundBankResource specificResource)) return null;
            return Compile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Decompile(IResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            if (!(resource is SoundBankResource specificResource)) return null;
            return Decompile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Compile(SoundBankResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            return sourceStream;
        }

        public Stream Decompile(SoundBankResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            return sourceStream;
        }
    }
}