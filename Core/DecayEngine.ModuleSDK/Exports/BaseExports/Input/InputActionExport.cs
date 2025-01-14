using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.Keyboard;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input
{
    public delegate void DigitalActivateDelegateExport(InputActionExport inputAction);
    public delegate void DigitalDeactivateDelegateExport(InputActionExport inputAction);
    public delegate void DigitalUpdateDelegateExport(InputActionExport inputAction, bool value);
    public delegate void AnalogUpdateDelegateExport(InputActionExport inputAction, float value);

    [ScriptExportClass("InputAction", "Represents an Input Action.")]
    public class InputActionExport : ExportableManagedObject<IInputAction>
    {
        public override int Type => (int) ManagedExportType.InputAction;

        [ScriptExportProperty("The name of the `InputAction`.")]
        public string Name { get; internal set; }

        [ScriptExportProperty("A list of triggers that the `InputAction` will listen for.")]
        public IEnumerable<InputActionTriggerExport> Triggers => Reference.Triggers.Select(t =>
        {
            return t switch
            {
                IGamePadAxisActionTrigger gamePadAxisActionTrigger =>
                    (InputActionTriggerExport) new GamePadAxisActionTriggerExport(gamePadAxisActionTrigger),
                IGamePadButtonActionTrigger gamePadButtonActionTrigger =>
                    (InputActionTriggerExport) new GamePadAxisActionTriggerExport(gamePadButtonActionTrigger),
                IKeyboardActionTrigger keyboardActionTrigger =>
                    (InputActionTriggerExport) new KeyboardActionTriggerExport(keyboardActionTrigger),
                _ => null
            };
        }).Where(t => t != null);

        [ScriptExportProperty("The current digital value of the `InputAction`.")]
        public bool DigitalValue => Reference.DigitalValue;

        [ScriptExportProperty("The current analog value of the `InputAction`.")]
        public float AnalogValue => Reference.AnalogValue;

        [ScriptExportProperty("An `EventHandler` that fires when the action is digitally activated.")]
        public EventExport<DigitalActivateDelegateExport> OnDigitalActivate { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the action is digitally deactivated.")]
        public EventExport<DigitalDeactivateDelegateExport> OnDigitalDeactivate { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the action's digital value changes'.")]
        public EventExport<DigitalUpdateDelegateExport> OnDigitalUpdate { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the action's analog value changes'.")]
        public EventExport<AnalogUpdateDelegateExport> OnAnalogUpdate { get; }

        [ScriptExportMethod("Adds a keyboard trigger to the `InputAction`.")]
        [return: ScriptExportReturn("The added keyboard trigger.")]
        public KeyboardActionTriggerExport AddKeyboardTrigger(
            [ScriptExportParameter("The scan code of the key the trigger will listen for.")] KeyboardScanCode scanCode,
            [ScriptExportParameter("The value the trigger will apply to analog actions when activated.")] float analogContribution
        )
        {
            return new KeyboardActionTriggerExport(Reference.AddKeyboardTrigger(scanCode, analogContribution));
        }

        [ScriptExportMethod("Adds a gamepad button trigger to the `InputAction`.")]
        [return: ScriptExportReturn("The added gamepad button trigger.")]
        public GamePadButtonActionTriggerExport AddGamePadButtonTrigger(
            [ScriptExportParameter("The scan code of the gamepad button the trigger will listen for.")] GamePadButtonScanCode scanCode,
            [ScriptExportParameter("The index of the gamepad the trigger will listen for.")] int gamePadIndex,
            [ScriptExportParameter("The value the trigger will apply to analog actions when activated.")] float analogContribution
        )
        {
            return new GamePadButtonActionTriggerExport(Reference.AddGamePadButtonTrigger(scanCode, gamePadIndex, analogContribution));
        }

        [ScriptExportMethod("Adds a gamepad axis trigger to the `InputAction`.")]
        [return: ScriptExportReturn("The added gamepad axis trigger.")]
        public GamePadAxisActionTriggerExport AddGamePadAxisTrigger(
            [ScriptExportParameter("The scan code of the gamepad axis the trigger will listen for.")] GamePadAxisScanCode scanCode,
            [ScriptExportParameter("The index of the gamepad the trigger will listen for.")] int gamePadIndex,
            [ScriptExportParameter("The thereshold value after which the trigger will activate digital actions.")] float digitalActivationThereshold,
            [ScriptExportParameter("The dead zone [0-1] area inside which the trigger will be ignored.")] float deadZone,
            [ScriptExportParameter("Whether the value of the trigger will be inverted.")] bool invert
        )
        {
            return new GamePadAxisActionTriggerExport(Reference.AddGamePadAxisTrigger(scanCode, gamePadIndex, digitalActivationThereshold, deadZone, invert));
        }

        [ScriptExportMethod("Removes a trigger from the `InputAction`.")]
        public void RemoveTrigger(
            [ScriptExportParameter("The trigger to remove.")] InputActionTriggerExport trigger
        )
        {
            Reference.RemoveTrigger(trigger.Value);
        }

        [ScriptExportMethod("Removes all triggers from the `InputAction`.")]
        public void ClearTriggers()
        {
            Reference.ClearTriggers();
        }

        [ExportCastConstructor]
        internal InputActionExport(ByReference<IInputAction> referencePointer) : base(referencePointer)
        {
            OnDigitalActivate = new EventExport<DigitalActivateDelegateExport>();
            Reference.OnDigitalActivate += action =>
            {
                if (action == Reference)
                {
                    OnDigitalActivate.Fire(this);
                }
            };

            OnDigitalDeactivate = new EventExport<DigitalDeactivateDelegateExport>();
            Reference.OnDigitalDeactivate += action =>
            {
                if (action == Reference)
                {
                    OnDigitalDeactivate.Fire(this);
                }
            };

            OnDigitalUpdate = new EventExport<DigitalUpdateDelegateExport>();
            Reference.OnDigitalUpdate += (action, val) =>
            {
                if (action == Reference)
                {
                    OnDigitalUpdate.Fire(this, val);
                }
            };

            OnAnalogUpdate = new EventExport<AnalogUpdateDelegateExport>();
            Reference.OnAnalogUpdate += (action, val) =>
            {
                if (action == Reference)
                {
                    OnAnalogUpdate.Fire(this, val);
                }
            };
        }

        [ExportCastConstructor]
        internal InputActionExport(IInputAction value) : base(value)
        {
            OnDigitalActivate = new EventExport<DigitalActivateDelegateExport>();
            Reference.OnDigitalActivate += action =>
            {
                if (action == Reference)
                {
                    OnDigitalActivate.Fire(this);
                }
            };

            OnDigitalDeactivate = new EventExport<DigitalDeactivateDelegateExport>();
            Reference.OnDigitalDeactivate += action =>
            {
                if (action == Reference)
                {
                    OnDigitalDeactivate.Fire(this);
                }
            };

            OnDigitalUpdate = new EventExport<DigitalUpdateDelegateExport>();
            Reference.OnDigitalUpdate += (action, val) =>
            {
                if (action == Reference)
                {
                    OnDigitalUpdate.Fire(this, val);
                }
            };

            OnAnalogUpdate = new EventExport<AnalogUpdateDelegateExport>();
            Reference.OnAnalogUpdate += (action, val) =>
            {
                if (action == Reference)
                {
                    OnAnalogUpdate.Fire(this, val);
                }
            };
        }
    }
}