using System;
using System.Linq;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.MayaPlugin.Scene;
using DecayEngine.ResourceBuilderLib.Resource;

[assembly: MPxCommandClass(typeof(DecayEngine.MayaPlugin.ExportAsPrefabCommand), "decayExportSceneAsPrefab")]

namespace DecayEngine.MayaPlugin
{
    public class ExportAsPrefabCommand : MPxCommand,IMPxCommand
    {
        public override void doIt(MArgList argl)
        {
            MGlobal.displayInfo("Hello from Decay Engine.");

            MayaScene scene = new MayaScene();

            for (MItDependencyNodes dependencyIterator = new MItDependencyNodes(); !dependencyIterator.isDone; dependencyIterator.next())
            {
                scene.AddDependencyNode(dependencyIterator.thisNode);
            }

            Uri outputDirUri = new Uri(@"H:\Work\repositories\decay-engine\res\mayatest\");
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
    }
}