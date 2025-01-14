using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;

#pragma warning disable 660,661

namespace DecayEngine.ModuleSDK.Math.Exports
{
    [ScriptExportClass("Vector2", "Represents a SIMD enhanced Vector with 2 dimensions.", typeof(MathNamespaceExport))]
    public class Vector2Export : ExportableManagedObject<Vector2>
    {
        [ScriptExportProperty("The X component of the `Vector2`.")]
        public float X
        {
            get => Reference.X;
            set => Reference.X = value;
        }

        [ScriptExportProperty("The Y component of the `Vector2`.")]
        public float Y
        {
            get => Reference.Y;
            set => Reference.Y = value;
        }

        [ScriptExportProperty("Returns a new instance of `Vector2` representing a vector whose `X` and `Y` components are Zero.")]
        public static Vector2Export Zero => new Vector2Export(Vector2.Zero);
        [ScriptExportProperty("Returns a new instance of `Vector2` representing a vector whose `X` and `Y` components are One.")]
        public static Vector2Export One => new Vector2Export(Vector2.One);
        [ScriptExportProperty("Returns a new instance of `Vector2` representing a vector whose `X` component is One and all other components are Zero.")]
        public static Vector2Export UnitX => new Vector2Export(Vector2.UnitX);
        [ScriptExportProperty("Returns a new instance of `Vector2` representing a vector whose `Y` component is One and all other components are Zero.")]
        public static Vector2Export UnitY => new Vector2Export(Vector2.UnitY);

        public override int Type => (int) ManagedExportType.Vector2;
        public override string SubType => "float";

        private Vector2Export(Vector2 vector) : base(vector)
        {
        }

        public Vector2Export(ByReference<Vector2> referencePointer) : base(referencePointer)
        {
        }

        [ScriptExportConstructor]
        public Vector2Export(
            [ScriptExportParameter("The value of all the components of the `Vector2`.")] float value
        ) : this(new Vector2(value))
        {
        }

        [ScriptExportConstructor]
        public Vector2Export(
            [ScriptExportParameter("The value of the `X` component of the `Vector2`.")] float x,
            [ScriptExportParameter("The value of the `Y` component of the `Vector2`.")] float y
        ) : this(new Vector2(x, y))
        {
        }

        public static implicit operator Vector2Export(Vector2 obj)
        {
            return new Vector2Export(obj);
        }

        [ScriptExportMethod("Returns the length of the `Vector2`.")]
        [return: ScriptExportReturn("The length of the `Vector2`.")]
        public float Length()
        {
            return Reference.Length;
        }

        [ScriptExportMethod("Returns the length of the `Vector2` squared.")]
        [return: ScriptExportReturn("The length of the `Vector2` squared.")]
        public float LengthSquared()
        {
            return Reference.LengthSquared;
        }

