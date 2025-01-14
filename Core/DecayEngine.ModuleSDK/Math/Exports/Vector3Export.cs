using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;

#pragma warning disable 660,661

namespace DecayEngine.ModuleSDK.Math.Exports
{
    [ScriptExportClass("Vector3", "Represents a SIMD enhanced Vector with 3 dimensions.", typeof(MathNamespaceExport))]
    public class Vector3Export : ExportableManagedObject<Vector3>
    {
        [ScriptExportProperty("The `X` component of the `Vector3`.")]
        public float X
        {
            get => Reference.X;
            set => Reference.X = value;
        }

        [ScriptExportProperty("The `Y` component of the `Vector3`.")]
        public float Y
        {
            get => Reference.Y;
            set => Reference.Y = value;
        }

        [ScriptExportProperty("The `Z` component of the `Vector3`.")]
        public float Z
        {
            get => Reference.Z;
            set => Reference.Z = value;
        }

        [ScriptExportProperty("Returns a new instance of `Vector3` representing a vector whose `X`, `Y` and `Z` components are Zero.")]
        public static Vector3Export Zero => new Vector3Export(Vector3.Zero);
        [ScriptExportProperty("Returns a new instance of `Vector3` representing a vector whose `X`, `Y` and `Z` components are One.")]
        public static Vector3Export One => new Vector3Export(Vector3.One);
        [ScriptExportProperty("Returns a new instance of `Vector3` representing a vector whose `X` component is One and all other components are Zero.")]
        public static Vector3Export UnitX => new Vector3Export(Vector3.UnitX);
        [ScriptExportProperty("Returns a new instance of `Vector3` representing a vector whose `Y` component is One and all other components are Zero.")]
        public static Vector3Export UnitY => new Vector3Export(Vector3.UnitY);
        [ScriptExportProperty("Returns a new instance of `Vector3` representing a vector whose `Z` component is One and all other components are Zero.")]
        public static Vector3Export UnitZ => new Vector3Export(Vector3.UnitZ);

        public override int Type => (int) ManagedExportType.Vector3;
        public override string SubType => "float";

        public Vector3Export(Vector3 vector) : base(vector)
        {
        }

        public Vector3Export(ByReference<Vector3> referencePointer) : base(referencePointer)
        {
        }

        [ScriptExportConstructor]
        public Vector3Export(
            [ScriptExportParameter("The value of all the components of the `Vector3`.")] float value
        ) : this(new Vector3(value))
        {
        }

        [ScriptExportConstructor]
        public Vector3Export(
            [ScriptExportParameter("The `Vector2` whose `X` and `Y` components will be used for the `Vector3`.")] Vector2Export value,
            [ScriptExportParameter("The value of the `Z` component of the `Vector3`.")] float z
        ) : this(new Vector3(value.X, value.Y, z))
        {
        }

        [ScriptExportConstructor]
        public Vector3Export(
            [ScriptExportParameter("The value of the `X` component of the `Vector3`.")] float x,
            [ScriptExportParameter("The value of the `Y` component of the `Vector3`.")] float y,
            [ScriptExportParameter("The value of the `Z` component of the `Vector3`.")] float z
        ) : this(new Vector3(x, y, z))
        {
        }

        public static implicit operator Vector3Export(Vector3 obj)
        {
            return new Vector3Export(obj);
        }

        [ScriptExportMethod("Returns the length of the `Vector3`.")]
        [return: ScriptExportReturn("The length of the `Vector3`.")]
        public float Length()
        {
            return Reference.Length;
        }

        [ScriptExportMethod("Returns the length of the `Vector3` squared.")]
        [return: ScriptExportReturn("The length of the `Vector3` squared.")]
        public float LengthSquared()
        {
            return Reference.LengthSquared;
        }

