using System;
using System.IO;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.MayaPlugin.Scene;
using DecayEngine.ResourceBuilderLib.Resource;

[assembly: MPxFileTranslatorClass(
    typeof(DecayEngine.MayaPlugin.PrefabFileTranslator),
    "Decay Engine Prefab",
    null,
    DecayEngine.MayaPlugin.PrefabFileTranslator.OptionScript,
    DecayEngine.MayaPlugin.PrefabFileTranslator.DefaultOptions)]

namespace DecayEngine.MayaPlugin
{
    public class PrefabFileTranslator : MPxFileTranslator
    {
        public const string OptionScript = "decayPrefabExportOptions";
        public const string DefaultOptions = "";

        public override bool haveReadMethod()
        {
            return false;
        }

        public override bool haveWriteMethod()
        {
            return true;
        }

        public override bool canBeOpened()
        {
            return false;
        }

        public override string defaultExtension()
        {
            return "meta";
        }

        public override void writer(MFileObject file, string options, FileAccessMode mode)
        {
            string outDirPath = Path.GetDirectoryName(file.fullName);
            if (string.IsNullOrEmpty(outDirPath) || !Directory.Exists(outDirPath))
            {
                MGlobal.displayError($"Invalid output path: {outDirPath}.");
                return;
            }

            string fileName = Path.GetFileName(file.fullName);
            Uri outputDirUri = new Uri(outDirPath + Path.DirectorySeparatorChar);

            switch (mode)
            {
                case FileAccessMode.kExportAccessMode:
                    ExportAll(fileName, outputDirUri);
                    break;
                case FileAccessMode.kExportActiveAccessMode:
                    ExportSelected(fileName, outputDirUri);
                    break;
            }
        }

        private static void ExportAll(string fileName, Uri outputDirUri)
        {
            MayaScene scene = new MayaScene(fileName);

            for (MItDependencyNodes dependencyIterator = new MItDependencyNodes(); !dependencyIterator.isDone; dependencyIterator.next())
            {
                scene.AddDependencyNode(dependencyIterator.thisNode);
            }

            ResourceBundle bundle = scene.Build(outputDirUri);

            if (scene.Children.Count < 1 || bundle.Resources.Count < 1)
            {
                MGlobal.displayError("Scene contains no valid objects, aborting.");
                return;
            }

            MGlobal.displayInfo("Parsed scene hierarchy:");
            MGlobal.displayInfo($"{scene.ToSceneString()}");

            MGlobal.displayInfo($"Serializing as prefab to directory: {outputDirUri.LocalPath}.");
            ResourceSerializationController.Serialize(bundle, outputDirUri);
        }

        private static void ExportSelected(string fileName, Uri outputDirUri)
        {
            throw new NotImplementedException();
        }
    }
}