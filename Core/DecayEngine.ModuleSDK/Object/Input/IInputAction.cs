using System;
using System.Collections.Generic;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.ModuleSDK.Object.Input
{
    public interface IInputAction
    {
        IEnumerable<IInputActionTrigger> Triggers { get; }

        bool DigitalValue { get; }
        float AnalogValue { get; }

        event Action<IInputAction> OnDigitalActivate;
        event Action<IInputAction> OnDigitalDeactivate;
        event Action<IInputAction, bool> OnDigitalUpdate;
        event Action<IInputAction, float> OnAnalogUpdate;

        IKeyboardActionTrigger AddKeyboardTrigger(KeyboardScanCode scanCode, float analogContribution);
        IGamePadButtonActionTrigger AddGamePadButtonTrigger(GamePadButtonScanCode scanCode, int gamePadIndex, float analogContribution);

        IGamePadAxisActionTrigger AddGamePadAxisTrigger
            (GamePadAxisScanCode scanCode, int gamePadIndex, float digitalActivationThereshold, float deadZone, bool invert);
        void RemoveTrigger(IInputActionTrigger trigger);
        void ClearTriggers();
        void Update();
    }
}