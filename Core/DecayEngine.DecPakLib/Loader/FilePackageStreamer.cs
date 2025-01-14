using System;
using System.IO;

namespace DecayEngine.DecPakLib.Loader
{
    public class FilePackageStreamer : IPackageStreamer
    {
        public bool CanRead => true;
        public bool CanWrite => true;
        public bool CanSeek => true;

        private readonly Uri _baseUri;

        public FilePackageStreamer(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Stream Read(Uri relativeUri)
        {
            return File.OpenRead(new Uri(_baseUri, relativeUri).LocalPath);
        }

        public Stream Write(Uri relativeUri)
        {
            return File.OpenWrite(new Uri(_baseUri, relativeUri).LocalPath);
        }
    }
}