using System;
using System.IO;
using DecayEngine.DecPakLib.Bundle;

namespace DecayEngine.ResourceBuilderLib.Resource
{
    public static class ResourceLoadingController
    {
        public static ResourceBundle Load(Uri inputBundleFilePath, TextWriter logStream = null)
        {
            if (logStream == null)
            {
                logStream = Console.Out;
            }

            if (!inputBundleFilePath.IsAbsoluteUri)
            {
                inputBundleFilePath = new Uri(new Uri(Directory.GetCurrentDirectory()), inputBundleFilePath);
            }

            logStream.WriteLine($"Input file resolved to: {inputBundleFilePath}.");
            logStream.WriteLine();

            logStream.WriteLine($"Loading resource bundle: {inputBundleFilePath}.");
            using FileStream fs = File.OpenRead(inputBundleFilePath.LocalPath);
            ResourceBundle bundle = ResourceBundle.Deserialize(fs);

            return bundle;
        }
    }
}