using System;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.DecPakLib.Math.Matrix
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3 : IEquatable<Matrix3>
    {
        public Vector3 Row0;
        public Vector3 Row1;
        public Vector3 Row2;

        public static readonly Matrix3 Identity = new Matrix3(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);
        public static readonly Matrix3 Zero = new Matrix3(Vector3.Zero, Vector3.Zero, Vector3.Zero);

        public Matrix3(Vector3 row0, Vector3 row1, Vector3 row2)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
        }

        public Matrix3
        (
            float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22
        )
        {
            Row0 = new Vector3(m00, m01, m02);
            Row1 = new Vector3(m10, m11, m12);
            Row2 = new Vector3(m20, m21, m22);
        }

        public Matrix3(Matrix4 matrix)
        {
            Row0 = matrix.Row1.Xyz;
            Row1 = matrix.Row2.Xyz;
            Row2 = matrix.Row3.Xyz;
        }

        public float Determinant
        {
            get
            {
                float m11 = Row0.X;
                float m12 = Row0.Y;
                float m13 = Row0.Z;
                float m21 = Row1.X;
                float m22 = Row1.Y;
                float m23 = Row1.Z;
                float m31 = Row2.X;
                float m32 = Row2.Y;
                float m33 = Row2.Z;

                return (m11 * m22 * m33) + (m12 * m23 * m31) + (m13 * m21 * m32)
                       - (m13 * m22 * m31) - (m11 * m23 * m32) - (m12 * m21 * m33);
            }
        }

        public Vector3 Column0 => new Vector3(Row0.X, Row1.X, Row2.X);
        public Vector3 Column1 => new Vector3(Row0.Y, Row1.Y, Row2.Y);
        public Vector3 Column2 => new Vector3(Row0.Z, Row1.Z, Row2.Z);

        public float M11
        {
            get => Row0.X;
            set => Row0.X = value;
        }

        public float M12
        {
            get => Row0.Y;
            set => Row0.Y = value;
        }

        public float M13
        {
            get => Row0.Z;
            set => Row0.Z = value;
        }

        public float M21
        {
            get => Row1.X;
            set => Row1.X = value;
        }

        public float M22
        {
            get => Row1.Y;
            set => Row1.Y = value;
        }

        public float M23
        {
            get => Row1.Z;
            set => Row1.Z = value;
        }

        public float M31
        {
            get => Row2.X;
            set => Row2.X = value;
        }

        public float M32
        {
            get => Row2.Y;
            set => Row2.Y = value;
        }

        public float M33
        {
            get => Row2.Z;
            set => Row2.Z = value;
        }

        public Vector3 Diagonal
        {
            get => new Vector3(Row0.X, Row1.Y, Row2.Z);
            set
            {
                Row0.X = value.X;
                Row1.Y = value.Y;
                Row2.Z = value.Z;
            }
        }

        public float Trace => Row0.X + Row1.Y + Row2.Z;

        public float this[int rowIndex, int columnIndex]
        {
            get =>
                rowIndex switch
                {
                    0 => Row0[columnIndex],
                    1 => Row1[columnIndex],
                    2 => Row2[columnIndex],
                    _ => throw new IndexOutOfRangeException("You tried to access this matrix at: (" + rowIndex + ", " + columnIndex + ")")
                };

            set
            {
                switch (rowIndex)
                {
                    case 0:
                        Row0[columnIndex] = value;
                        break;
                    case 1:
                        Row1[columnIndex] = value;
                        break;
                    case 2:
                        Row2[columnIndex] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("You tried to set this matrix at: (" + rowIndex + ", " +
                                                           columnIndex + ")");
                }
            }
        }

        public void Invert()
        {
            this = Invert(this);
        }

        public void Transpose()
        {
            this = Transpose(this);
        }

        public Matrix3 Normalized()
        {
            Matrix3 m = this;
            m.Normalize();
            return m;
        }

        public void Normalize()
        {
            float determinant = Determinant;
            Row0 /= determinant;
            Row1 /= determinant;
            Row2 /= determinant;
        }

        public Matrix3 Inverted()
        {
            Matrix3 m = this;
            if (!m.Determinant.IsZero())
            {
                m.Invert();
            }

            return m;
        }

        public Matrix3 ClearScale()
        {
            Matrix3 m = this;
            m.Row0 = m.Row0.Normalized;
            m.Row1 = m.Row1.Normalized;
            m.Row2 = m.Row2.Normalized;
            return m;
        }

        public Matrix3 ClearRotation()
        {
            Matrix3 m = this;
            m.Row0 = new Vector3(m.Row0.Length, 0, 0);
            m.Row1 = new Vector3(0, m.Row1.Length, 0);
            m.Row2 = new Vector3(0, 0, m.Row2.Length);
            return m;
        }

        public Vector3 ExtractScale()
        {
            return new Vector3(Row0.Length, Row1.Length, Row2.Length);
        }

        public Quaternion ExtractRotation(bool rowNormalize = true)
        {
            Vector3 row0 = Row0;
            Vector3 row1 = Row1;
            Vector3 row2 = Row2;

            if (rowNormalize)
            {
                row0 = row0.Normalized;
                row1 = row1.Normalized;
                row2 = row2.Normalized;
            }

            Quaternion q = default(Quaternion);
            double trace = 0.25 * (row0[0] + row1[1] + row2[2] + 1.0);

            if (trace > 0)
            {
                double sq = System.Math.Sqrt(trace);

                q.W = (float)sq;
                sq = 1.0 / (4.0 * sq);
                q.X = (float)((row1[2] - row2[1]) * sq);
                q.Y = (float)((row2[0] - row0[2]) * sq);
                q.Z = (float)((row0[1] - row1[0]) * sq);
            }
            else if (row0[0] > row1[1] && row0[0] > row2[2])
            {
                double sq = 2.0 * System.Math.Sqrt(1.0 + row0[0] - row1[1] - row2[2]);

                q.X = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[1] - row1[2]) * sq);
                q.Y = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[0] + row0[2]) * sq);
            }
            else if (row1[1] > row2[2])
            {
                double sq = 2.0 * System.Math.Sqrt(1.0 + row1[1] - row0[0] - row2[2]);

                q.Y = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row2[0] - row0[2]) * sq);
                q.X = (float)((row1[0] + row0[1]) * sq);
                q.Z = (float)((row2[1] + row1[2]) * sq);
            }
            else
            {
                double sq = 2.0 * System.Math.Sqrt(1.0 + row2[2] - row0[0] - row1[1]);

                q.Z = (float)(0.25 * sq);
                sq = 1.0 / sq;
                q.W = (float)((row1[0] - row0[1]) * sq);
                q.X = (float)((row2[0] + row0[2]) * sq);
                q.Y = (float)((row2[1] + row1[2]) * sq);
            }

            q = q.Normalized;
            return q;
        }

        public float[] ToFloat()
        {
            return new[]
            {
                M11, M12, M13,
                M21, M22, M23,
                M31, M32, M33
            };
        }

        public static void CreateFromAxisAngle(Vector3 axis, float angle, out Matrix3 result)
        {
            axis = axis.Normalized;
            float axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;

            float cos = (float)System.Math.Cos(-angle);
            float sin = (float)System.Math.Sin(-angle);
            float t = 1.0f - cos;

            float tXx = t * axisX * axisX;
            float tXy = t * axisX * axisY;
            float tXz = t * axisX * axisZ;
            float tYy = t * axisY * axisY;
            float tYz = t * axisY * axisZ;
            float tZz = t * axisZ * axisZ;

            float sinX = sin * axisX;
            float sinY = sin * axisY;
            float sinZ = sin * axisZ;

            result = Identity;
            result.Row0.X = tXx + cos;
            result.Row0.Y = tXy - sinZ;
            result.Row0.Z = tXz + sinY;
            result.Row1.X = tXy + sinZ;
            result.Row1.Y = tYy + cos;
            result.Row1.Z = tYz - sinX;
            result.Row2.X = tXz - sinY;
            result.Row2.Y = tYz + sinX;
            result.Row2.Z = tZz + cos;
        }

        public static Matrix3 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            CreateFromAxisAngle(axis, angle, out Matrix3 result);
            return result;
        }

        public static void CreateFromQuaternion(ref Quaternion q, out Matrix3 result)
        {
            Vector4 axisAngle = q.AxisAngle;
            float angle = axisAngle.W;
            Vector3 axis = axisAngle.Xyz;
            CreateFromAxisAngle(axis, angle, out result);
        }

        public static Matrix3 CreateFromQuaternion(Quaternion q)
        {
            CreateFromQuaternion(ref q, out Matrix3 result);
            return result;
        }

        public static void CreateRotationX(float angle, out Matrix3 result)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);

            result = Identity;
            result.Row1.Y = cos;
            result.Row1.Z = sin;
            result.Row2.Y = -sin;
            result.Row2.Z = cos;
        }

        public static Matrix3 CreateRotationX(float angle)
        {
            CreateRotationX(angle, out Matrix3 result);
            return result;
        }

        public static void CreateRotationY(float angle, out Matrix3 result)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);

            result = Identity;
            result.Row0.X = cos;
            result.Row0.Z = -sin;
            result.Row2.X = sin;
            result.Row2.Z = cos;
        }

        public static Matrix3 CreateRotationY(float angle)
        {
            CreateRotationY(angle, out Matrix3 result);
            return result;
        }

        public static void CreateRotationZ(float angle, out Matrix3 result)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);

            result = Identity;
            result.Row0.X = cos;
            result.Row0.Y = sin;
            result.Row1.X = -sin;
            result.Row1.Y = cos;
        }

        public static Matrix3 CreateRotationZ(float angle)
        {
            CreateRotationZ(angle, out Matrix3 result);
            return result;
        }

        public static Matrix3 CreateScale(float scale)
        {
            CreateScale(scale, out Matrix3 result);
            return result;
        }

        public static Matrix3 CreateScale(Vector3 scale)
        {
            CreateScale(ref scale, out Matrix3 result);
            return result;
        }

        public static Matrix3 CreateScale(float x, float y, float z)
        {
            CreateScale(x, y, z, out Matrix3 result);
            return result;
        }

        public static void CreateScale(float scale, out Matrix3 result)
        {
            result = Identity;
            result.Row0.X = scale;
            result.Row1.Y = scale;
            result.Row2.Z = scale;
        }

        public static void CreateScale(ref Vector3 scale, out Matrix3 result)
        {
            result = Identity;
            result.Row0.X = scale.X;
            result.Row1.Y = scale.Y;
            result.Row2.Z = scale.Z;
        }

        public static void CreateScale(float x, float y, float z, out Matrix3 result)
        {
            result = Identity;
            result.Row0.X = x;
            result.Row1.Y = y;
            result.Row2.Z = z;
        }

        public static Matrix3 Add(Matrix3 left, Matrix3 right)
        {
            Add(ref left, ref right, out Matrix3 result);
            return result;
        }

        public static void Add(ref Matrix3 left, ref Matrix3 right, out Matrix3 result)
        {
            result.Row0 = Vector3.Add(left.Row0, right.Row0);
            result.Row1 = Vector3.Add(left.Row1, right.Row1);
            result.Row2 = Vector3.Add(left.Row2, right.Row2);
        }

        public static Matrix3 Mult(Matrix3 left, Matrix3 right)
        {
            Mult(ref left, ref right, out Matrix3 result);
            return result;
        }

        public static void Mult(ref Matrix3 left, ref Matrix3 right, out Matrix3 result)
        {
            float leftM11 = left.Row0.X;
            float leftM12 = left.Row0.Y;
            float leftM13 = left.Row0.Z;
            float leftM21 = left.Row1.X;
            float leftM22 = left.Row1.Y;
            float leftM23 = left.Row1.Z;
            float leftM31 = left.Row2.X;
            float leftM32 = left.Row2.Y;
            float leftM33 = left.Row2.Z;
            float rightM11 = right.Row0.X;
            float rightM12 = right.Row0.Y;
            float rightM13 = right.Row0.Z;
            float rightM21 = right.Row1.X;
            float rightM22 = right.Row1.Y;
            float rightM23 = right.Row1.Z;
            float rightM31 = right.Row2.X;
            float rightM32 = right.Row2.Y;
            float rightM33 = right.Row2.Z;

            result = Identity;
            result.Row0.X = (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31);
            result.Row0.Y = (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32);
            result.Row0.Z = (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33);
            result.Row1.X = (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31);
            result.Row1.Y = (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32);
            result.Row1.Z = (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33);
            result.Row2.X = (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31);
            result.Row2.Y = (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32);
            result.Row2.Z = (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33);
        }

        public static void Invert(ref Matrix3 mat, out Matrix3 result)
        {
            int[] colIdx = { 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1 };

            float[,] inverse =
            {
                { mat.Row0.X, mat.Row0.Y, mat.Row0.Z },
                { mat.Row1.X, mat.Row1.Y, mat.Row1.Z },
                { mat.Row2.X, mat.Row2.Y, mat.Row2.Z }
            };

            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 3; i++)
            {
                float maxPivot = 0.0f;
                for (int j = 0; j < 3; j++)
                {
                    if (pivotIdx[j] == 0) continue;
                    for (int k = 0; k < 3; ++k)
                    {
                        if (pivotIdx[k] == -1)
                        {
                            float absVal = System.Math.Abs(inverse[j, k]);
                            if (!(absVal > maxPivot)) continue;
                            maxPivot = absVal;
                            irow = j;
                            icol = k;
                        }
                        else if (pivotIdx[k] > 0)
                        {
                            result = mat;
                            return;
                        }
                    }
                }

                ++pivotIdx[icol];

                if (irow != icol)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];

                if (pivot.IsZero())
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                }

                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 3; ++k)
                {
                    inverse[icol, k] *= oneOverPivot;
                }

                for (int j = 0; j < 3; ++j)
                {
                    if (icol == j) continue;
                    float f = inverse[j, icol];
                    inverse[j, icol] = 0.0f;
                    for (int k = 0; k < 3; ++k)
                    {
                        inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 2; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 3; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            result = Identity;
            result.Row0.X = inverse[0, 0];
            result.Row0.Y = inverse[0, 1];
            result.Row0.Z = inverse[0, 2];
            result.Row1.X = inverse[1, 0];
            result.Row1.Y = inverse[1, 1];
            result.Row1.Z = inverse[1, 2];
            result.Row2.X = inverse[2, 0];
            result.Row2.Y = inverse[2, 1];
            result.Row2.Z = inverse[2, 2];
        }

        public static Matrix3 Invert(Matrix3 mat)
        {
            Invert(ref mat, out Matrix3 result);
            return result;
        }

        public static Matrix3 Transpose(Matrix3 mat)
        {
            return new Matrix3(mat.Column0, mat.Column1, mat.Column2);
        }

        public static void Transpose(ref Matrix3 mat, out Matrix3 result)
        {
            result = Identity;
            result.Row0.X = mat.Row0.X;
            result.Row0.Y = mat.Row1.X;
            result.Row0.Z = mat.Row2.X;
            result.Row1.X = mat.Row0.Y;
            result.Row1.Y = mat.Row1.Y;
            result.Row1.Z = mat.Row2.Y;
            result.Row2.X = mat.Row0.Z;
            result.Row2.Y = mat.Row1.Z;
            result.Row2.Z = mat.Row2.Z;
        }

        public static Matrix3 operator *(Matrix3 left, Matrix3 right)
        {
            return Mult(left, right);
        }

        public static bool operator ==(Matrix3 left, Matrix3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix3 left, Matrix3 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{Row0}\n{Row1}\n{Row2}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Row0.GetHashCode();
                hashCode = (hashCode * 397) ^ Row1.GetHashCode();
                hashCode = (hashCode * 397) ^ Row2.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix3 matrix3 && Equals(matrix3);
        }

        public bool Equals(Matrix3 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2;
        }
    }
}