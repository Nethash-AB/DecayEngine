using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Object.Input;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input
{
    [ScriptExportClass("InputActionTrigger", "Represents an Input Action Trigger.")]
    public abstract class InputActionTriggerExport : ExportableManagedObject<IInputActionTrigger>
    {
        protected InputActionTriggerExport()
        {
        }

        [ExportCastConstructor]
        protected InputActionTriggerExport(ByReference<IInputActionTrigger> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        protected InputActionTriggerExport(IInputActionTrigger value) : base(value)
        {
        }
    }
}