using System;
using System.IO;
using DecayEngine.DecPakLib.Loader;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DecayEngine.Demo.Xbox
{
    public class WinRtAssetPackageStreamer : IPackageStreamer
    {
        public bool CanRead => true;
        public bool CanWrite => false;
        public bool CanSeek => false;

        private readonly Uri _baseUri;

        public WinRtAssetPackageStreamer(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Stream Read(Uri relativeUri)
        {
            Uri fullUri = new Uri(_baseUri, relativeUri);
            StorageFile resourceFile = StorageFile.GetFileFromApplicationUriAsync(fullUri).AsTask().Result;
            IRandomAccessStreamWithContentType resourceWinStream = resourceFile.OpenReadAsync().AsTask().Result;
            return resourceWinStream.GetInputStreamAt(0).AsStreamForRead();
        }

        public Stream Write(Uri relativeUri)
        {
            throw new NotSupportedException("Writting to winrt asset storage is not supported.");
        }
    }
}
