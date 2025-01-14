using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.ResourceBuilderLib.Resource.Serializers;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Property.Reference;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Filter;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Terminator;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Initiator;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Single;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Statement.Component;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Statement.GameObject;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.AnimatedMaterial;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Collider;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Font;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Mesh;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.PbrMaterial;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.PostProcessing;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Prefab;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Scene;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Script;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Shader;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.ShaderProgram;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Sound;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.SoundBank;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Texture2D;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Texture3D;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Common.PropertySheet;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Component.Script;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math;
using DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Transform;

namespace DecayEngine.ResourceBuilderLib.Resource
{
    public static class ResourceSerializationController
    {
        private static readonly List<IResourceSerializer> Serializers;

        static ResourceSerializationController()
        {
            Serializers = new List<IResourceSerializer>
            {
                new ResourceReferenceExpressionSerializer(),
                new FilterByComponentTypeSerializer(),
                new FilterByNameSerializer(),
                new SelectChildrenSerializer(),
                new SelectComponentsSerializer(),
                new SelectFirstTerminatorSerializer(),
                new SelectAllTerminatorSerializer(),
                new SelectFrameBufferTerminatorSerializer(),
                new SelectActiveSceneSerializer(),
                new SelectGlobalSerializer(),
                new SelectThisSerializer(),
                new SelectRootSerializer(),
                new SelectParentSerializer(),
                new CreateComponentSerializer(),
                new CreateGameObjectSerializer(),
                new Collider2DSerializer(),
                new FontSerializer(),
                new AnimatedMaterialSerializer(),
                new PrefabSerializer(),
                new SceneSerializer(),
                new ScriptSerializer(),
                new ShaderSerializer(),
                new ShaderProgramSerializer(),
                new SoundSerializer(),
                new SoundBankSerializer(),
                new Texture2DSerializer(),
                new Texture3DSerializer(),
                new AnimationFrameSerializer(),
                new TriangleSerializer(),
                new ColorSerializer(),
                new Vector2Serializer(),
                new Vector3Serializer(),
                new Vector4Serializer(),
                new VertexSerializer(),
                new TransformSerializer(),
                new ScriptInjectionSerializer(),
                new PostProcessingPresetSerializer(),
                new PostProcessingStageSerializer(),
                new PropertySheetSerializer(),
                new MeshSerializer(),
                new PbrMaterialSerializer()
            };
        }

        public static IResourceSerializer<TResource> GetSerializer<TResource>()
            where TResource : IResource
        {
            return (IResourceSerializer<TResource>) Serializers.FirstOrDefault(value => value is IResourceSerializer<TResource>);
        }

        public static IResourceSerializer GetSerializer(IResource resource)
        {
            return (from serializer in Serializers
                from iface in serializer.GetType().GetInterfaces()
                where iface.IsGenericType
                    let genericArg = iface.GetGenericArguments().FirstOrDefault()
                    where genericArg != null && genericArg.IsInstanceOfType(resource)
                        select serializer).FirstOrDefault();
        }

        public static IResourceSerializer GetSerializer(string tag)
        {
            return Serializers.FirstOrDefault(seializer => seializer.Tag == tag);
        }

