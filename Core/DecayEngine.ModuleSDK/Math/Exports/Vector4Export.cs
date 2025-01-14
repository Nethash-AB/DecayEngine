using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;

#pragma warning disable 660,661

namespace DecayEngine.ModuleSDK.Math.Exports
{
    [ScriptExportClass("Vector4", "Represents a SIMD enhanced Vector with 4 dimensions.", typeof(MathNamespaceExport))]
    public class Vector4Export : ExportableManagedObject<Vector4>
    {
        [ScriptExportProperty("The `X` component of the `Vector4`.")]
        public float X
        {
            get => Reference.X;
            set => Reference.X = value;
        }

        [ScriptExportProperty("The `Y` component of the `Vector4`.")]
        public float Y
        {
            get => Reference.Y;
            set => Reference.Y = value;
        }

        [ScriptExportProperty("The `Z` component of the `Vector4`.")]
        public float Z
        {
            get => Reference.Z;
            set => Reference.Z = value;
        }

        [ScriptExportProperty("The `W` component of the `Vector4`.")]
        public float W
        {
            get => Reference.Z;
            set => Reference.Z = value;
        }

        [ScriptExportProperty("Returns a new instance of `Vector4` representing a vector whose `X`, `Y`, `Z` and `W` components are Zero.")]
        public static Vector4Export Zero => new Vector4Export(Vector4.Zero);
        [ScriptExportProperty("Returns a new instance of `Vector4` representing a vector whose `X`, `Y`, `Z` and `W` components are One.")]
        public static Vector4Export One => new Vector4Export(Vector4.One);
        [ScriptExportProperty("Returns a new instance of `Vector4` representing a vector whose `X` component is One and all other components are Zero.")]
        public static Vector4Export UnitX => new Vector4Export(Vector4.UnitX);
        [ScriptExportProperty("Returns a new instance of `Vector4` representing a vector whose `Y` component is One and all other components are Zero.")]
        public static Vector4Export UnitY => new Vector4Export(Vector4.UnitY);
        [ScriptExportProperty("Returns a new instance of `Vector4` representing a vector whose `Z` component is One and all other components are Zero.")]
        public static Vector4Export UnitZ => new Vector4Export(Vector4.UnitZ);
        [ScriptExportProperty("Returns a new instance of `Vector4` representing a vector whose `W` component is One and all other components are Zero.")]
        public static Vector4Export UnitW => new Vector4Export(Vector4.UnitW);

        public override int Type => (int) ManagedExportType.Vector4;
        public override string SubType => "float";

        private Vector4Export(Vector4 vector) : base(vector)
        {
        }

        public Vector4Export(ByReference<Vector4> referencePointer) : base(referencePointer)
        {
        }

        [ScriptExportConstructor]
        public Vector4Export(
            [ScriptExportParameter("The value of all the components of the `Vector4`.")] float value
        ) : this(new Vector4(value))
        {
        }

        [ScriptExportConstructor]
        public Vector4Export(
            [ScriptExportParameter("The `Vector2` whose `X` and `Y` components will be used for the `Vector4`.")] Vector2Export value,
            [ScriptExportParameter("The value of the `Z` component of the `Vector4`.")] float z,
            [ScriptExportParameter("The value of the `W` component of the `Vector4`.")] float w
        ) : this(new Vector4(value.X, value.Y, z, w))
        {
        }

        [ScriptExportConstructor]
        public Vector4Export(
            [ScriptExportParameter("The value of the `X` component of the `Vector4`.")] float x,
            [ScriptExportParameter("The value of the `Y` component of the `Vector4`.")] float y,
            [ScriptExportParameter("The value of the `Z` component of the `Vector4`.")] float z,
            [ScriptExportParameter("The value of the `W` component of the `Vector4`.")] float w
        ) : this(new Vector4(x, y, z, w))
        {
        }

