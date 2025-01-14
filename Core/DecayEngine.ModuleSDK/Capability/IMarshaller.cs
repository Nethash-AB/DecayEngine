namespace DecayEngine.ModuleSDK.Capability
{
    public interface IMarshaller
    {
        object MarshalTo(object obj);
        T MarshalFrom<T>(object obj) where T : class, new();
    }
}