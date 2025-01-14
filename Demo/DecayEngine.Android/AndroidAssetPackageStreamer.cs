using System;
using System.IO;
using Android.Content.Res;
using DecayEngine.DecPakLib.Loader;

namespace DecayEngine.Android
{
    public class AndroidAssetPackageStreamer : IPackageStreamer
    {
        public bool CanRead => true;
        public bool CanWrite => false;
        public bool CanSeek => false;

        private readonly AssetManager _assetManager;

        public AndroidAssetPackageStreamer(AssetManager assetManager)
        {
            _assetManager = assetManager;
        }

        public Stream Read(Uri relativeUri)
        {
            return _assetManager.Open(relativeUri.ToString());
        }

        public Stream Write(Uri relativeUri)
        {
            throw new NotSupportedException("Writting to android asset storage is not supported.");
        }
    }
}