        [ScriptExportMethod("Computes the Euclidean distance between the two given `Vector3`.")]
        [return: ScriptExportReturn("The Euclidean distance between the two given `Vector3`.")]
        public static float Distance(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export value1,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value2
        )
        {
            return Vector3.Distance(value1, value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance between the `Vector3` and the specified `Vector3`.")]
        [return: ScriptExportReturn("The Euclidean distance between the specified `Vector3`s.")]
        public float Distance(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value
        )
        {
            return Distance(Reference, value);
        }

        [ScriptExportMethod("Computes the Euclidean distance squared between the two given `Vector3`.")]
        [return: ScriptExportReturn("The Euclidean distance squared between the two given `Vector3`.")]
        public static float DistanceSquared(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export value1,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value2
        )
        {
            return Vector3.DistanceSquared(value1, value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance squared between the `Vector3` and the specified `Vector3`.")]
        [return: ScriptExportReturn("The Euclidean distance squared between the specified `Vector3`s.")]
        public float DistanceSquared(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value
        )
        {
            return DistanceSquared(Reference, value);
        }

        [ScriptExportMethod("Returns a `Vector3` with the same direction as the specified `Vector3`, but with a length of One.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of normalizing the specified `Vector3`.")]
        public static Vector3Export Normalize(
            [ScriptExportParameter("The `Vector3` to normalize.")] Vector3Export value
        )
        {
            return value.Reference.Normalized;
        }

        [ScriptExportMethod("Returns a `Vector3` with the same direction as the `Vector3`, but with a length of One.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of normalizing the `Vector3`.")]
        public Vector3Export Normalize()
        {
            return Normalize(Reference);
        }

        [ScriptExportMethod("Returns the cross product of two `Vector3`s.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the cross product of the specified `Vector3`s.")]
        public static Vector3Export Cross(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export vector1,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export vector2
        )
        {
            return Vector3.Cross(vector1, vector2);
        }

        [ScriptExportMethod("Returns the cross product of the `Vector3` and the specified `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the cross product of the specified `Vector3`s.")]
        public Vector3Export Cross(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export vector
        )
        {
            return Cross(Reference, vector);
        }

//        [ScriptExportMethod("Returns the reflection of a `Vector3` off a surface that has the specified normal.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of reflecting the specified `Vector3` off a surface that has the specified normal `Vector3`.")]
//        public static Vector3Export Reflect(
//            [ScriptExportParameter("The `Vector3` to reflect.")] Vector3Export vector,
//            [ScriptExportParameter("The `Vector3` representing the normal of the surface to reflect from.")] Vector3Export normal
//        )
//        {
//            return Vector3.Reflect(vector, normal);
//        }

//        [ScriptExportMethod("Returns the reflection of the `Vector3` off a surface that has the specified normal.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of reflecting the `Vector3` off a surface that has the specified normal `Vector3`.")]
//        public Vector3Export Reflect(
//            [ScriptExportParameter("The `Vector3` representing the normal of the surface to reflect from.")] Vector3Export normal
//        )
//        {
//            return Reflect(Reference, normal);
//        }

        [ScriptExportMethod("Restricts a `Vector3` between a minimum and a maximum value.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of clamping the specified `Vector3` between the specified values.")]
        public static Vector3Export Clamp(
            [ScriptExportParameter("The `Vector3` to clamp.")] Vector3Export value1,
            [ScriptExportParameter("The `Vector3` representing the minimum value to clamp with.")] Vector3Export min,
            [ScriptExportParameter("The `Vector3` representing the maximum value to clamp with.")] Vector3Export max
        )
        {
            return Vector3.Clamp(value1, min, max);
        }

        [ScriptExportMethod("Restricts the `Vector3` between a minimum and a maximum value.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of clamping the `Vector3` between the specified values.")]
        public Vector3Export Clamp(
            [ScriptExportParameter("The `Vector3` representing the minimum value to clamp with.")] Vector3Export min,
            [ScriptExportParameter("The `Vector3` representing the maximum value to clamp with.")] Vector3Export max
        )
        {
            return Clamp(Reference, min, max);
        }

        [ScriptExportMethod("Performs a linear interpolation between two `Vector3`s based on the given weighting.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the result of interpolating the specified `Vector3`s by the specified `amount`.")]
        public static Vector3Export Lerp(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export value1,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value2,
            [ScriptExportParameter("The amount to interpolate by.")] float amount
        )
        {
            return Vector3.Lerp(value1, value2, amount);
        }

        [ScriptExportMethod("Adds two `Vector3`s together.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the sum of the specified `Vector3`s.")]
        public static Vector3Export Add(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export left,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Vector3.Add(left, right);
        }

        [ScriptExportMethod("Adds the specified `Vector3` to the `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the sum of the specified `Vector3`s.")]
        public Vector3Export Add(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Add(Reference, right);
        }

        [ScriptExportMethod("Subtracts the second `Vector3` from the first.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the subtraction of the specified `Vector3`s.")]
        public static Vector3Export Subtract(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export left,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Vector3.Subtract(left, right);
        }

        [ScriptExportMethod("Subtracts the specified `Vector3` from the `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the subtraction of the specified `Vector3`s.")]
        public Vector3Export Subtract(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Subtract(Reference, right);
        }

        [ScriptExportMethod("Multiplies two `Vector3`s together.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the multiplication of the specified `Vector3`s.")]
        public static Vector3Export Multiply(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export left,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Vector3.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies the `Vector3` by the specified `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the multiplication of the specified `Vector3`s.")]
        public Vector3Export Multiply(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Multiply(Reference, right);
        }

        [ScriptExportMethod("Multiplies a `Vector3` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the multiplication of the specified `Vector3` and the specified scalar.")]
        public static Vector3Export Multiply(
            [ScriptExportParameter("The `Vector3` to multiply.")] Vector3Export left,
            [ScriptExportParameter("The scalar to multiply by.")] float right
        )
        {
            return Vector3.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies a `Vector3` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the multiplication of the specified `Vector3` and the specified scalar.")]
        public static Vector3Export Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float left,
            [ScriptExportParameter("The `Vector3` to multiply.")] Vector3Export right
        )
        {
            return Vector3.Multiply(right, left);
        }

        [ScriptExportMethod("Multiplies the `Vector3` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the multiplication of the `Vector3` and the specified scalar.")]
        public Vector3Export Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float value
        )
        {
            return Multiply(Reference, value);
        }

        [ScriptExportMethod("Divides the first `Vector3` by the second.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the division of the specified `Vector3`s.")]
        public static Vector3Export Divide(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export left,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Vector3.Divide(left, right);
        }

        [ScriptExportMethod("Divides the `Vector3` by the specified `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the division of the specified `Vector3`s.")]
        public Vector3Export Divide(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export right
        )
        {
            return Divide(Reference, right);
        }

        [ScriptExportMethod("Divides the specified `Vector3` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the division of the specified `Vector3` and the specified scalar.")]
        public static Vector3Export Divide(
            [ScriptExportParameter("The `Vector3` to divide.")] Vector3Export left,
            [ScriptExportParameter("The scalar to divide by.")] float divisor
        )
        {
            return Vector3.Divide(left, divisor);
        }

        [ScriptExportMethod("Divides the `Vector3` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the division of the `Vector3` and the specified scalar.")]
        public Vector3Export Divide(
            [ScriptExportParameter("The scalar to divide by.")] float divisor
        )
        {
            return Divide(Reference, divisor);
        }

        [ScriptExportMethod("Negates the specified `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the negation of the specified `Vector3`.")]
        public static Vector3Export Negate(
            [ScriptExportParameter("The `Vector3` to negate.")] Vector3Export value
        )
        {
            return -value.Reference;
        }

        [ScriptExportMethod("Negates the `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the negation of the `Vector3`.")]
        public Vector3Export Negate()
        {
            return Negate(Reference);
        }

        [ScriptExportMethod("Returns the dot product of two `Vector3`s.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the dot product of the specified `Vector3`s.")]
        public static float Dot(
            [ScriptExportParameter("The first `Vector3`.")] Vector3Export vector1,
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export vector2
        )
        {
            return Vector3.Dot(vector1, vector2);
        }

        [ScriptExportMethod("Returns the dot product of the `Vector3` and the specified `Vector3`.")]
        [return: ScriptExportReturn("A new instance of `Vector3` representing the dot product of the specified `Vector3`s.")]
        public float Dot(
            [ScriptExportParameter("The second `Vector3`.")] Vector3Export vector2
        )
        {
            return Dot(Reference, vector2);
        }

//        [ScriptExportMethod("Returns a `Vector3` whose elements are the minimum of each of the pairs of elements in two specified `Vector3`s.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the minimum between the specified `Vector3`s.")]
//        public static Vector3Export Min(
//            [ScriptExportParameter("The first `Vector3`.")] Vector3Export value1,
//            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value2
//        )
//        {
//            return Vector3.Min(value1, value2);
//        }

//        [ScriptExportMethod("Returns a `Vector3` whose elements are the maximum of each of the pairs of elements in two specified `Vector3`s.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the maximum between the specified `Vector3`s.")]
//        public static Vector3Export Max(
//            [ScriptExportParameter("The first `Vector3`.")] Vector3Export value1,
//            [ScriptExportParameter("The second `Vector3`.")] Vector3Export value2
//        )
//        {
//            return Vector3.Max(value1, value2);
//        }

//        [ScriptExportMethod("Returns a `Vector3` whose elements are the absolute values of each of the specified `Vector3`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the absolute of the specified `Vector3`s.")]
//        public static Vector3Export Abs(
//            [ScriptExportParameter("The `Vector3` to get the absolute of.")] Vector3Export value
//        )
//        {
//            return Vector3.Abs(value);
//        }

//        [ScriptExportMethod("Returns a `Vector3` whose elements are the absolute values of each of the `Vector3`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the absolute of the `Vector3`.")]
//        public Vector3Export Abs()
//        {
//            return Abs(Reference);
//        }

//        [ScriptExportMethod("Returns a `Vector3` whose elements are the square root of each of the specified `Vector3`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the square root of the specified `Vector3`s.")]
//        public static Vector3Export SquareRoot(
//            [ScriptExportParameter("The `Vector3` to get the square root of.")] Vector3Export value
//        )
//        {
//            return Vector3.SquareRoot(value);
//        }

//        [ScriptExportMethod("Returns a `Vector3` whose elements are the square root of each of the `Vector3`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector3` representing the square root of the `Vector3`.")]
//        public Vector3Export SquareRoot()
//        {
//            return SquareRoot(Reference);
//        }

        public static Vector3Export operator +(Vector3Export left, Vector3Export right)
        {
            return new Vector3Export(left.Reference + right.Reference);
        }

        public static Vector3Export operator -(Vector3Export left, Vector3Export right)
        {
            return new Vector3Export(left.Reference - right.Reference);
        }

        public static Vector3Export operator *(Vector3Export left, Vector3Export right)
        {
            return new Vector3Export(left.Reference * right.Reference);
        }

        public static Vector3Export operator *(Vector3Export left, float right)
        {
            return new Vector3Export(left.Reference * right);
        }

        public static Vector3Export operator *(float left, Vector3Export right)
        {
            return new Vector3Export(left * right.Reference);
        }

//        public static Vector3Export operator /(Vector3Export left, Vector3Export right)
//        {
//            return new Vector3Export(left.Reference / right.Reference);
//        }

        public static Vector3Export operator /(Vector3Export value1, float value2)
        {
            return new Vector3Export(value1.Reference / value2);
        }

        public static Vector3Export operator -(Vector3Export value)
        {
            return new Vector3Export(-value.Reference);
        }

        public static bool operator ==(Vector3Export left, Vector3Export right)
        {
            if (left == null || right == null) return false;
            return left.Reference.Equals(right.Reference);
        }

        public static bool operator !=(Vector3Export left, Vector3Export right)
        {
            if (left == null || right == null) return true;
            return !left.Reference.Equals(right.Reference);
        }
    }
}