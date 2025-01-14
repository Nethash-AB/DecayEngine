using System;
using System.IO;

namespace DecayEngine.DecPakLib.Loader
{
    public interface IPackageStreamer
    {
        bool CanRead { get; }
        bool CanWrite { get; }
        bool CanSeek { get; }

        Stream Read(Uri relativeUri);
        Stream Write(Uri relativeUri);
    }
}