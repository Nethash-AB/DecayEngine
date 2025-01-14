using System;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Matrix;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace DecayEngine.DecPakLib.Math.Vector
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>
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

        public float Length => (float)System.Math.Sqrt((W * W) + Xyz.LengthSquared);
        public float LengthSquared => (W * W) + Xyz.LengthSquared;

        public Vector4 AxisAngle
        {
            get
            {
                Quaternion q = this;
                if (System.Math.Abs(q.W) > 1.0f)
                {
                    q = q.Normalized;
                }

                Vector4 result = new Vector4
                {
                    W = 2.0f * (float)System.Math.Acos(q.W)
                };

                float den = (float)System.Math.Sqrt(1.0 - (q.W * q.W));
                if (den > 0.0001f)
                {
                    result.Xyz = q.Xyz / den;
                }
                else
                {
                    result.Xyz = Vector3.UnitX;
                }

                return result;
            }
        }

        public Vector3 EulerAngles
        {
            get
            {
                const float singularityThreshold = 0.4999995f;

                float sqw = W * W;
                float sqx = X * X;
                float sqy = Y * Y;
                float sqz = Z * Z;
                float unit = sqx + sqy + sqz + sqw;
                float singularityTest = (X * Z) + (W * Y);

                if (singularityTest > singularityThreshold * unit)
                {
                    return new Vector3(
                        (float)(2 * System.Math.Atan2(X, W)),
                        MathHelper.PiOver2,
                        0f
                    );
                }

                if (singularityTest < -singularityThreshold * unit)
                {
                    return new Vector3(
                        (float)(-2 * System.Math.Atan2(X, W)),
                        -MathHelper.PiOver2,
                        0f
                    );
                }

                return new Vector3(
                    (float)System.Math.Atan2(2 * ((W * Z) - (X * Y)), sqw + sqx - sqy - sqz),
                    (float)System.Math.Asin(2 * singularityTest / unit),
                    (float)System.Math.Atan2(2 * ((W * X) - (Y * Z)), sqw - sqx - sqy + sqz)
                );
            }
        }

        public Quaternion Conjugate => new Quaternion(-Xyz, W);

        public Quaternion Normalized
        {
            get
            {
                float scale = 1.0f / Length;
                return new Quaternion(Xyz * scale, W * scale);
            }
        }

        public Quaternion Inverted
        {
            get
            {
                float lengthSq = LengthSquared;
                if (lengthSq.IsZero()) return this;

                float i = 1.0f / lengthSq;
                return new Quaternion(Xyz * -i, W * i);
            }
        }

        public Quaternion(float x, float y, float z, float w)
        {
            _x = x.Fixed();
            _y = y.Fixed();
            _z = z.Fixed();
            _w = w.Fixed();
        }

        public Quaternion(Vector3 v, float w)
            : this(v.X, v.Y, v.Z, w)
        {
        }

        public Quaternion(float rotationX, float rotationY, float rotationZ)
        {
            rotationX *= 0.5f;
            rotationY *= 0.5f;
            rotationZ *= 0.5f;

            float c1 = (float)System.Math.Cos(rotationX);
            float c2 = (float)System.Math.Cos(rotationY);
            float c3 = (float)System.Math.Cos(rotationZ);
            float s1 = (float)System.Math.Sin(rotationX);
            float s2 = (float)System.Math.Sin(rotationY);
            float s3 = (float)System.Math.Sin(rotationZ);

            _x = (s1 * c2 * c3 + c1 * s2 * s3).Fixed();
            _y = (c1 * s2 * c3 - s1 * c2 * s3).Fixed();
            _z = (c1 * c2 * s3 + s1 * s2 * c3).Fixed();
            _w = (c1 * c2 * c3 - s1 * s2 * s3).Fixed();
        }

        public Quaternion(Vector3 eulerAngles)
            : this(eulerAngles.X, eulerAngles.Y, eulerAngles.Z)
        {
        }

        public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);

        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                left.Xyz + right.Xyz,
                left.W + right.W);
        }

        public static Quaternion Subtract(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                left.Xyz - right.Xyz,
                left.W - right.W);
        }

        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                (right.W * left.Xyz) + (left.W * right.Xyz) + Vector3.Cross(left.Xyz, right.Xyz),
                (left.W * right.W) - Vector3.Dot(left.Xyz, right.Xyz));
        }

        public static Quaternion Multiply(Quaternion quaternion, float scale)
        {
            return new Quaternion
            (
                quaternion.X * scale,
                quaternion.Y * scale,
                quaternion.Z * scale,
                quaternion.W * scale
            );
        }

        public static Quaternion FromAxisAngle(Vector3 axis, float angle)
        {
            if (axis.LengthSquared.IsZero())
            {
                return Identity;
            }

            Quaternion result = Identity;

            angle *= 0.5f;
            axis = axis.Normalized;
            result.Xyz = axis * (float)System.Math.Sin(angle);
            result.W = (float)System.Math.Cos(angle);

            return result.Normalized;
        }

        public static Quaternion FromDirection(Vector3 forward)
        {
            return FromDirection(forward, Vector3.Up);
        }

        public static Quaternion FromDirection(Vector3 forward, Vector3 up)
        {
            if (forward.Length.IsZero() || up.Length.IsZero()) return Identity;

            Vector3 normalizedForward = forward.Normalized;
            Vector3 normalizedUp = up.Normalized;

            if (normalizedForward == normalizedUp) return Identity;

            Matrix4 rotationMatrix = Matrix4.Identity;
            rotationMatrix.Forward = normalizedForward;
            rotationMatrix.Right = Vector3.Cross(normalizedForward, normalizedUp);
            rotationMatrix.Up = Vector3.Cross(-rotationMatrix.Forward, rotationMatrix.Right);

            return rotationMatrix.ExtractRotation();
        }

        public static Quaternion FromEulerAngles(float pitch, float yaw, float roll)
        {
            return new Quaternion(pitch, yaw, roll);
        }

        public static Quaternion FromEulerAngles(Vector3 eulerAngles)
        {
            return new Quaternion(eulerAngles);
        }

        public static Quaternion FromMatrix(Matrix3 matrix)
        {
            float trace = matrix.Trace;

            if (trace > 0)
            {
                float s = (float)System.Math.Sqrt(trace + 1) * 2;
                float invS = 1f / s;

                return new Quaternion(
                    (matrix.Row2.Y - matrix.Row1.Z) * invS,
                    (matrix.Row0.Z - matrix.Row2.X) * invS,
                    (matrix.Row1.X - matrix.Row0.Y) * invS,
                    s * 0.25f
                );
            }

            float m00 = matrix.Row0.X, m11 = matrix.Row1.Y, m22 = matrix.Row2.Z;

            if (m00 > m11 && m00 > m22)
            {
                float s = (float)System.Math.Sqrt(1 + m00 - m11 - m22) * 2;
                float invS = 1f / s;

                return new Quaternion(
                    s * 0.25f,
                    (matrix.Row0.Y + matrix.Row1.X) * invS,
                    (matrix.Row0.Z + matrix.Row2.X) * invS,
                    (matrix.Row2.Y - matrix.Row1.Z) * invS
                );
            }

            if (m11 > m22)
            {
                float s = (float)System.Math.Sqrt(1 + m11 - m00 - m22) * 2;
                float invS = 1f / s;

                return new Quaternion(
                    (matrix.Row0.Y + matrix.Row1.X) * invS,
                    s * 0.25f,
                    (matrix.Row1.Z + matrix.Row2.Y) * invS,
                    (matrix.Row0.Z - matrix.Row2.X) * invS
                );
            }
            else
            {
                float s = (float)System.Math.Sqrt(1 + m22 - m00 - m11) * 2;
                float invS = 1f / s;

                return new Quaternion(
                    (matrix.Row0.Z + matrix.Row2.X) * invS,
                    (matrix.Row1.Z + matrix.Row2.Y) * invS,
                    s * 0.25f,
                    (matrix.Row1.X - matrix.Row0.Y) * invS
                );
            }
        }

        public static Quaternion Slerp(Quaternion q1, Quaternion q2, float blend)
        {
            if (q1.LengthSquared.IsZero())
            {
                return q2.LengthSquared.IsZero() ? Identity : q2;
            }

            if (q2.LengthSquared.IsZero())
            {
                return q1;
            }

            float cosHalfAngle = (q1.W * q2.W) + Vector3.Dot(q1.Xyz, q2.Xyz);

            if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
            {
                return q1;
            }

            if (cosHalfAngle < 0.0f)
            {
                q2.Xyz = -q2.Xyz;
                q2.W = -q2.W;
                cosHalfAngle = -cosHalfAngle;
            }

            float blendA;
            float blendB;
            if (cosHalfAngle < 0.99f)
            {
                float halfAngle = (float)System.Math.Acos(cosHalfAngle);
                float sinHalfAngle = (float)System.Math.Sin(halfAngle);
                float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
                blendA = (float)System.Math.Sin(halfAngle * (1.0f - blend)) * oneOverSinHalfAngle;
                blendB = (float)System.Math.Sin(halfAngle * blend) * oneOverSinHalfAngle;
            }
            else
            {
                blendA = 1.0f - blend;
                blendB = blend;
            }

            Quaternion result = new Quaternion((blendA * q1.Xyz) + (blendB * q2.Xyz), (blendA * q1.W) + (blendB * q2.W));
            return result.LengthSquared > 0.0f ? result.Normalized : Identity;
        }

        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return Add(left, right);
        }

        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return Subtract(left, right);
        }

        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            return Multiply(left, right);
        }

        public static Quaternion operator *(Quaternion quaternion, float scale)
        {
            return Multiply(quaternion, scale);
        }

        public static Quaternion operator *(float scale, Quaternion quaternion)
        {
            return Multiply(quaternion, scale);
        }

        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"V: {Xyz}, W: {W}";
        }

        public override bool Equals(object other)
        {
            if (other is Quaternion == false)
            {
                return false;
            }

            return this == (Quaternion)other;
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

        public bool Equals(Quaternion other)
        {
            return
                _x.IsApproximately(other._x) &&
                _y.IsApproximately(other._y) &&
                _z.IsApproximately(other._z) &&
                _w.IsApproximately(other._w);
        }
    }
}