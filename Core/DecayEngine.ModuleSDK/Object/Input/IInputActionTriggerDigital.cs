namespace DecayEngine.ModuleSDK.Object.Input
{
    public interface IInputActionTriggerDigital : IInputActionTrigger
    {
        bool Value { get; set; }
        float AnalogContribution { get; set; }
    }
}