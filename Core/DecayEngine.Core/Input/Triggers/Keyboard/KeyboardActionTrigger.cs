using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.Core.Input.Triggers.Keyboard
{
    public class KeyboardActionTrigger : IKeyboardActionTrigger
    {
        public KeyboardScanCode ScanCode { get; }

        public bool Value { get; set; }
        public float AnalogContribution { get; set; }

        public KeyboardActionTrigger(KeyboardScanCode scanCode, float analogContribution)
        {
            ScanCode = scanCode;
            AnalogContribution = analogContribution > 1f ? 1f : analogContribution < -1f ? -1f : analogContribution;
        }
    }
}