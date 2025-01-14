using System;
using System.Runtime.InteropServices;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace DecayEngine.DecPakLib.Math.Vector
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector2 : IEquatable<Vector2>
    {
        private float _x;
        private float _y;

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

        public float this[int index]
        {
            get =>
                index switch
                {
                    0 => X,
                    1 => Y,
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
                    default:
                        throw new IndexOutOfRangeException("You tried to set this vector at index: " + index);
                }
            }
        }

        public float Length => (float)System.Math.Sqrt((X * X) + (Y * Y));
        public float LengthSquared => (X * X) + (Y * Y);

        public Vector2 PerpendicularRight => new Vector2(Y, -X);
        public Vector2 PerpendicularLeft => new Vector2(-Y, X);

        public Vector2 Normalized
        {
            get
            {
                float scale = 1.0f / Length;
                return new Vector2(X * scale, Y * scale);
            }
        }

        public Vector2 Negated => new Vector2(-_x, -_y);

        public Vector2(float x, float y)
        {
            _x = x.Fixed();
            _y = y.Fixed();
        }

        public Vector2(float value)
            : this(value, value)
        {
        }

        public Vector2(Vector2 v)
            : this(v.X, v.Y)
        {
        }

        public Vector2(Vector3 v)
            : this(v.X, v.Y)
        {
        }

        public Vector2(Vector4 v)
            : this(v.X, v.Y)
        {
        }

        public static readonly Vector2 UnitX = new Vector2(1, 0);
        public static readonly Vector2 UnitY = new Vector2(0, 1);
        public static readonly Vector2 Zero = new Vector2(0, 0);
        public static readonly Vector2 One = new Vector2(1, 1);

        public static readonly Vector2 Right = UnitX;
        public static readonly Vector2 Up = UnitY;

        public static readonly int SizeInBytes = Marshal.SizeOf<Vector2>();

        public static Vector2 Add(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 Multiply(Vector2 vector, float scale)
        {
            return new Vector2(vector.X * scale, vector.Y * scale);
        }

        public static Vector2 Multiply(Vector2 vector, Vector2 scale)
        {
            return new Vector2(vector.X * scale.X, vector.Y * scale.Y);
        }

        public static Vector2 Divide(Vector2 vector, float scale)
        {
            return new Vector2(vector.X / scale, vector.Y / scale);
        }

        public static Vector2 Divide(Vector2 vector, Vector2 scale)
        {
            return new Vector2(vector.X / scale.X, vector.Y / scale.Y);
        }

        public static Vector2 ComponentMin(Vector2 a, Vector2 b)
        {
            (float x, float y) = b;
            a.X = a.X < x ? a.X : x;
            a.Y = a.Y < y ? a.Y : y;
            return a;
        }

        public static Vector2 ComponentMax(Vector2 a, Vector2 b)
        {
            (float x, float y) = b;
            a.X = a.X > x ? a.X : x;
            a.Y = a.Y > y ? a.Y : y;
            return a;
        }

        public static Vector2 MagnitudeMin(Vector2 left, Vector2 right)
        {
            return left.LengthSquared < right.LengthSquared ? left : right;
        }

        public static Vector2 MagnitudeMax(Vector2 left, Vector2 right)
        {
            return left.LengthSquared >= right.LengthSquared ? left : right;
        }

        public static Vector2 Clamp(Vector2 vec, Vector2 min, Vector2 max)
        {
            (float x, float y) = min;
            (float f, float f1) = max;
            vec.X = vec.X < x ? x : vec.X > f ? f : vec.X;
            vec.Y = vec.Y < y ? y : vec.Y > f1 ? f1 : vec.Y;
            return vec;
        }

        public static float Distance(Vector2 vec1, Vector2 vec2)
        {
            return (float)System.Math.Sqrt((vec2.X - vec1.X) * (vec2.X - vec1.X) + (vec2.Y - vec1.Y) * (vec2.Y - vec1.Y));
        }

        public static float DistanceSquared(Vector2 vec1, Vector2 vec2)
        {
            return (vec2.X - vec1.X) * (vec2.X - vec1.X) + (vec2.Y - vec1.Y) * (vec2.Y - vec1.Y);
        }

        public static float Dot(Vector2 left, Vector2 right)
        {
            (float x, float y) = left;
            (float f, float f1) = right;
            return (x * f) + (y * f1);
        }

        public static float PerpDot(Vector2 left, Vector2 right)
        {
            (float x, float y) = left;
            (float f, float f1) = right;
            return (x * f1) - (y * f);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float blend)
        {
            (float x, float y) = b;
            a.X = (blend * (x - a.X)) + a.X;
            a.Y = (blend * (y - a.Y)) + a.Y;
            return a;
        }

        public static Vector2 BaryCentric(Vector2 a, Vector2 b, Vector2 c, float u, float v)
        {
            return a + (u * (b - a)) + (v * (c - a));
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return Add(left, right);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return Subtract(left, right);
        }

        public static Vector2 operator -(Vector2 vec)
        {
            return vec.Negated;
        }

        public static Vector2 operator *(Vector2 vec, float scale)
        {
            return Multiply(vec, scale);
        }

        public static Vector2 operator *(float scale, Vector2 vec)
        {
            return Multiply(vec, scale);
        }

        public static Vector2 operator *(Vector2 vec, Vector2 scale)
        {
            return Multiply(vec, scale);
        }

        public static Vector2 operator /(Vector2 vec, float scale)
        {
            return Divide(vec, scale);
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector2((float X, float Y) values)
        {
            (float x, float y) = values;
            return new Vector2(x, y);
        }

        public override string ToString()
        {
            return string.Format("({0}{2} {1})", X, Y, ",");
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 vector2 && Equals(vector2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
            }
        }

        public bool Equals(Vector2 other)
        {
            return
                _x.IsApproximately(other._x) &&
                _y.IsApproximately(other._y);
        }

        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }
    }
}