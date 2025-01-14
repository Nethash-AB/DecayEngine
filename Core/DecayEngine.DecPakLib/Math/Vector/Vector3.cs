using System;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Matrix;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace DecayEngine.DecPakLib.Math.Vector
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3 : IEquatable<Vector3>
    {
        private float _x;
        private float _y;
        private float _z;

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

        public Vector2 Xy
        {
            get => new Vector2(_x, _y);
            set
            {
                X = value.X;
                Y = value.Y;
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
                    default:
                        throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
                }
            }
        }

        public float Length => (float)System.Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        public float LengthSquared => (X * X) + (Y * Y) + (Z * Z);

        public Vector3 Normalized
        {
            get
            {
                float scale = 1.0f / Length;
                return new Vector3(X * scale, Y * scale, Z * scale);
            }
        }

        public Vector3 Negated => new Vector3(-_x, -_y, -_z);

        public Vector3(float x, float y, float z)
        {
            _x = x.Fixed();
            _y = y.Fixed();
            _z = z.Fixed();
        }

        public Vector3(float value)
            : this(value, value, value)
        {
        }

        public Vector3(Vector2 v)
            : this(v.X, v.Y, 0f)
        {
        }

        public Vector3(Vector2 v, float z)
            : this(v.X, v.Y, z)
        {
        }

        public Vector3(Vector3 v)
            : this(v.X, v.Y, v.Z)
        {
        }

        public Vector3(Vector4 v)
            : this(v.X, v.Y, v.Z)
        {
        }

        public static readonly Vector3 UnitX = new Vector3(1, 0, 0);
        public static readonly Vector3 UnitY = new Vector3(0, 1, 0);
        public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);
        public static readonly Vector3 One = new Vector3(1, 1, 1);

        public static readonly Vector3 Right = UnitX;
        public static readonly Vector3 Up = UnitY;
        public static readonly Vector3 Forward = -UnitZ;

        public static readonly int SizeInBytes = Marshal.SizeOf<Vector3>();

        public static Vector3 Add(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 Multiply(Vector3 vector, float scale)
        {
            return new Vector3(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        public static Vector3 Multiply(Vector3 vector, Vector3 scale)
        {
            return new Vector3(vector.X * scale.X, vector.Y * scale.Y, vector.Z * scale.Z);
        }

        public static Vector3 Divide(Vector3 vector, float scale)
        {
            return new Vector3(vector.X / scale, vector.Y / scale, vector.Z / scale);
        }

        public static Vector3 Divide(Vector3 vector, Vector3 scale)
        {
            return new Vector3(vector.X / scale.X, vector.Y / scale.Y, vector.Z / scale.Z);
        }

        public static Vector3 ComponentMin(Vector3 a, Vector3 b)
        {
            (float x, float y, float z) = b;
            a.X = a.X < x ? a.X : x;
            a.Y = a.Y < y ? a.Y : y;
            a.Z = a.Z < z ? a.Z : z;
            return a;
        }

        public static Vector3 ComponentMax(Vector3 a, Vector3 b)
        {
            (float x, float y, float z) = b;
            a.X = a.X > x ? a.X : x;
            a.Y = a.Y > y ? a.Y : y;
            a.Z = a.Z > z ? a.Z : z;
            return a;
        }

        public static Vector3 MagnitudeMin(Vector3 left, Vector3 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        public static Vector3 MagnitudeMax(Vector3 left, Vector3 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        public static Vector3 Clamp(Vector3 vec, Vector3 min, Vector3 max)
        {
            (float x, float y, float z) = min;
            (float f, float f1, float z1) = max;
            vec.X = vec.X < x ? x : vec.X > f ? f : vec.X;
            vec.Y = vec.Y < y ? y : vec.Y > f1 ? f1 : vec.Y;
            vec.Z = vec.Z < z ? z : vec.Z > z1 ? z1 : vec.Z;
            return vec;
        }

        public static float Distance(Vector3 vec1, Vector3 vec2)
        {
            return (float)System.Math.Sqrt(
                (vec2.X - vec1.X) * (vec2.X - vec1.X) +
                (vec2.Y - vec1.Y) * (vec2.Y - vec1.Y) +
                (vec2.Z - vec1.Z) * (vec2.Z - vec1.Z)
            );
        }

        public static float DistanceSquared(Vector3 vec1, Vector3 vec2)
        {
            return
                (vec2.X - vec1.X) * (vec2.X - vec1.X) +
                (vec2.Y - vec1.Y) * (vec2.Y - vec1.Y) +
                (vec2.Z - vec1.Z) * (vec2.Z - vec1.Z);
        }

        public static float Dot(Vector3 left, Vector3 right)
        {
            (float x, float y, float z) = left;
            (float f, float f1, float z1) = right;
            return (x * f) + (y * f1) + (z * z1);
        }

        public static Vector3 Cross(Vector3 left, Vector3 right)
        {
            return new Vector3(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X
            );
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float blend)
        {
            a.X = (blend * (b.X - a.X)) + a.X;
            a.Y = (blend * (b.Y - a.Y)) + a.Y;
            a.Z = (blend * (b.Z - a.Z)) + a.Z;
            return a;
        }

        public static Vector3 BaryCentric(Vector3 a, Vector3 b, Vector3 c, float u, float v)
        {
            return a + (u * (b - a)) + (v * (c - a));
        }

        public static Vector3 TransformVector(Vector3 vec, Matrix4 mat)
        {
            return new Vector3(
                vec.X * mat.Row1.X + vec.Y * mat.Row2.X + vec.Z * mat.Row3.X,
                vec.X * mat.Row1.Y + vec.Y * mat.Row2.Y + vec.Z * mat.Row3.Y,
                vec.X * mat.Row1.Z + vec.Y * mat.Row2.Z + vec.Z * mat.Row3.Z
            );
        }

        public static Vector3 TransformNormal(Vector3 norm, Matrix4 mat)
        {
            Matrix4 inverse = Matrix4.Invert(mat);
            return TransformNormalInverse(norm, inverse);
        }

        public static Vector3 TransformNormalInverse(Vector3 norm, Matrix4 invMat)
        {
            return new Vector3(
                norm.X * invMat.Row1.X + norm.Y * invMat.Row1.Y + norm.Z * invMat.Row1.Z,
                norm.X * invMat.Row2.X + norm.Y * invMat.Row2.Y + norm.Z * invMat.Row2.Z,
                norm.X * invMat.Row3.X + norm.Y * invMat.Row3.Y + norm.Z * invMat.Row3.Z
            );
        }

        public static Vector3 TransformPosition(Vector3 pos, Matrix4 mat)
        {
            return new Vector3(
                pos.X * mat.Row1.X + pos.Y * mat.Row2.X + pos.Z * mat.Row3.X + mat.Row4.X,
                pos.X * mat.Row1.Y + pos.Y * mat.Row2.Y + pos.Z * mat.Row3.Y + mat.Row4.Y,
                pos.X * mat.Row1.Z + pos.Y * mat.Row2.Z + pos.Z * mat.Row3.Z + mat.Row4.Z
            );
        }

        public static Vector3 Transform(Vector3 vec, Matrix3 mat)
        {
            return new Vector3(
                vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X,
                vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y,
                vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z
            );
        }

        public static Vector4 Transform(Vector3 vec, Matrix4 mat)
        {
            return new Vector4(
                vec.X * mat.M11 + vec.Y * mat.M21 + vec.Z * mat.M31 + mat.M41,
                vec.X * mat.M12 + vec.Y * mat.M22 + vec.Z * mat.M32 + mat.M42,
                vec.X * mat.M13 + vec.Y * mat.M23 + vec.Z * mat.M33 + mat.M43,
                vec.X * mat.M14 + vec.Y * mat.M24 + vec.Z * mat.M34 + mat.M44
            );
        }

        public static Vector3 Transform(Vector3 vec, Quaternion quat)
        {
            Vector3 xyz = quat.Xyz;
            Vector3 temp = Cross(xyz, vec);
            Vector3 temp2 = Multiply(vec, quat.W);
            temp = Add(temp, temp2);
            temp2 = Cross(xyz, temp);
            temp2 = Multiply(temp2, 2f);
            return Add(vec, temp2);
        }

        public static Vector3 Transform(Matrix3 mat, Vector3 vec)
        {
            return new Vector3(
                mat.Row0.X * vec.X + mat.Row0.Y * vec.Y + mat.Row0.Z * vec.Z,
                mat.Row1.X * vec.X + mat.Row1.Y * vec.Y + mat.Row1.Z * vec.Z,
                mat.Row2.X * vec.X + mat.Row2.Y * vec.Y + mat.Row2.Z * vec.Z
            );
        }

        public static Vector3 TransformPerspective(Vector3 vec, Matrix4 mat)
        {
            Vector4 v = new Vector4(vec.X, vec.Y, vec.Z, 1);
            v = Vector4.Transform(v, mat);
            return new Vector3(
                v.X / v.W,
                v.Y / v.W,
                v.Z / v.W
            );
        }

        public static float CalculateAngle(Vector3 first, Vector3 second)
        {
            float temp = Dot(first, second);
            return (float)System.Math.Acos(MathHelper.Clamp(temp / (first.Length * second.Length), -1.0, 1.0));
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return Add(left, right);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return Subtract(left, right);
        }

        public static Vector3 operator -(Vector3 vec)
        {
            return vec.Negated;
        }

        public static Vector3 operator *(Vector3 vec, Matrix4 mat)
        {
            return Transform(vec, mat).Xyz;
        }

        public static Vector3 operator *(Vector3 vec, float scale)
        {
            return Multiply(vec, scale);
        }

        public static Vector3 operator *(float scale, Vector3 vec)
        {
            return Multiply(vec, scale);
        }

        public static Vector3 operator *(Vector3 vec, Vector3 scale)
        {
            return Multiply(vec, scale);
        }

        public static Vector3 operator *(Vector3 vec, Matrix3 mat)
        {
            return Transform(vec, mat);
        }

        public static Vector3 operator *(Matrix3 mat, Vector3 vec)
        {
            return Transform(mat, vec);
        }

        public static Vector3 operator *(Quaternion quat, Vector3 vec)
        {
            return Transform(vec, quat);
        }

        public static Vector3 operator /(Vector3 vec, float scale)
        {
            return Divide(vec, scale);
        }

        public static Vector3 operator /(Vector3 vec, Vector3 scale)
        {
            return Divide(vec, scale);
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector3((float X, float Y, float Z) values)
        {
            (float x, float y, float z) = values;
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return string.Format("({0}{3} {1}{3} {2})", X, Y, Z, ",");
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 vector3 && Equals(vector3);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _x.GetHashCode();
                hashCode = (hashCode * 397) ^ _y.GetHashCode();
                hashCode = (hashCode * 397) ^ _z.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(Vector3 other)
        {
            return
                _x.IsApproximately(other._x) &&
                _y.IsApproximately(other._y) &&
                _z.IsApproximately(other._z);
        }

        public void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}