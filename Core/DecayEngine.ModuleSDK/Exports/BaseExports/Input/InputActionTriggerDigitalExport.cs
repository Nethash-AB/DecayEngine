using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Object.Input;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input
{
    [ScriptExportClass("InputActionTriggerDigital", "Represents a Digital Input Action Trigger.")]
    public abstract class InputActionTriggerDigitalExport : InputActionTriggerExport
    {
        public override int Type => (int) ManagedExportType.InputActionTriggerDigital;
        public override string SubType => "";

        [ScriptExportProperty("The value of the Input Action Trigger.")]
        public bool TriggerValue => ((IInputActionTriggerDigital) Reference).Value;

        [ScriptExportProperty("The value the trigger will apply to analog actions when activated.")]
        public float AnalogContribution
        {
            get => ((IInputActionTriggerDigital) Reference).AnalogContribution;
            set => ((IInputActionTriggerDigital) Reference).AnalogContribution = value;
        }

        [ExportCastConstructor]
        protected InputActionTriggerDigitalExport(ByReference<IInputActionTrigger> referencePointer)
            : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        protected InputActionTriggerDigitalExport(IInputActionTrigger value) : base(value)
        {
        }
    }
}