        [ScriptExportConstructor]
        public Vector4Export(
            [ScriptExportParameter("The `Vector3` whose `X`, `Y` and `Z` components will be used for the `Vector4`.")] Vector3Export value,
            [ScriptExportParameter("The value of the `W` component of the `Vector4`.")] float w
        ) : this(new Vector4(value, w))
        {
        }

        public static implicit operator Vector4Export(Vector4 obj)
        {
            return new Vector4Export(obj);
        }

        [ScriptExportMethod("Returns the length of the `Vector4`.")]
        [return: ScriptExportReturn("The length of the `Vector4`.")]
        public float Length()
        {
            return Reference.Length;
        }

        [ScriptExportMethod("Returns the length of the `Vector4` squared.")]
        [return: ScriptExportReturn("The length of the `Vector4` squared.")]
        public float LengthSquared()
        {
            return Reference.LengthSquared;
        }

        [ScriptExportMethod("Computes the Euclidean distance between the two given `Vector4`.")]
        [return: ScriptExportReturn("The Euclidean distance between the two given `Vector4`.")]
        public static float Distance(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export value1,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2
        )
        {
            return value1.Distance(value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance between the `Vector4` and the specified `Vector4`.")]
        [return: ScriptExportReturn("The Euclidean distance between the specified `Vector4`s.")]
        public float Distance(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2
        )
        {
            return Distance(Reference, value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance squared between the two given `Vector4`.")]
        [return: ScriptExportReturn("The Euclidean distance squared between the two given `Vector4`.")]
        public static float DistanceSquared(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export value1,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2
        )
        {
            return value1.DistanceSquared(value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance squared between the `Vector4` and the specified `Vector4`.")]
        [return: ScriptExportReturn("The Euclidean distance squared between the specified `Vector4`s.")]
        public float DistanceSquared(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2
        )
        {
            return DistanceSquared(Reference, value2);
        }

        [ScriptExportMethod("Returns a `Vector4` with the same direction as the specified `Vector4`, but with a length of One.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the result of normalizing the specified `Vector4`.")]
        public static Vector4Export Normalize(
            [ScriptExportParameter("The `Vector4` to normalize.")] Vector4Export value
        )
        {
            return value.Reference.Normalized;
        }

        [ScriptExportMethod("Returns a `Vector4` with the same direction as the `Vector4`, but with a length of One.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the result of normalizing the `Vector4`.")]
        public Vector4Export Normalize()
        {
            return Normalize(Reference);
        }

        [ScriptExportMethod("Restricts a `Vector4` between a minimum and a maximum value.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the result of clamping the specified `Vector4` between the specified values.")]
        public static Vector4Export Clamp(
            [ScriptExportParameter("The `Vector4` to clamp.")] Vector4Export value1,
            [ScriptExportParameter("The `Vector4` representing the minimum value to clamp with.")] Vector4Export min,
            [ScriptExportParameter("The `Vector4` representing the maximum value to clamp with.")] Vector4Export max
        )
        {
            return Vector4.Clamp(value1, min, max);
        }

        [ScriptExportMethod("Restricts the `Vector4` between a minimum and a maximum value.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the result of clamping the `Vector4` between the specified values.")]
        public Vector4Export Clamp(
            [ScriptExportParameter("The `Vector4` representing the minimum value to clamp with.")] Vector4Export min,
            [ScriptExportParameter("The `Vector4` representing the maximum value to clamp with.")] Vector4Export max
        )
        {
            return Clamp(Reference, min, max);
        }

        [ScriptExportMethod("Performs a linear interpolation between two `Vector4`s based on the given weighting.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the result of interpolating the specified `Vector4`s by the specified `amount`.")]
        public static Vector4Export Lerp(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export value1,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2,
            [ScriptExportParameter("The amount to interpolate by.")] float amount
        )
        {
            return Vector4.Lerp(value1, value2, amount);
        }

        [ScriptExportMethod("Adds two `Vector4`s together.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the sum of the specified `Vector4`s.")]
        public static Vector4Export Add(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export left,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Vector4.Add(left, right);
        }

        [ScriptExportMethod("Adds the specified `Vector4` to the `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the sum of the specified `Vector4`s.")]
        public Vector4Export Add(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Add(Reference, right);
        }

        [ScriptExportMethod("Subtracts the second `Vector4` from the first.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the subtraction of the specified `Vector4`s.")]
        public static Vector4Export Subtract(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export left,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Vector4.Subtract(left, right);
        }

        [ScriptExportMethod("Subtracts the specified `Vector4` from the `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the subtraction of the specified `Vector4`s.")]
        public Vector4Export Subtract(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Subtract(Reference, right);
        }

        [ScriptExportMethod("Multiplies two `Vector4`s together.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the multiplication of the specified `Vector4`s.")]
        public static Vector4Export Multiply(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export left,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Vector4.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies the `Vector4` by the specified `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the multiplication of the specified `Vector4`s.")]
        public Vector4Export Multiply(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Multiply(Reference, right);
        }

        [ScriptExportMethod("Multiplies a `Vector4` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the multiplication of the specified `Vector4` and the specified scalar.")]
        public static Vector4Export Multiply(
            [ScriptExportParameter("The `Vector4` to multiply.")] Vector4Export left,
            [ScriptExportParameter("The scalar to multiply by.")] float right
        )
        {
            return Vector4.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies a `Vector4` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the multiplication of the specified `Vector4` and the specified scalar.")]
        public static Vector4Export Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float left,
            [ScriptExportParameter("The `Vector4` to multiply.")] Vector4Export right
        )
        {
            return left * right.Reference;
        }

        [ScriptExportMethod("Multiplies the `Vector4` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the multiplication of the `Vector4` and the specified scalar.")]
        public Vector4Export Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float value
        )
        {
            return Multiply(Reference, value);
        }

        [ScriptExportMethod("Divides the first `Vector4` by the second.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the division of the specified `Vector4`s.")]
        public static Vector4Export Divide(
            [ScriptExportParameter("The first `Vector4`.")] Vector4Export left,
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Vector4.Divide(left, right);
        }

        [ScriptExportMethod("Divides the `Vector4` by the specified `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the division of the specified `Vector4`s.")]
        public Vector4Export Divide(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export right
        )
        {
            return Divide(Reference, right);
        }

        [ScriptExportMethod("Divides the specified `Vector4` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the division of the specified `Vector4` and the specified scalar.")]
        public static Vector4Export Divide(
            [ScriptExportParameter("The `Vector4` to divide.")] Vector4Export left,
            [ScriptExportParameter("The scalar to divide by.")] float divisor
        )
        {
            return Vector4.Divide(left, divisor);
        }

        [ScriptExportMethod("Divides the `Vector4` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the division of the `Vector4` and the specified scalar.")]
        public Vector4Export Divide(
            [ScriptExportParameter("The scalar to divide by.")] float divisor
        )
        {
            return Divide(Reference, divisor);
        }

        [ScriptExportMethod("Negates the specified `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the negation of the specified `Vector4`.")]
        public static Vector4Export Negate(
            [ScriptExportParameter("The `Vector4` to negate.")] Vector4Export value
        )
        {
            return value.Reference;
        }

        [ScriptExportMethod("Negates the `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the negation of the `Vector4`.")]
        public Vector4Export Negate()
        {
            return Negate(Reference);
        }

        [ScriptExportMethod("Returns the dot product of two `Vector4`s.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the dot product of the specified `Vector4`s.")]
        public static float Dot(
            [ScriptExportParameter("The first `Vector3`.")] Vector4Export vector1,
            [ScriptExportParameter("The second `Vector3`.")] Vector4Export vector2
        )
        {
            return Vector4.Dot(vector1, vector2);
        }

        [ScriptExportMethod("Returns the dot product of the `Vector4` and the specified `Vector4`.")]
        [return: ScriptExportReturn("A new instance of `Vector4` representing the dot product of the specified `Vector4`s.")]
        public float Dot(
            [ScriptExportParameter("The second `Vector4`.")] Vector4Export vector2
        )
        {
            return Dot(Reference, vector2);
        }

//        [ScriptExportMethod("Returns a `Vector4` whose elements are the minimum of each of the pairs of elements in two specified `Vector4`s.")]
//        [return: ScriptExportReturn("A new instance of `Vector4` representing the minimum between the specified `Vector4`s.")]
//        public static Vector4Export Min(
//            [ScriptExportParameter("The first `Vector4`.")] Vector4Export value1,
//            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2
//        )
//        {
//            return Vector4.Min(value1, value2);
//        }

//        [ScriptExportMethod("Returns a `Vector4` whose elements are the maximum of each of the pairs of elements in two specified `Vector4`s.")]
//        [return: ScriptExportReturn("A new instance of `Vector4` representing the maximum between the specified `Vector4`s.")]
//        public static Vector4Export Max(
//            [ScriptExportParameter("The first `Vector4`.")] Vector4Export value1,
//            [ScriptExportParameter("The second `Vector4`.")] Vector4Export value2
//        )
//        {
//            return Vector4.Max(value1, value2);
//        }

//        [ScriptExportMethod("Returns a `Vector4` whose elements are the absolute values of each of the specified `Vector4`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector4` representing the absolute of the specified `Vector4`s.")]
//        public static Vector4Export Abs(
//            [ScriptExportParameter("The `Vector4` to get the absolute of.")] Vector4Export value
//        )
//        {
//            return Vector4.Abs(value);
//        }

//        [ScriptExportMethod("Returns a `Vector4` whose elements are the absolute values of each of the `Vector4`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector4` representing the absolute of the `Vector4`.")]
//        public Vector4Export Abs()
//        {
//            return Abs(Reference);
//        }

//        [ScriptExportMethod("Returns a `Vector4` whose elements are the square root of each of the specified `Vector4`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector4` representing the square root of the specified `Vector4`s.")]
//        public static Vector4Export SquareRoot(
//            [ScriptExportParameter("The `Vector4` to get the square root of.")] Vector4Export value
//        )
//        {
//            return Vector4.SquareRoot(value);
//        }

//        [ScriptExportMethod("Returns a `Vector4` whose elements are the square root of each of the `Vector4`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector4` representing the square root of the `Vector4`.")]
//        public Vector4Export SquareRoot()
//        {
//            return SquareRoot(Reference);
//        }

        public static Vector4Export operator +(Vector4Export left, Vector4Export right)
        {
            return new Vector4Export(left.Reference + right.Reference);
        }

        public static Vector4Export operator -(Vector4Export left, Vector4Export right)
        {
            return new Vector4Export(left.Reference - right.Reference);
        }

        public static Vector4Export operator *(Vector4Export left, Vector4Export right)
        {
            return new Vector4Export(left.Reference * right.Reference);
        }

        public static Vector4Export operator *(Vector4Export left, float right)
        {
            return new Vector4Export(left.Reference * right);
        }

        public static Vector4Export operator *(float left, Vector4Export right)
        {
            return new Vector4Export(left * right.Reference);
        }

//        public static Vector4Export operator /(Vector4Export left, Vector4Export right)
//        {
//            return new Vector4Export(left.Reference / right.Reference);
//        }

        public static Vector4Export operator /(Vector4Export value1, float value2)
        {
            return new Vector4Export(value1.Reference / value2);
        }

        public static Vector4Export operator -(Vector4Export value)
        {
            return new Vector4Export(-value.Reference);
        }

        public static bool operator ==(Vector4Export left, Vector4Export right)
        {
            if (left == null || right == null) return false;
            return left.Reference.Equals(right.Reference);
        }

        public static bool operator !=(Vector4Export left, Vector4Export right)
        {
            if (left == null || right == null) return true;
            return !left.Reference.Equals(right.Reference);
        }
    }
}