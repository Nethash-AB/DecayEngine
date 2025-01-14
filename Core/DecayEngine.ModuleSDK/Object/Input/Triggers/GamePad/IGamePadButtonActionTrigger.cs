namespace DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad
{
    public interface IGamePadButtonActionTrigger : IInputActionTriggerDigital
    {
        GamePadButtonScanCode ScanCode { get; }
        int GamePadIndex { get; }
    }
}