        public static ResourceBundle Deserialize(Uri baseDirectory, TargetPlatform targetPlatforms, TextWriter logStream = null)
        {
            if (logStream == null)
            {
                logStream = Console.Out;
            }

            if (!baseDirectory.IsAbsoluteUri)
            {
                baseDirectory = new Uri(new Uri($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}", UriKind.Absolute), baseDirectory);
            }

            if (Path.GetExtension(baseDirectory.LocalPath) != "")
            {
                baseDirectory = new Uri(Path.GetDirectoryName(baseDirectory.LocalPath));
            }
            else if (Path.GetExtension(baseDirectory.LocalPath) == "" &&
                     baseDirectory.LocalPath[baseDirectory.LocalPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                baseDirectory = new Uri($"{baseDirectory.LocalPath}{Path.DirectorySeparatorChar}");
            }

            if (!Directory.Exists(baseDirectory.LocalPath))
            {
                throw new DirectoryNotFoundException($"The directory {baseDirectory} was not found or is not a directory.");
            }

            logStream.WriteLine($"Base directory resolved to: {baseDirectory}.");

            logStream.WriteLine();

            List<Uri> resourcePaths = new List<Uri>();
            Regex regex = new Regex(@"\\[\.].*");
            foreach (Uri dir in Directory
                .GetDirectories(baseDirectory.LocalPath, "*", SearchOption.AllDirectories)
                .Select(path => new Uri(path, UriKind.Absolute)))
            {
                if (regex.IsMatch(dir.LocalPath)) continue;

                foreach (Uri metaFileUri in Directory
                    .GetFiles(dir.LocalPath, "*.meta", SearchOption.TopDirectoryOnly)
                    .Select(path => new Uri(path, UriKind.Absolute)))
                {
                    logStream.WriteLine($"Meta file found at: {metaFileUri}.");
                    resourcePaths.Add(metaFileUri);
                }
            }

            logStream.WriteLine();

            if (resourcePaths.Count < 1)
            {
                throw new Exception($"No resource descriptors found at path {baseDirectory}.");
            }

            XDocument globalDocument = new XDocument();
            XElement globalResourcesElement = new XElement("resources");
            globalDocument.Add(globalResourcesElement);

            foreach (Uri path in resourcePaths)
            {
                logStream.WriteLine($"Reading meta file: {path}.");
                using TextReader reader = File.OpenText(path.LocalPath);

                XDocument document = XDocument.Load(reader);

                XElement root = document.Element("resources");
                if (root == null) continue;

                foreach (XElement element in root.Elements())
                {
                    element.SetAttributeValue("filePath", baseDirectory.MakeRelativeUri(path).ToString());
                    globalResourcesElement.Add(element);
                }
            }

            logStream.WriteLine();

            ResourceBundle resourceBundle = new ResourceBundle
            {
                Resources = new List<IResource>()
            };

            logStream.WriteLine("Deserializing meta files.");
            foreach (XElement child in globalResourcesElement.Elements())
            {
                IResourceSerializer serializer = GetSerializer(child.Name.ToString());
                if (serializer == null || !serializer.BuildsType<IRootResource>()) continue; // Only root elements can be at the root

                TargetPlatform resourceTargetPlatforms = ParseTargetPlatforms(child);
                if (resourceTargetPlatforms != TargetPlatform.NotSet && (resourceTargetPlatforms & targetPlatforms) == 0)
                {
                    logStream.WriteLine($"Skipping resource ({child.Attribute("filePath")}) " +
                                        $"because none of its target platforms ({resourceTargetPlatforms.ToString()}) were selected for building.");
                    continue;
                }

                IRootResource resource = (IRootResource) serializer.Deserialize(child);
                if (resource == null)
                {
                    throw new Exception($"Error deserializing resource ({child.Attribute("filePath")}), aborting.");
                }

                if (resourceBundle.Resources.Contains(resource)) continue;

                resource.TargetPlatforms = resourceTargetPlatforms;
                logStream.WriteLine($"Resource deserialized: {resource.Id} ({resource.MetaFilePath}).");
                resourceBundle.Resources.Add(resource);
            }

            return resourceBundle;
        }

        public static void Serialize(ResourceBundle root, Uri outputDirectory, TextWriter logStream = null)
        {
            if (logStream == null)
            {
                logStream = Console.Out;
            }

            if (!outputDirectory.IsAbsoluteUri)
            {
                outputDirectory = new Uri(new Uri(Directory.GetCurrentDirectory()), outputDirectory);
            }

            if (Path.GetExtension(outputDirectory.LocalPath) != "")
            {
                outputDirectory = new Uri(Path.GetDirectoryName(outputDirectory.LocalPath));
            }
            else if (Path.GetExtension(outputDirectory.LocalPath) == "" &&
                     outputDirectory.LocalPath[outputDirectory.LocalPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                outputDirectory = new Uri($"{outputDirectory.LocalPath}{Path.DirectorySeparatorChar}");
            }

            logStream.WriteLine($"Output directory resolved to: {outputDirectory}.");

            logStream.WriteLine();

            Dictionary<Uri, ICollection<XElement>> output = new Dictionary<Uri, ICollection<XElement>>();

            logStream.WriteLine("Serializing resources.");
            foreach (IRootResource resource in root.Resources.OfType<IRootResource>())
            {
                logStream.WriteLine($"Processing resource: {resource.Id}.");
                IResourceSerializer serializer = GetSerializer(resource);

                logStream.WriteLine($"Serializing resource: {resource.Id}.");
                XElement element = serializer?.Serialize(resource);
                if (element == null)
                {
                    logStream.WriteLine("Error serializing resource, skipping.");
                    continue;
                }

                if (resource.TargetPlatforms != TargetPlatform.NotSet)
                {
                    element.SetAttributeValue("platforms", resource.TargetPlatforms.ToString().ToLower());
                }

                Uri metaFilePath = new Uri(resource.MetaFilePath, UriKind.Relative);
                if (!output.ContainsKey(metaFilePath))
                {
                    output[metaFilePath] = new List<XElement>();
                }
                output[metaFilePath].Add(element);

                logStream.WriteLine();
            }

            logStream.WriteLine("Writting resource meta files.");
            foreach (KeyValuePair<Uri, ICollection<XElement>> kv in output)
            {
                Uri uri = kv.Key;
                ICollection<XElement> xmlElements = kv.Value;

                Uri outputFilePath = new Uri(outputDirectory, uri);

                logStream.WriteLine($"Processing meta file: {outputFilePath}.");

                if (File.Exists(outputFilePath.LocalPath))
                {
                    File.Delete(outputFilePath.LocalPath);
                }

                string directoryPath = Path.GetDirectoryName(outputFilePath.LocalPath);
                if (!Directory.Exists(directoryPath))
                {
                    logStream.WriteLine($"Creating missing directory: {new Uri(directoryPath, UriKind.Absolute)}.");
                    Directory.CreateDirectory(directoryPath);
                }

                string disclaimer = "Decay Engine resource metadata file." +
                                    "\n" +
                                    "\nThis file was created by a tool." +
                                    $"\nResourceCompiler Version: {Assembly.GetEntryAssembly()?.GetName().Version.ToString() ?? "0.0.0.0"}";

                XDocument document = new XDocument();

                XComment disclaimerComment = new XComment(disclaimer);
                document.Add(disclaimerComment);

                XDeclaration declaration = new XDeclaration("1.0", "utf-8", null);
                document.Declaration = declaration;

                XElement globalResourceElement = new XElement("resources");
                document.Add(globalResourceElement);

                foreach (XElement element in xmlElements)
                {
                    globalResourceElement.Add(element);
                }

                logStream.WriteLine($"Writting meta file: {outputFilePath}.");
                using TextWriter writer = new StreamWriter(outputFilePath.LocalPath, false);
                using XmlWriter xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "    ",
                    Encoding = Encoding.UTF8
                });
                document.Save(xmlWriter);

                logStream.WriteLine();
            }
        }

        private static TargetPlatform ParseTargetPlatforms(XElement element)
        {
            XAttribute attribute = element.Attribute("platforms");
            if (attribute == null || string.IsNullOrEmpty(attribute.Value)) return TargetPlatform.NotSet;

            TargetPlatform targetPlatforms = TargetPlatform.NotSet;
            foreach (string platform in attribute.Value.Split(';'))
            {
                if (!Enum.TryParse(platform, true, out TargetPlatform result)) continue;

                targetPlatforms |= result;
            }

            return targetPlatforms == TargetPlatform.NotSet ? TargetPlatform.NotSet : targetPlatforms;
        }
    }
}