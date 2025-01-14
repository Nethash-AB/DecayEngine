using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Parameter
{
    [TypeConverter(typeof(TargetPlatformTypeConverter))]
    public class TargetPlatforms : IEquatable<TargetPlatforms>
    {
        [Flags]
        private enum TargetPlatformTypes
        {
            None = 0,
            Desktop = 1,
            Android = 2,
            All = Desktop | Android
        }

        public class TargetPlatformTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string[]) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string[] stringValues)
                {
                    return Enum.Parse<TargetPlatformTypes>(string.Join(", ", stringValues.Select(s => s.Trim())));
                }

                return base.ConvertFrom(context, culture, value);
            }
        }

        TargetPlatformTypes Value { get; }

        public bool BuildDesktop => Value.HasFlag(TargetPlatformTypes.Desktop);
        public bool BuildAndroid => Value.HasFlag(TargetPlatformTypes.Android);

        public static TargetPlatforms None = new TargetPlatforms(TargetPlatformTypes.None);
        public static TargetPlatforms Desktop = new TargetPlatforms(TargetPlatformTypes.Desktop);
        public static TargetPlatforms Android = new TargetPlatforms(TargetPlatformTypes.Android);
        public static TargetPlatforms All = new TargetPlatforms(TargetPlatformTypes.All);

        TargetPlatforms(TargetPlatformTypes targetPlatformType)
        {
            Value = targetPlatformType;
        }

        public static bool operator == (TargetPlatforms left, TargetPlatforms right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator != (TargetPlatforms left, TargetPlatforms right)
        {
            return !(left == right);
        }

        public override string ToString() => Value.ToString();

        public static implicit operator string(TargetPlatforms targetPlatforms)
        {
            return targetPlatforms.Value.ToString();
        }

        public bool Equals(TargetPlatforms other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TargetPlatforms) obj);
        }

        public override int GetHashCode() => (int) Value;
    }
}