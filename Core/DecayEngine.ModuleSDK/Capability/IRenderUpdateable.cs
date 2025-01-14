namespace DecayEngine.ModuleSDK.Capability
{
    public interface IRenderUpdateable : IActivable
    {
        void RenderUpdate(float deltaTime);
    }
}