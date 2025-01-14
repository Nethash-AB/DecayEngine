using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;

namespace DecayEngine.Core.Input.Triggers.GamePad
{
    public class GamePadButtonActionTrigger : IGamePadButtonActionTrigger
    {
        public GamePadButtonScanCode ScanCode { get; }
        public int GamePadIndex { get; }

        public bool Value { get; set; }
        public float AnalogContribution { get; set; }

        public GamePadButtonActionTrigger(GamePadButtonScanCode scanCode, int gamePadIndex, float analogContribution)
        {
            ScanCode = scanCode;
            GamePadIndex = gamePadIndex;
            AnalogContribution = analogContribution > 1f ? 1f : analogContribution < -1f ? -1f : analogContribution;
        }
    }
}