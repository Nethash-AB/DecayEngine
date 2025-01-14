using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.MayaPlugin.MayaInterop;
using TeximpNet;
using TeximpNet.Compression;

namespace DecayEngine.MayaPlugin.Scene.Node
{
    public class MayaTextureNode : IMayaNode
    {
        public MayaScene Scene { get; set; }
        public MObject MayaObject { get; }
        public List<IMayaNode> Children { get; }

        public MayaTextureNode(MObject mayaObject)
        {
            MayaObject = mayaObject;
            Children = new List<IMayaNode>();
        }

        public bool IsValid => Children.Count < 1;

        public string DisplayId
        {
            get
            {
                MFnDependencyNode dependencyNodeFn = new MFnDependencyNode(MayaObject);
                return $"{dependencyNodeFn.name} [{GetType().Name}({dependencyNodeFn.typeName})]";
            }
        }

        public IEnumerable<IResource> ToDecayResource(Uri outputDirUri)
        {
            List<IResource> resources = new List<IResource>();

            Uri textureDataOutputDirUri = new Uri(outputDirUri, new Uri($"textures{Path.DirectorySeparatorChar}", UriKind.Relative));
            Texture2DResource textureResource = ExportDecayTexture(textureDataOutputDirUri);

            resources.Add(textureResource);

            return resources;
        }

        private Texture2DResource ExportDecayTexture(Uri outputDirUri)
        {
            if (!Directory.Exists(outputDirUri.LocalPath))
            {
                Directory.CreateDirectory(outputDirUri.LocalPath);
            }

            MFnDependencyNode dependencyNodeFn = new MFnDependencyNode(MayaObject);

            MPlug fileTextureNamePlug = dependencyNodeFn.findPlug("ftn");
            string fileTexturePath = fileTextureNamePlug.asStringProperty;
            if (!File.Exists(fileTexturePath))
            {
                MGlobal.displayError($"Texture ({dependencyNodeFn.name} does not exist. Expected path: {fileTexturePath}.");
                return null;
            }

            string textureFileName = $"{dependencyNodeFn.name}.dds";
            Uri textureDataUri = new Uri(outputDirUri, new Uri(textureFileName, UriKind.Relative));

            MImage mayaImage = new MImage();
            mayaImage.readFromTextureNode(MayaObject);

            mayaImage.getSize(out uint w, out uint h);
            int depth = (int) mayaImage.depth;
            int width = (int) w;
            int height = (int) h;

            string textureDataPath = textureDataUri.LocalPath;
            MGlobal.displayInfo($"Serializing Texture ({dependencyNodeFn.name} to file: {textureDataPath}):");

            GCHandle pixelsHandle = GCHandle.Alloc(mayaImage.GetPixels(), GCHandleType.Pinned);
            Surface textureSurface =
                Surface.LoadFromRawData(pixelsHandle.AddrOfPinnedObject(), width, height, width * depth, false, true);
            pixelsHandle.Free();

            if (File.Exists(textureDataPath))
            {
                File.Delete(textureDataPath);
            }

            using (Compressor compressor = new Compressor())
            {
                compressor.Compression.Format = CompressionFormat.BC3;
                compressor.Compression.Quality = CompressionQuality.Production;
                compressor.Input.SetData(textureSurface);
                compressor.Input.SetMipmapGeneration(true);

                using FileStream fs = File.OpenWrite(textureDataPath);
                compressor.Process(fs);
            }

            return new Texture2DResource
            {
                Id = dependencyNodeFn.name,
                MetaFilePath = $"{Path.GetFileNameWithoutExtension(Scene.MetaFileName)}_textures.meta",
                Source = new DataPointer
                {
                    SourcePath = $"./textures/{textureFileName}"
                }
            };
        }
    }
}