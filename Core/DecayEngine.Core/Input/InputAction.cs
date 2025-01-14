using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.Core.Input.Triggers.GamePad;
using DecayEngine.Core.Input.Triggers.Keyboard;
using DecayEngine.DecPakLib.Math;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.Core.Input
{
    public class InputAction : IInputAction
    {
        private readonly List<IInputActionTrigger> _triggers;
        private bool _lastDigitalValue;
        private float _lastAnalogValue;

        public IEnumerable<IInputActionTrigger> Triggers => _triggers;

        public bool DigitalValue
        {
            get
            {
                if (_triggers.Count < 1) return false;

                return _triggers.OfType<IInputActionTriggerDigital>().Any(t => t.Value) ||
                       _triggers.OfType<IInputActionTriggerAnalog>().Any(t =>
                       {
                           if (t.DigitalActivationThereshold > 0)
                           {
                               return t.Value >= t.DigitalActivationThereshold;
                           }

                           if (t.DigitalActivationThereshold < 0)
                           {
                               return t.Value <= t.DigitalActivationThereshold;
                           }

                           return false;
                       });
            }
        }

        public float AnalogValue
        {
            get
            {
                if (_triggers.Count < 1) return 0f;

                List<IInputActionTriggerDigital> activatedDigitalTriggers = _triggers.OfType<IInputActionTriggerDigital>().Where(t => t.Value).ToList();

                float maxDigitalValue;
                float minDigitalValue;
                if (activatedDigitalTriggers.Count < 1)
                {
                    maxDigitalValue = 0f;
                    minDigitalValue = 0f;
                }
                else
                {
                    maxDigitalValue = activatedDigitalTriggers.Max(t => t.AnalogContribution);
                    minDigitalValue = activatedDigitalTriggers.Min(t => t.AnalogContribution);
                }

                List<IInputActionTriggerAnalog> analogTriggers = _triggers.OfType<IInputActionTriggerAnalog>().ToList();
                float maxAnalogValue;
                float minAnalogValue;
                if (analogTriggers.Count < 1)
                {
                    maxAnalogValue = 0f;
                    minAnalogValue = 0f;
                }
                else
                {
                    maxAnalogValue = analogTriggers.Max(t => t.Value);
                    minAnalogValue = analogTriggers.Min(t => t.Value);
                }

                float maxValue = maxDigitalValue > maxAnalogValue ? maxDigitalValue : maxAnalogValue;
                float minValue = minDigitalValue < minAnalogValue ? minDigitalValue : minAnalogValue;

                return Math.Abs(maxValue) > Math.Abs(minValue) ? maxValue : minValue;
            }
        }

        public event Action<IInputAction> OnDigitalActivate;
        public event Action<IInputAction> OnDigitalDeactivate;
        public event Action<IInputAction, bool> OnDigitalUpdate;
        public event Action<IInputAction, float> OnAnalogUpdate;

        public InputAction()
        {
            _triggers = new List<IInputActionTrigger>();
            _lastDigitalValue = false;
        }

        public IKeyboardActionTrigger AddKeyboardTrigger(KeyboardScanCode scanCode, float analogContribution)
        {
            IKeyboardActionTrigger trigger = _triggers
                .OfType<IKeyboardActionTrigger>()
                .FirstOrDefault(a => a.ScanCode == scanCode);
            if (trigger != null)
            {
                trigger.AnalogContribution = analogContribution;
                return trigger;
            }

            trigger = new KeyboardActionTrigger(scanCode, analogContribution);
            _triggers.Add(trigger);
            return trigger;
        }

        public IGamePadButtonActionTrigger AddGamePadButtonTrigger(GamePadButtonScanCode scanCode, int gamePadIndex, float analogContribution)
        {
            IGamePadButtonActionTrigger trigger = _triggers
                .OfType<IGamePadButtonActionTrigger>()
                .FirstOrDefault(a => a.ScanCode == scanCode && a.GamePadIndex == gamePadIndex);
            if (trigger != null)
            {
                trigger.AnalogContribution = analogContribution;
                return trigger;
            }

            trigger = new GamePadButtonActionTrigger(scanCode, gamePadIndex, analogContribution);
            _triggers.Add(trigger);
            return trigger;
        }

        public IGamePadAxisActionTrigger AddGamePadAxisTrigger
            (GamePadAxisScanCode scanCode, int gamePadIndex, float digitalActivationThereshold, float deadZone, bool invert)
        {
            IGamePadAxisActionTrigger trigger = _triggers
                .OfType<IGamePadAxisActionTrigger>()
                .FirstOrDefault(a => a.ScanCode == scanCode && a.GamePadIndex == gamePadIndex);
            if (trigger != null)
            {
                trigger.DigitalActivationThereshold = digitalActivationThereshold;
                return trigger;
            }

            trigger = new GamePadAxisActionTrigger(scanCode, gamePadIndex, digitalActivationThereshold, deadZone, invert);
            _triggers.Add(trigger);
            return trigger;
        }

        public void RemoveTrigger(IInputActionTrigger trigger)
        {
            if (_triggers.Contains(trigger))
            {
                _triggers.Remove(trigger);
            }
        }

        public void ClearTriggers()
        {
            _triggers.Clear();
        }

        public void Update()
        {
            bool newDigitalValue = DigitalValue;
            if (!_lastDigitalValue && newDigitalValue)
            {
                _lastDigitalValue = true;
                OnDigitalActivate?.Invoke(this);
            }
            else if (_lastDigitalValue && !newDigitalValue)
            {
                _lastDigitalValue = false;
                OnDigitalDeactivate?.Invoke(this);
            }
            OnDigitalUpdate?.Invoke(this, _lastDigitalValue);

            float newAnalogValue = AnalogValue;
            if (_lastAnalogValue.IsApproximately(newAnalogValue)) return;

            _lastAnalogValue = newAnalogValue;
            OnAnalogUpdate?.Invoke(this, _lastAnalogValue);
        }
    }
}