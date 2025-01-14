using System;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports
{
    [ScriptExportClass("ManagedObject", "Represents a Managed class or struct.")]
    public abstract class ExportableManagedObject<T> : IEquatable<ExportableManagedObject<T>>, IManagedWrapper
    {
        private T _value;
        private readonly ByReference<T> _referencePointer;
        protected ref T Reference => ref _referencePointer != null ? ref _referencePointer.Invoke() : ref _value;

        public T Value => _referencePointer != null ? _referencePointer.Invoke() : _value;

        public abstract int Type { get; }
        public virtual string SubType => null;

        protected ExportableManagedObject()
        {
        }

        public ExportableManagedObject(T value)
        {
            _value = value;
        }

        public ExportableManagedObject(ByReference<T> referencePointer)
        {
            _referencePointer = referencePointer;
        }

        public static implicit operator T(ExportableManagedObject<T> exportableObject)
        {
            return exportableObject.Value;
        }

        [ScriptExportMethod("Indicates whether the current object is equal to another object of the same type.")]
        [return: ScriptExportReturn("`true` if both objects are the same, `false` otherwise.")]
        public virtual bool Equals(
        [ScriptExportParameter("The other object of the same type to check equality against.")] ExportableManagedObject<T> other
        )
        {
            return other != null && Reference.Equals(other.Reference);
        }

        [ScriptExportMethod("Indicates whether the current object is equal to another object of the same type.")]
        [return: ScriptExportReturn("`true` if both objects are the same, `false` otherwise.")]
        public override bool Equals(
        [ScriptExportParameter("The other object of the same type to check equality against.")] object obj
        )
        {
            return obj is ExportableManagedObject<T> export && Equals(export);
        }

        public override int GetHashCode()
        {
            return Reference.GetHashCode();
        }
    }
}