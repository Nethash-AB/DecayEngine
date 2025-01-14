using System;
using System.IO;
using System.Runtime.InteropServices;
using DecayEngine.ModuleSDK.Engine.Render;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace DecayEngine.Standalone
{
    public class Rgba32Icon : ISurfaceIcon
    {
        private GCHandle _gcHandle;
        public IntPtr Handle { get; }
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public int Pitch { get; }

        public Rgba32Icon(Stream iconStream)
        {
            iconStream.Seek(0, SeekOrigin.Begin);
            Image<Rgba32> icon = Image.Load(iconStream);
            iconStream.Close();

            Width = icon.Width;
            Height = icon.Height;
            Depth = icon.PixelType.BitsPerPixel;
            Pitch = icon.GetPixelRowSpan(0).Length * (icon.PixelType.BitsPerPixel / 8);

            byte[] iconBytes = MemoryMarshal.AsBytes(icon.GetPixelSpan()).ToArray();
            _gcHandle = GCHandle.Alloc(iconBytes, GCHandleType.Pinned);
            Handle = _gcHandle.AddrOfPinnedObject();
        }

        ~Rgba32Icon()
        {
            ReleaseUnmanagedResources();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            _gcHandle.Free();
        }
    }
}