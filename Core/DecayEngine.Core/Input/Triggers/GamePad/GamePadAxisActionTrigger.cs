using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;

namespace DecayEngine.Core.Input.Triggers.GamePad
{
    public class GamePadAxisActionTrigger : IGamePadAxisActionTrigger
    {
        public GamePadAxisScanCode ScanCode { get; }
        public int GamePadIndex { get; }

        public float Value { get; set; }
        public float DigitalActivationThereshold { get; set; }
        public float DeadZone { get; set; }
        public bool Invert { get; set; }

        public GamePadAxisActionTrigger(GamePadAxisScanCode scanCode, int gamePadIndex, float digitalActivationThereshold, float deadZone, bool invert)
        {
            ScanCode = scanCode;
            GamePadIndex = gamePadIndex;
            DigitalActivationThereshold = digitalActivationThereshold > 1f ? 1f : digitalActivationThereshold < -1f ? -1f : digitalActivationThereshold;
            DeadZone = deadZone > 1f ? 1f : deadZone < 0f ? 0f : deadZone;
            Invert = invert;
        }
    }
}