using Autodesk.Maya.OpenMaya;

[assembly: ExtensionPlugin(typeof(DecayEngine.MayaPlugin.DecayEnginePlugin), "Any")]

namespace DecayEngine.MayaPlugin
{
    public class DecayEnginePlugin : IExtensionPlugin
    {
        public bool InitializePlugin()
        {
            return true;
        }

        public bool UninitializePlugin()
        {
            return true;
        }

        public string GetMayaDotNetSdkBuildVersion()
        {
            const string version = "201353";
            return version;
        }
    }
}