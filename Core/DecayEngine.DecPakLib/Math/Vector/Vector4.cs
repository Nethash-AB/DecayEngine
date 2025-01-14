using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Matrix;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace DecayEngine.DecPakLib.Math.Vector
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector4 : IEquatable<Vector4>
    {
        private float _x;
        private float _y;
        private float _z;
        private float _w;

        public float X
        {
            get => _x;
            set => _x = value.Fixed();
        }

        public float Y
        {
            get => _y;
            set => _y = value.Fixed();
        }

        public float Z
        {
            get => _z;
            set => _z = value.Fixed();
        }

        public float W
        {
            get => _w;
            set => _w = value.Fixed();
        }

        public Vector3 Xyz
        {
            get => new Vector3(_x, _y, _z);
            set
            {
                _x = value.X.Fixed();
                _y = value.Y.Fixed();
                _z = value.Z.Fixed();
            }
        }

        public float this[int index]
        {
            get =>
                index switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    3 => W,
                    _ => throw new IndexOutOfRangeException("You tried to access this vector at index: " + index)
                };

            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
                }
            }
        }

        public float Length => (float)System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        public float LengthSquared => X * X + Y * Y + Z * Z + W * W;

        public Vector4 Normalized
        {
            get
            {
                float scale = 1.0f / Length;
                return new Vector4(X * scale, Y * scale, Z * scale, W * scale);
            }
        }

        public Vector4 Negated => new Vector4(-_x, -_y, -_z, -_w);

        public Vector4(float x, float y, float z, float w)
        {
            _x = x.Fixed();
            _y = y.Fixed();
            _z = z.Fixed();
            _w = w.Fixed();
        }

        public Vector4(float value)
            : this(value, value, value, value)
        {
        }

        public Vector4(Vector2 v)
            : this(v.X, v.Y, 0f, 0f)
        {
        }

        public Vector4(Vector3 v)
            : this(v.X, v.Y, v.Z, 0f)
        {
        }

        public Vector4(Vector3 v, float w)
            : this(v.X, v.Y, v.Z, w)
        {
        }

        public Vector4(Vector4 v)
            : this(v.X, v.Y, v.Z, v.W)
        {
        }

        public static readonly Vector4 UnitX = new Vector4(1, 0, 0, 0);
        public static readonly Vector4 UnitY = new Vector4(0, 1, 0, 0);
        public static readonly Vector4 UnitZ = new Vector4(0, 0, 1, 0);
        public static readonly Vector4 UnitW = new Vector4(0, 0, 0, 1);
        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);
        public static readonly Vector4 One = new Vector4(1, 1, 1, 1);

        public static readonly Vector4 Right = UnitX;
        public static readonly Vector4 Up = UnitY;
        public static readonly Vector4 Forward = -UnitZ;

        public static readonly int SizeInBytes = Marshal.SizeOf<Vector4>();

        public static Vector4 Add(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Vector4 Subtract(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Vector4 Multiply(Vector4 vector, float scale)
        {
            return new Vector4(vector.X * scale, vector.Y * scale, vector.Z * scale, vector.W * scale);
        }

        public static Vector4 Multiply(Vector4 vector, Vector4 scale)
        {
            return new Vector4(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z, vector.W * scale.W);
        }

        public static Vector4 Divide(Vector4 vector, float scale)
        {
            return new Vector4(vector.X / scale, vector.Y / scale, vector.Z / scale, vector.W / scale);
        }

        public static Vector4 Divide(Vector4 vector, Vector4 scale)
        {
            return new Vector4(vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z, vector.W / scale.W);
        }

        public static Vector4 ComponentMin(Vector4 a, Vector4 b)
        {
            (float x, float y, float z, float w) = b;
            a.X = a.X < x ? a.X : x;
            a.Y = a.Y < y ? a.Y : y;
            a.Z = a.Z < z ? a.Z : z;
            a.W = a.W < w ? a.W : w;
            return a;
        }

        public static Vector4 ComponentMax(Vector4 a, Vector4 b)
        {
            (float x, float y, float z, float w) = b;
            a.X = a.X > x ? a.X : x;
            a.Y = a.Y > y ? a.Y : y;
            a.Z = a.Z > z ? a.Z : z;
            a.W = a.W > w ? a.W : w;
            return a;
        }

        public static Vector4 MagnitudeMin(Vector4 left, Vector4 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        public static Vector4 MagnitudeMax(Vector4 left, Vector4 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        public static Vector4 Clamp(Vector4 vec, Vector4 min, Vector4 max)
        {
            (float x, float y, float z, float w) = min;
            (float f, float f1, float z1, float w1) = max;
            vec.X = vec.X < x ? x : vec.X > f ? f : vec.X;
            vec.Y = vec.Y < y ? y : vec.Y > f1 ? f1 : vec.Y;
            vec.Z = vec.Z < z ? z : vec.Z > z1 ? z1 : vec.Z;
            vec.W = vec.W < w ? w : vec.W > w1 ? w1 : vec.W;
            return vec;
        }

        public static float Dot(Vector4 left, Vector4 right)
        {
            (float x, float y, float z, float w) = left;
            (float f, float f1, float z1, float w1) = right;
            return (x * f) + (y * f1) + (z * z1) + (w * w1);
        }

        public static Vector4 Lerp(Vector4 a, Vector4 b, float blend)
        {
            a.X = (blend * (b.X - a.X)) + a.X;
            a.Y = (blend * (b.Y - a.Y)) + a.Y;
            a.Z = (blend * (b.Z - a.Z)) + a.Z;
            a.W = (blend * (b.W - a.W)) + a.W;
            return a;
        }

        public static Vector4 BaryCentric(Vector4 a, Vector4 b, Vector4 c, float u, float v)
        {
            return a + (u * (b - a)) + (v * (c - a));
        }

        public static Vector4 Transform(Vector4 vec, Matrix4 mat)
        {
            return new Vector4(
                (vec.X * mat.Row1.X) + (vec.Y * mat.Row2.X) + (vec.Z * mat.Row3.X) + (vec.W * mat.Row4.X),
                (vec.X * mat.Row1.Y) + (vec.Y * mat.Row2.Y) + (vec.Z * mat.Row3.Y) + (vec.W * mat.Row4.Y),
                (vec.X * mat.Row1.Z) + (vec.Y * mat.Row2.Z) + (vec.Z * mat.Row3.Z) + (vec.W * mat.Row4.Z),
                (vec.X * mat.Row1.W) + (vec.Y * mat.Row2.W) + (vec.Z * mat.Row3.W) + (vec.W * mat.Row4.W));
        }

        public static Vector4 Transform(Vector4 vec, Quaternion quat)
        {
            Quaternion v = new Quaternion(vec.X, vec.Y, vec.Z, vec.W);
            Quaternion i = quat.Inverted;
            Quaternion t = Quaternion.Multiply(quat, v);
            v = Quaternion.Multiply(t, i);

            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vector4 Transform(Matrix4 mat, Vector4 vec)
        {
            return new Vector4(
                (mat.Row1.X * vec.X) + (mat.Row1.Y * vec.Y) + (mat.Row1.Z * vec.Z) + (mat.Row1.W * vec.W),
                (mat.Row2.X * vec.X) + (mat.Row2.Y * vec.Y) + (mat.Row2.Z * vec.Z) + (mat.Row2.W * vec.W),
                (mat.Row3.X * vec.X) + (mat.Row3.Y * vec.Y) + (mat.Row3.Z * vec.Z) + (mat.Row3.W * vec.W),
                (mat.Row4.X * vec.X) + (mat.Row4.Y * vec.Y) + (mat.Row4.Z * vec.Z) + (mat.Row4.W * vec.W));
        }

        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return Add(left, right);
        }

        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return Subtract(left, right);
        }

        public static Vector4 operator -(Vector4 vec)
        {
            return vec.Negated;
        }

        public static Vector4 operator *(Vector4 vec, float scale)
        {
            return Multiply(vec, scale);
        }

        public static Vector4 operator *(float scale, Vector4 vec)
        {
            return Multiply(vec, scale);
        }

        public static Vector4 operator *(Vector4 vec, Vector4 scale)
        {
            return Multiply(vec, scale);
        }

        public static Vector4 operator *(Vector4 vec, Matrix4 mat)
        {
            return Transform(vec, mat);
        }

        public static Vector4 operator *(Matrix4 mat, Vector4 vec)
        {
            return Transform(mat, vec);
        }

        public static Vector4 operator *(Quaternion quat, Vector4 vec)
        {
            return Transform(vec, quat);
        }

        public static Vector4 operator /(Vector4 vec, float scale)
        {
            return Divide(vec, scale);
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector4((float X, float Y, float Z, float W) values)
        {
            (float x, float y, float z, float w) = values;
            return new Vector4(x, y, z, w);
        }

        public override string ToString()
        {
            return string.Format("({0}{4} {1}{4} {2}{4} {3})", X, Y, Z, W, ",");
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _x.GetHashCode();
                hashCode = (hashCode * 397) ^ _y.GetHashCode();
                hashCode = (hashCode * 397) ^ _z.GetHashCode();
                hashCode = (hashCode * 397) ^ _w.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 vector4 && Equals(vector4);
        }

        public bool Equals(Vector4 other)
        {
            return
                _x.IsApproximately(other._x) &&
                _y.IsApproximately(other._y) &&
                _z.IsApproximately(other._z) &&
                _w.IsApproximately(other._w);
        }

        public void Deconstruct(out float x, out float y, out float z, out float w)
        {
            x = X;
            y = Y;
            z = Z;
            w = W;
        }
    }
}