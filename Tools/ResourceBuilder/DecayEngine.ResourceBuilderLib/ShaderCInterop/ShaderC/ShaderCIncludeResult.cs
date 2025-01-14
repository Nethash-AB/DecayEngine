using System;

namespace DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderC
{
    public struct ShaderCIncludeResult
    {
        public string SourceName;
        public int SourceNameLength;
        public string Content;
        public int ContentLength;
        public IntPtr UserData;
    }
}