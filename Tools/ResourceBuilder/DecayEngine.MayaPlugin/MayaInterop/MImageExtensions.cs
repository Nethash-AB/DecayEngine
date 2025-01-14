using System;
using System.Runtime.InteropServices;
using Autodesk.Maya.OpenMaya;

namespace DecayEngine.MayaPlugin.MayaInterop
{
    public static class MImageExtensions
    {
        public static byte[] GetPixels(this MImage mayaImage)
        {
            uint depth = mayaImage.depth;
            mayaImage.getSize(out uint width, out uint height);
            IntPtr pixelsPointer = GetPixelsInternal(MImage.getCPtr(mayaImage));

            byte[] pixels = new byte[width * height * depth];
            Marshal.Copy(pixelsPointer, pixels, 0, pixels.Length);

            return pixels;
        }

        [DllImport("swigfiles.dll", EntryPoint = "CSharp_MImage_pixels")]
        private static extern IntPtr GetPixelsInternal(HandleRef jarg1);
    }
}