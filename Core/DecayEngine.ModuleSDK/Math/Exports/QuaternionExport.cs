using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;

#pragma warning disable 660,661

namespace DecayEngine.ModuleSDK.Math.Exports
{
    [ScriptExportClass("Quaternion", "Represents a SIMD enhanced Quaternion.", typeof(MathNamespaceExport))]
    public class QuaternionExport : ExportableManagedObject<Quaternion>
    {
        [ScriptExportProperty("The `X` component of the `Quaternion`.")]
        public float X
        {
            get => Reference.X;
            set => Reference.X = value;
        }

        [ScriptExportProperty("The `Y` component of the `Quaternion`.")]
        public float Y
        {
            get => Reference.Y;
            set => Reference.Y = value;
        }

        [ScriptExportProperty("The `Z` component of the `Quaternion`.")]
        public float Z
        {
            get => Reference.Z;
            set => Reference.Z = value;
        }

        [ScriptExportProperty("The angle component of the `Quaternion` in radians.")]
        public float W
        {
            get => Reference.W;
            set => Reference.W = value;
        }

        [ScriptExportProperty("The euler angles (pitch, yaw, roll) of the `Quaternion` in radians.")]
        public Vector3Export EulerAngles => Reference.EulerAngles;

        [ScriptExportProperty("The conjugate of the `Quaternion`.")]
        public QuaternionExport Conjugate => Reference.Conjugate;

        [ScriptExportProperty("Returns a new instance of `Quaternion` representing the identity rotation.")]
        public static QuaternionExport Identity => new QuaternionExport(Quaternion.Identity);

        public override int Type => (int) ManagedExportType.Quaternion;
        public override string SubType => "float";

        public QuaternionExport(Quaternion quaternion) : base(quaternion)
        {
        }

        public QuaternionExport(ByReference<Quaternion> referencePointer) : base(referencePointer)
        {
        }

        [ScriptExportConstructor]
        public QuaternionExport(
            [ScriptExportParameter("The value of the `X` component of the `Quaternion`.")] float x,
            [ScriptExportParameter("The value of the `Y` component of the `Quaternion`.")] float y,
            [ScriptExportParameter("The value of the `Z` component of the `Quaternion`.")] float z,
            [ScriptExportParameter("The value of the `W` component of the `Quaternion`.")] float w
        ) : this(new Quaternion(x, y, z, w))
        {
        }

        [ScriptExportConstructor]
        public QuaternionExport(
            [ScriptExportParameter("The `Vector3` representing the axis of the `Quaternion`.")] Vector3Export axis,
            [ScriptExportParameter("The angle of the `Quaternion`.")] float w
        ) : this(new Quaternion(axis, w))
        {
        }

        public static implicit operator QuaternionExport(Quaternion obj)
        {
            return new QuaternionExport(obj);
        }

        [ScriptExportMethod("Returns the axis (x, y, z) and angle (w) of the `Quaternion`.")]
        [return: ScriptExportReturn("The axis (x, y, z) and angle (w) of the `Quaternion`.")]
        public Vector4Export ToAxisAngle()
        {
            return Reference.AxisAngle;
        }


        [ScriptExportMethod("Returns the length of the `Quaternion`.")]
        [return: ScriptExportReturn("The length of the `Quaternion`.")]
        public float Length()
        {
            return Reference.Length;
        }

        [ScriptExportMethod("Returns the length of the `Quaternion` squared.")]
        [return: ScriptExportReturn("The length of the `Quaternion` squared.")]
        public float LengthSquared()
        {
            return Reference.LengthSquared;
        }

        [ScriptExportMethod("Returns a `Quaternion` equal to the rotation represented by the specified euler angles (pitch, yaw roll) in radians.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` equal to the rotation represented by the specified euler angles (pitch, yaw roll) in radians.")]
        public static QuaternionExport FromEulerAngles(
            [ScriptExportParameter("The euler angles (pitch, yaw, roll) in radians to construct the `Quaternion` with.")] Vector3Export value
        )
        {
            return Quaternion.FromEulerAngles(value);
        }

        [ScriptExportMethod("Returns a `Quaternion` equal to the rotation represented by the specified axis (world x, y, z) and angle (in radians).")]
        [return: ScriptExportReturn("A new instance of `Quaternion` equal to the rotation represented by the specified axis (world x, y, z) and angle (in radians).")]
        public static QuaternionExport FromAxisAngle(
            [ScriptExportParameter("The axis (world x, y, z) to construct the `Quaternion` with.")] Vector3Export axis,
            [ScriptExportParameter("The angle in radians to construct the `Quaternion` with.")] float angle
        )
        {
            return Quaternion.FromAxisAngle(axis, angle);
        }

        [ScriptExportMethod("Returns the specified `Quaternion` in normalized form.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the result of normalizing the specified `Quaternion`.")]
        public static QuaternionExport Normalize(
            [ScriptExportParameter("The `Quaternion` to normalize.")] QuaternionExport value
        )
        {
            return value.Reference.Normalized;
        }

