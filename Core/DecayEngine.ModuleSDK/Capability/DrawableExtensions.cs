using System;
using System.Linq;
using System.Reflection;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.Capabilities;

namespace DecayEngine.ModuleSDK.Capability
{
    public static class DrawableExtensions
    {
        public static IDrawable GetRealDrawable(this IDrawableExport drawableExport)
        {
            if (drawableExport.GetType().IsInstanceOfType(typeof(ExportableManagedObject<>)))
            {
                throw new ArgumentException($"Drawable is not {typeof(ExportableManagedObject<>).Name}.", nameof(drawableExport));
            }

            Type baseType = drawableExport.GetType().IsGenericType ? drawableExport.GetType() : drawableExport.GetType().BaseType;
            Type genericType = baseType?.GenericTypeArguments[0];
            if (genericType == null || genericType.GetInterface(typeof(IDrawable).Name) == null)
            {
                throw new ArgumentException($"Drawable is not {typeof(ExportableManagedObject<IDrawable>).Name}.", nameof(drawableExport));
            }

            Type realType = typeof(ExportableManagedObject<>).MakeGenericType(genericType);
            object value = realType.GetProperty("Value")?.GetValue(drawableExport); // RISKY. REMEMBER TO RENAME THIS IF THE PROPERTY IS RENAMED.
            if (value == null)
            {
                throw new ArgumentException("Drawable does not have a wrapped value.", nameof(drawableExport));
            }

            return (IDrawable) value;
        }

        public static IDrawableExport GetDrawableProxy(this IDrawable drawable)
        {
            if (drawable is IComponent component)
            {
                Type drawableType = component.ExportType;
                ConstructorInfo constructor = drawableType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .FirstOrDefault(ctr => 
                        ctr.GetCustomAttribute<ExportCastConstructorAttribute>() != null 
                        && ctr.GetParameters().Any(param => typeof(IDrawable).IsAssignableFrom(param.ParameterType)));

                if (constructor == null)
                {
                    throw new ArgumentException($"Drawable does not have an export casting constructor ({nameof(ExportCastConstructorAttribute)}).",
                        nameof(drawable));
                }

                return (IDrawableExport) constructor.Invoke(new object[]{drawable});
//                return (IDrawableExport) 
//                    Activator.CreateInstance(drawableType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, drawable);
            }

            Type proxyType = typeof(ExportableManagedObject<>).MakeGenericType(drawable.GetType());
            return (IDrawableExport) 
                Activator.CreateInstance(proxyType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly, drawable);
        }
    }
}