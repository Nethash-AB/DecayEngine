namespace DecayEngine.ModuleSDK.Capability
{
    public interface IScriptUpdateable : IActivable
    {
        void ScriptUpdate(float deltaTime);
    }
}