        [ScriptExportMethod("Computes the Euclidean distance between the two given `Vector2`.")]
        [return: ScriptExportReturn("The Euclidean distance between the two given `Vector2`.")]
        public static float Distance(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export value1,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2)
        {
            return Vector2.Distance(value1, value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance between the `Vector2` and the specified `Vector2`.")]
        [return: ScriptExportReturn("The Euclidean distance between the specified `Vector2`s.")]
        public float Distance(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2)
        {
            return Distance(Reference, value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance squared between the two given `Vector2`.")]
        [return: ScriptExportReturn("The Euclidean distance squared between the two given `Vector2`.")]
        public static float DistanceSquared(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export value1,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2
        )
        {
            return Vector2.DistanceSquared(value1, value2);
        }

        [ScriptExportMethod("Computes the Euclidean distance squared between the `Vector2` and the specified `Vector2`.")]
        [return: ScriptExportReturn("The Euclidean distance squared between the specified `Vector2`s.")]
        public float DistanceSquared(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2
        )
        {
            return DistanceSquared(Reference, value2);
        }

        [ScriptExportMethod("Returns a `Vector2` with the same direction as the specified `Vector2`, but with a length of One.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of normalizing the specified `Vector2`.")]
        public static Vector2Export Normalize(
            [ScriptExportParameter("The `Vector2` to normalize.")] Vector2Export value
        )
        {
            return value.Reference.Normalized;
        }

        [ScriptExportMethod("Returns a `Vector2` with the same direction as the `Vector2`, but with a length of One.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of normalizing the `Vector2`.")]
        public Vector2Export Normalize()
        {
            return Normalize(Reference);
        }
//
//        [ScriptExportMethod("Returns the reflection of a `Vector2` off a surface that has the specified normal.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of reflecting the specified `Vector2` off a surface that has the specified normal `Vector2`.")]
//        public static Vector2Export Reflect(
//            [ScriptExportParameter("The `Vector2` to reflect.")] Vector2Export vector,
//            [ScriptExportParameter("The `Vector2` representing the normal of the surface to reflect from.")] Vector2Export normal
//        )
//        {
//            return Vector2.Reflect(vector, normal);
//        }

//        [ScriptExportMethod("Returns the reflection of the `Vector2` off a surface that has the specified normal.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of reflecting the `Vector2` off a surface that has the specified normal `Vector2`.")]
//        public Vector2Export Reflect(
//            [ScriptExportParameter("The `Vector2` representing the normal of the surface to reflect from.")] Vector2Export normal
//        )
//        {
//            return Reflect(Reference, normal);
//        }

        [ScriptExportMethod("Restricts a `Vector2` between a minimum and a maximum value.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of clamping the specified `Vector2` between the specified values.")]
        public static Vector2Export Clamp(
            [ScriptExportParameter("The `Vector2` to clamp.")] Vector2Export value1,
            [ScriptExportParameter("The `Vector2` representing the minimum value to clamp with.")] Vector2Export min,
            [ScriptExportParameter("The `Vector2` representing the maximum value to clamp with.")] Vector2Export max
        )
        {
            return Vector2.Clamp(value1, min, max);
        }

        [ScriptExportMethod("Restricts the `Vector2` between a minimum and a maximum value.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of clamping the `Vector2` between the specified values.")]
        public Vector2Export Clamp(
            [ScriptExportParameter("The `Vector2` representing the minimum value to clamp with.")] Vector2Export min,
            [ScriptExportParameter("The `Vector2` representing the maximum value to clamp with.")] Vector2Export max
        )
        {
            return Clamp(Reference, min, max);
        }

        [ScriptExportMethod("Performs a linear interpolation between two `Vector2`s based on the given weighting.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the result of interpolating the specified `Vector2`s by the specified `amount`.")]
        public static Vector2Export Lerp(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export value1,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2,
            [ScriptExportParameter("The amount to interpolate by.")] float amount
        )
        {
            return Vector2.Lerp(value1, value2, amount);
        }

        [ScriptExportMethod("Adds two `Vector2`s together.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the sum of the specified `Vector2`s.")]
        public static Vector2Export Add(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export left,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Vector2.Add(left, right);
        }

        [ScriptExportMethod("Adds the specified `Vector2` to the `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the sum of the specified `Vector2`s.")]
        public Vector2Export Add(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Add(Reference, right);
        }

        [ScriptExportMethod("Subtracts the second `Vector2` from the first.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the subtraction of the specified `Vector2`s.")]
        public static Vector2Export Subtract(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export left,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Vector2.Subtract(left, right);
        }

        [ScriptExportMethod("Subtracts the specified `Vector2` from the `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the subtraction of the specified `Vector2`s.")]
        public Vector2Export Subtract(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Subtract(Reference, right);
        }

        [ScriptExportMethod("Multiplies two `Vector2`s together.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the multiplication of the specified `Vector2`s.")]
        public static Vector2Export Multiply(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export left,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Vector2.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies the `Vector2` by the specified `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the multiplication of the specified `Vector2`s.")]
        public Vector2Export Multiply(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Multiply(Reference, right);
        }

        [ScriptExportMethod("Multiplies a `Vector2` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the multiplication of the specified `Vector2` and the specified scalar.")]
        public static Vector2Export Multiply(
            [ScriptExportParameter("The `Vector2` to multiply.")] Vector2Export left,
            [ScriptExportParameter("The scalar to multiply by.")] float right
        )
        {
            return Vector2.Multiply(left, right);
        }

        [ScriptExportMethod("Multiplies a `Vector2` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the multiplication of the specified `Vector2` and the specified scalar.")]
        public static Vector2Export Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float left,
            [ScriptExportParameter("The `Vector2` to multiply.")] Vector2Export right
        )
        {
            return left * right.Reference;
        }

        [ScriptExportMethod("Multiplies the `Vector2` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the multiplication of the `Vector2` and the specified scalar.")]
        public Vector2Export Multiply(
            [ScriptExportParameter("The scalar to multiply by.")] float right
        )
        {
            return Multiply(Reference, right);
        }

        [ScriptExportMethod("Divides the first `Vector2` by the second.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the division of the specified `Vector2`s.")]
        public static Vector2Export Divide(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export left,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Vector2.Divide(left, right);
        }

        [ScriptExportMethod("Divides the `Vector2` by the specified `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the division of the specified `Vector2`s.")]
        public Vector2Export Divide(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export right
        )
        {
            return Divide(Reference, right);
        }

        [ScriptExportMethod("Divides the specified `Vector2` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the division of the specified `Vector2` and the specified scalar.")]
        public static Vector2Export Divide(
            [ScriptExportParameter("The `Vector2` to divide.")] Vector2Export left,
            [ScriptExportParameter("The scalar to divide by.")] float divisor
        )
        {
            return Vector2.Divide(left, divisor);
        }

        [ScriptExportMethod("Divides the `Vector2` by a specified scalar.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the division of the `Vector2` and the specified scalar.")]
        public Vector2Export Divide(
            [ScriptExportParameter("The scalar to divide by.")] float divisor
        )
        {
            return Divide(Reference, divisor);
        }

        [ScriptExportMethod("Negates the specified `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the negation of the specified `Vector2`.")]
        public static Vector2Export Negate(
            [ScriptExportParameter("The `Vector2` to negate.")] Vector2Export value
        )
        {
            return -value.Reference;
        }

        [ScriptExportMethod("Negates the `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the negation of the `Vector2`.")]
        public Vector2Export Negate()
        {
            return Negate(Reference);
        }

        [ScriptExportMethod("Returns the dot product of two `Vector2`s.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the dot product of the specified `Vector2`s.")]
        public static float Dot(
            [ScriptExportParameter("The first `Vector2`.")] Vector2Export vector1,
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export vector2
        )
        {
            return Vector2.Dot(vector1, vector2);
        }

        [ScriptExportMethod("Returns the dot product of the `Vector2` and the specified `Vector2`.")]
        [return: ScriptExportReturn("A new instance of `Vector2` representing the dot product of the specified `Vector2`s.")]
        public float Dot(
            [ScriptExportParameter("The second `Vector2`.")] Vector2Export vector2
        )
        {
            return Dot(Reference, vector2);
        }

//        [ScriptExportMethod("Returns a `Vector2` whose elements are the minimum of each of the pairs of elements in two specified `Vector2`s.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the minimum between the specified `Vector2`s.")]
//        public Vector2Export Min(
//            [ScriptExportParameter("The first `Vector2`.")] Vector2Export value1,
//            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2
//        )
//        {
//            return Vector2.Min(value1, value2);
//        }

//        [ScriptExportMethod("Returns a `Vector2` whose elements are the maximum of each of the pairs of elements in two specified `Vector2`s.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the maximum between the specified `Vector2`s.")]
//        public static Vector2Export Max(
//            [ScriptExportParameter("The first `Vector2`.")] Vector2Export value1,
//            [ScriptExportParameter("The second `Vector2`.")] Vector2Export value2
//        )
//        {
//            return Vector2.Max(value1, value2);
//        }

//        [ScriptExportMethod("Returns a `Vector2` whose elements are the absolute values of each of the specified `Vector2`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the absolute of the specified `Vector2`s.")]
//        public static Vector2Export Abs(
//            [ScriptExportParameter("The `Vector2` to get the absolute of.")] Vector2Export value
//        )
//        {
//            return Vector2.Abs(value);
//        }

//        [ScriptExportMethod("Returns a `Vector2` whose elements are the absolute values of each of the `Vector2`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the absolute of the `Vector2`.")]
//        public Vector2Export Abs()
//        {
//            return Abs(Reference);
//        }

//        [ScriptExportMethod("Returns a `Vector2` whose elements are the square root of each of the specified `Vector2`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the square root of the specified `Vector2`s.")]
//        public static Vector2Export SquareRoot(
//            [ScriptExportParameter("The `Vector2` to get the square root of.")] Vector2Export value
//        )
//        {
//            return Vector2.SquareRoot(value);
//        }

//        [ScriptExportMethod("Returns a `Vector2` whose elements are the square root of each of the `Vector2`'s elements.")]
//        [return: ScriptExportReturn("A new instance of `Vector2` representing the square root of the `Vector2`.")]
//        public Vector2Export SquareRoot()
//        {
//            return SquareRoot(Reference);
//        }

        public static Vector2Export operator +(Vector2Export left, Vector2Export right)
        {
            return new Vector2Export(left.Reference + right.Reference);
        }

        public static Vector2Export operator -(Vector2Export left, Vector2Export right)
        {
            return new Vector2Export(left.Reference - right.Reference);
        }

        public static Vector2Export operator *(Vector2Export left, Vector2Export right)
        {
            return new Vector2Export(left.Reference * right.Reference);
        }

        public static Vector2Export operator *(Vector2Export left, float right)
        {
            return new Vector2Export(left.Reference * right);
        }

        public static Vector2Export operator *(float left, Vector2Export right)
        {
            return new Vector2Export(left * right.Reference);
        }

//        public static Vector2Export operator /(Vector2Export left, Vector2Export right)
//        {
//            return new Vector2Export(left.Reference / right.Reference);
//        }

        public static Vector2Export operator /(Vector2Export value1, float value2)
        {
            return new Vector2Export(value1.Reference / value2);
        }

        public static Vector2Export operator -(Vector2Export value)
        {
            return new Vector2Export(-value.Reference);
        }

        public static bool operator ==(Vector2Export left, Vector2Export right)
        {
            if (left == null || right == null) return false;
            return left.Reference.Equals(right.Reference);
        }

        public static bool operator !=(Vector2Export left, Vector2Export right)
        {
            if (left == null || right == null) return true;
            return !left.Reference.Equals(right.Reference);
        }
    }
}