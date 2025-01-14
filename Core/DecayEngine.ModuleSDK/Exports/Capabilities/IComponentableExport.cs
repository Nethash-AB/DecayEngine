using System.Collections.Generic;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.Capabilities
{
    [ScriptExportInterface("IComponentable", "Represents an object that can have components attached.")]
    public interface IComponentableExport
    {
        [ScriptExportProperty("A list of all the components attached to the object.", typeOverride: typeof(IEnumerable<IComponentExport>))]
        object Components { get; }

        [ScriptExportMethod("Attaches a component to the object.")]
        void AttachComponent(
            [ScriptExportParameter("The component to attach.")] IComponentExport component
        );

        [ScriptExportMethod("Attaches a list of components to the object.")]
        void AttachComponents(
            [ScriptExportParameter("The list of components to attach.")] IEnumerable<IComponentExport> components
        );

        [ScriptExportMethod("Removes a component from the object.")]
        void RemoveComponent(
            [ScriptExportParameter("The component to remove.")] IComponentExport component
        );

        [ScriptExportMethod("Removes a list of components from the object.")]
        void RemoveComponents(
            [ScriptExportParameter("The list of components to remove.")] IEnumerable<IComponentExport> components
        );
    }
}