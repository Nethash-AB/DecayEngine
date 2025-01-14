namespace DecayEngine.ModuleSDK.Component.Light
{
    public interface ISpotLight : ILight
    {
        float Radius { get; set; }
        float CutoffAngle { get; set; }
    }
}