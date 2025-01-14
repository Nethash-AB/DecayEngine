using System;
using System.IO;
using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;

namespace DecayEngine.OpenGL.Object.Texture.Texture3D
{
    public static class Texture3DFactory
    {
        public static T Create<T>(Texture3DResource resource)
            where T : Texture3D
        {
            if (resource == null) return null;

            TextureDataStructure textureDataStructureBack = new TextureDataStructure();
            using (MemoryStream ms = resource.SourceBack.GetData())
            {
                textureDataStructureBack.Deserialize(ms);
            }

            TextureDataStructure textureDataStructureDown = new TextureDataStructure();
            using (MemoryStream ms = resource.SourceDown.GetData())
            {
                textureDataStructureDown.Deserialize(ms);
            }

            TextureDataStructure textureDataStructureFront = new TextureDataStructure();
            using (MemoryStream ms = resource.SourceFront.GetData())
            {
                textureDataStructureFront.Deserialize(ms);
            }

            TextureDataStructure textureDataStructureLeft = new TextureDataStructure();
            using (MemoryStream ms = resource.SourceLeft.GetData())
            {
                textureDataStructureLeft.Deserialize(ms);
            }

            TextureDataStructure textureDataStructureRight = new TextureDataStructure();
            using (MemoryStream ms = resource.SourceRight.GetData())
            {
                textureDataStructureRight.Deserialize(ms);
            }

            TextureDataStructure textureDataStructureUp = new TextureDataStructure();
            using (MemoryStream ms = resource.SourceUp.GetData())
            {
                textureDataStructureUp.Deserialize(ms);
            }

            T component = (T) Activator.CreateInstance(typeof(T), new object[]
                {
                    new[]
                    {
                        textureDataStructureRight,
                        textureDataStructureLeft,
                        textureDataStructureUp,
                        textureDataStructureDown,
                        textureDataStructureFront,
                        textureDataStructureBack
                    }
                }
            );

            component.Resource = resource;
            return component;
        }
    }
}