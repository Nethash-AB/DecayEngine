namespace DecayEngine.ModuleSDK.Capability
{
    public interface IHierarchical<TChild> : IChildBearer<TChild>, IParentable<TChild>
        where TChild : IHierarchical<TChild>
    {
    }
}