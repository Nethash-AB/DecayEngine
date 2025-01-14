using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Coroutine
{
    [ScriptExportEnum("CoroutineContext", "Represents the context a coroutine will run on.")]
    public enum CoroutineContextExport
    {
        [ScriptExportField("The couroutine should be run by the scripting engine.")]
        Script = 0,
        [ScriptExportField("The couroutine should be run by the rendering engine.")]
        Render = 1,
        [ScriptExportField("The couroutine should be run by the physics engine.")]
        Physics = 2,
        [ScriptExportField("The couroutine should be run by the sound engine.")]
        Sound = 3
    }
}