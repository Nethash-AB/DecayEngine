using System;
using System.Linq;
using System.Reflection;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Capability
{
    public static class ComponentExtensions
    {
        public static IComponent GetRealComponent(this IComponentExport componentExport)
        {
            if (componentExport.GetType().IsInstanceOfType(typeof(ExportableManagedObject<>)))
            {
                throw new ArgumentException($"Component is not {typeof(ExportableManagedObject<>).Name}.", nameof(componentExport));
            }

            Type baseType = componentExport.GetType().IsGenericType ? componentExport.GetType() : componentExport.GetType().BaseType;
            Type genericType = baseType?.GenericTypeArguments[0];
            if (genericType == null || genericType.GetInterface(typeof(IComponent).Name) == null)
            {
                throw new ArgumentException($"Component is not {typeof(ExportableManagedObject<IComponent>).Name}.", nameof(componentExport));
            }

            Type realType = typeof(ExportableManagedObject<>).MakeGenericType(genericType);
            object value = realType.GetProperty("Value")?.GetValue(componentExport); // RISKY. REMEMBER TO RENAME THIS IF THE PROPERTY IS RENAMED.
            if (value == null)
            {
                throw new ArgumentException("Component does not have a wrapped value.", nameof(componentExport));
            }

            return (IComponent) value;
        }

        public static IComponentExport GetComponentProxy(this IComponent component)
        {
            Type componentType = component.ExportType;

            ConstructorInfo constructor = null;
            if (componentType != null)
            {
                constructor = componentType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .FirstOrDefault(ctr => 
                        ctr.GetCustomAttribute<ExportCastConstructorAttribute>() != null 
                        && ctr.GetParameters().Any(param => typeof(IComponent).IsAssignableFrom(param.ParameterType)));
            }

            if (constructor == null)
            {
                throw new ArgumentException($"Component does not have an export casting constructor ({nameof(ExportCastConstructorAttribute)}).",
                    nameof(component));
            }

            return (IComponentExport) constructor.Invoke(new object[]{component});
        }
    }
}