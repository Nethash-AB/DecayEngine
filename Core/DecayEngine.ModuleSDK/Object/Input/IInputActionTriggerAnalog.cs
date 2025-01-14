namespace DecayEngine.ModuleSDK.Object.Input
{
    public interface IInputActionTriggerAnalog : IInputActionTrigger
    {
        float Value { get; set; }
        float DigitalActivationThereshold { get; set; }
        float DeadZone { get; set; }
        bool Invert { get; set; }
    }
}