        [ScriptExportMethod("Returns the specified `Quaternion` in normalized form.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the result of normalizing the `Quaternion`.")]
        public QuaternionExport Normalize()
        {
            return Normalize(Reference);
        }

        [ScriptExportMethod("Returns the specified `Quaternion` in inverted form.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the result of inverting the specified `Quaternion`.")]
        public static QuaternionExport Invert(
            [ScriptExportParameter("The `Quaternion` to invert.")] QuaternionExport value
        )
        {
            return value.Reference.Normalized;
        }

        [ScriptExportMethod("Returns the specified `Quaternion` in inverted form.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the result of inverting the `Quaternion`.")]
        public QuaternionExport Invert()
        {
            return Invert(Reference);
        }

        [ScriptExportMethod("Adds two `Quaternion`s together.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the sum of the specified `Quaternion`s.")]
        public static QuaternionExport Add(
            [ScriptExportParameter("The first `Quaternion`.")] QuaternionExport left,
            [ScriptExportParameter("The second `Quaternion`.")] QuaternionExport right
        )
        {
            return Quaternion.Add(left, right);
        }

        [ScriptExportMethod("Adds the specified `Quaternion` to the `Quaternion`.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the sum of the specified `Quaternion`s.")]
        public QuaternionExport Add(
            [ScriptExportParameter("The second `Quaternion`.")] QuaternionExport right
        )
        {
            return Add(Reference, right);
        }

        [ScriptExportMethod("Subtracts the second `Quaternion` from the first.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the subtraction of the specified `Quaternion`s.")]
        public static QuaternionExport Subtract(
            [ScriptExportParameter("The first `Quaternion`.")] QuaternionExport left,
            [ScriptExportParameter("The second `Quaternion`.")] QuaternionExport right
        )
        {
            return Quaternion.Subtract(left, right);
        }

        [ScriptExportMethod("Subtracts the specified `Quaternion` from the `Quaternion`.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the subtraction of the specified `Quaternion`s.")]
        public QuaternionExport Subtract(
            [ScriptExportParameter("The second `Quaternion`.")] QuaternionExport right
        )
        {
            return Subtract(Reference, right);
        }

        [ScriptExportMethod("Multiplies two `Quaternion`s together.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the multiplication of the specified `Quaternion`s.")]
        public static QuaternionExport Multiply(
            [ScriptExportParameter("The first `Quaternion`.")] QuaternionExport left,
            [ScriptExportParameter("The second `Quaternion`.")] QuaternionExport right
        )
        {
            return Quaternion.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies the `Quaternion` by the specified `Quaternion`.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the multiplication of the specified `Quaternion`s.")]
        public QuaternionExport Multiply(
            [ScriptExportParameter("The second `Quaternion`.")] QuaternionExport right
        )
        {
            return Multiply(Reference, right);
        }

        [ScriptExportMethod("Multiplies a `Quaternion` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the multiplication of the specified `Quaternion` and the specified scalar.")]
        public static QuaternionExport Multiply(
            [ScriptExportParameter("The `Quaternion` to multiply.")] QuaternionExport left,
            [ScriptExportParameter("The scalar to multiply by.")] float right
        )
        {
            return Quaternion.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies a `Quaternion` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the multiplication of the specified `Quaternion` and the specified scalar.")]
        public static QuaternionExport Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float left,
            [ScriptExportParameter("The `Quaternion` to multiply.")] QuaternionExport right
        )
        {
            return left * right.Reference;
        }

        [ScriptExportMethod("Multiplies the `Quaternion` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Quaternion` representing the multiplication of the `Quaternion` and the specified scalar.")]
        public QuaternionExport Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float value
        )
        {
            return Multiply(Reference, value);
        }

        public static QuaternionExport operator +(QuaternionExport left, QuaternionExport right)
        {
            return new QuaternionExport(left.Reference + right.Reference);
        }

        public static QuaternionExport operator -(QuaternionExport left, QuaternionExport right)
        {
            return new QuaternionExport(left.Reference - right.Reference);
        }

        public static QuaternionExport operator *(QuaternionExport left, QuaternionExport right)
        {
            return new QuaternionExport(left.Reference * right.Reference);
        }

        public static QuaternionExport operator *(QuaternionExport left, float right)
        {
            return new QuaternionExport(left.Reference * right);
        }

        public static QuaternionExport operator *(float left, QuaternionExport right)
        {
            return new QuaternionExport(left * right.Reference);
        }

        public static bool operator ==(QuaternionExport left, QuaternionExport right)
        {
            if (left == null || right == null) return false;
            return left.Reference.Equals(right.Reference);
        }

        public static bool operator !=(QuaternionExport left, QuaternionExport right)
        {
            if (left == null || right == null) return true;
            return !left.Reference.Equals(right.Reference);
        }
    }
}