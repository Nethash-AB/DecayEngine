using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Object.Input;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input
{
    [ScriptExportClass("InputActionTriggerAnalog", "Represents an Analog Input Action Trigger.")]
    public abstract class InputActionTriggerAnalogExport : InputActionTriggerExport
    {
        public override int Type => (int) ManagedExportType.InputActionTriggerAnalog;
        public override string SubType => "";

        [ScriptExportProperty("The value of the Input Action Trigger.")]
        public float TriggerValue => ((IInputActionTriggerAnalog) Reference).Value;

        [ScriptExportProperty("The thereshold value after which the Input Action Trigger will activate digital actions.")]
        public float DigitalActivationThereshold
        {
            get => ((IInputActionTriggerAnalog) Reference).DigitalActivationThereshold;
            set => ((IInputActionTriggerAnalog) Reference).DigitalActivationThereshold = value;
        }

        [ScriptExportProperty("The dead zone [0-1] area inside which the trigger will be ignored.")]
        public float DeadZone
        {
            get => ((IInputActionTriggerAnalog) Reference).DeadZone;
            set => ((IInputActionTriggerAnalog) Reference).DeadZone = value;
        }

        [ScriptExportProperty("Whether the value of the trigger will be inverted.")]
        public bool Invert
        {
            get => ((IInputActionTriggerAnalog) Reference).Invert;
            set => ((IInputActionTriggerAnalog) Reference).Invert = value;
        }

        [ExportCastConstructor]
        protected InputActionTriggerAnalogExport(ByReference<IInputActionTrigger> referencePointer)
            : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        protected InputActionTriggerAnalogExport(IInputActionTrigger value) : base(value)
        {
        }
    }
}