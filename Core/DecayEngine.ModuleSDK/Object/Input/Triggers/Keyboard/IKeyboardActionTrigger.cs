namespace DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard
{
    public interface IKeyboardActionTrigger : IInputActionTriggerDigital
    {
        KeyboardScanCode ScanCode { get; }
    }
}