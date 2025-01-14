using System.Collections.Generic;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.Texture;
using DecayEngine.OpenGL.Object.Texture;

namespace DecayEngine.OpenGL
{
    public static class OpenGlGlobalState
    {
        public static Dictionary<TextureTargets, ITexture> TextureTargetState = new Dictionary<TextureTargets, ITexture>
        {
            {TextureTargets.Color, null},
            {TextureTargets.Normal, null},
            {TextureTargets.FrameBufferColor, null},
            {TextureTargets.FrameBufferDepthStencil, null}
        };
    }
}