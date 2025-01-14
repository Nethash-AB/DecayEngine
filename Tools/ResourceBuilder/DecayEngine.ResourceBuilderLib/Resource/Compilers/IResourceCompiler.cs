using System.Collections.Generic;
using System.IO;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;

namespace DecayEngine.ResourceBuilderLib.Resource.Compilers
{
    public interface IResourceCompiler
    {
        Stream Compile(IResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers);
        Stream Decompile(IResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers);
    }

    public interface IResourceCompiler<in TResource> : IResourceCompiler
        where TResource : IResource
    {
        Stream Compile(TResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers);
        Stream Decompile(TResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers);
    }
}