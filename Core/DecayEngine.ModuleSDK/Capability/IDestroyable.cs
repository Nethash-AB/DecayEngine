namespace DecayEngine.ModuleSDK.Capability
{
    public interface IDestroyable
    {
        bool Destroyed { get; }
        void Destroy();
    }
}