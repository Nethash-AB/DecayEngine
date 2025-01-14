namespace DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad
{
    public interface IGamePadAxisActionTrigger : IInputActionTriggerAnalog
    {
        GamePadAxisScanCode ScanCode { get; }
        int GamePadIndex { get; }
    }
}