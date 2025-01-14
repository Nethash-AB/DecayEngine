using DecayEngine.DecPakLib;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IParentable<TParent>
    {
        TParent Parent { get; }
        ByReference<TParent> ParentByRef { get; }

        void SetParent(TParent parent);
        IParentable<TParent> AsParentable<T>() where T : TParent;
    }
}