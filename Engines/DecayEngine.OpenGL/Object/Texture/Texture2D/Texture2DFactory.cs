using System;
using System.IO;
using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Engine.Render;

namespace DecayEngine.OpenGL.Object.Texture.Texture2D
{
    public static class Texture2DFactory
    {
        public static T Create<T>(Texture2DResource resource)
            where T : Texture2D
        {
            if (resource == null) return null;

            TextureDataStructure textureDataStructure = new TextureDataStructure();
            using (MemoryStream ms = resource.Source.GetData())
            {
                textureDataStructure.Deserialize(ms);
            }

            T component =
                (T) Activator.CreateInstance(typeof(T), textureDataStructure);

            component.Resource = resource;
            return component;
        }
    }
}