using System;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.DecPakLib.Math.Matrix
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Matrix4 : IEquatable<Matrix4>
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public static readonly Matrix4 Identity =
            new Matrix4(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);
        public static readonly Matrix4 Zero = new Matrix4(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);

        public Vector4 Row1
        {
            get => new Vector4(M11, M12, M13, M14);
            set { M11 = value.X; M12 = value.Y; M13 = value.Z; M14 = value.W; }
        }

        public Vector4 Row2
        {
            get => new Vector4(M21, M22, M23, M24);
            set { M21 = value.X; M22 = value.Y; M23 = value.Z; M24 = value.W; }
        }

        public Vector4 Row3
        {
            get => new Vector4(M31, M32, M33, M34);
            set { M31 = value.X; M32 = value.Y; M33 = value.Z; M34 = value.W; }
        }

        public Vector4 Row4
        {
            get => new Vector4(M41, M42, M43, M44);
            set { M41 = value.X; M42 = value.Y; M43 = value.Z; M44 = value.W; }
        }

        public Matrix4 Basis
        {
            get =>
                new Matrix4(
                    M11, M12, M13, 0,
                    M21, M22, M23, 0,
                    M31, M32, M33, 0,
                    0, 0, 0, 1);
            set
            {
                M11 = value.M11;
                M12 = value.M12;
                M13 = value.M13;
                M21 = value.M21;
                M22 = value.M22;
                M23 = value.M23;
                M31 = value.M31;
                M32 = value.M32;
                M33 = value.M33;
            }
        }

        public Vector3 Origin
        {
            get => new Vector3(M41, M42, M43);
            set { M41 = value.X; M42 = value.Y; M43 = value.Z; }
        }

        public float Determinant =>
            (M11 * M22 * M33 * M44) - (M11 * M22 * M34 * M43) + (M11 * M23 * M34 * M42) - (M11 * M23 * M32 * M44)
            + (M11 * M24 * M32 * M43) - (M11 * M24 * M33 * M42) - (M12 * M23 * M34 * M41) + (M12 * M23 * M31 * M44)
            - (M12 * M24 * M31 * M43) + (M12 * M24 * M33 * M41) - (M12 * M21 * M33 * M44) + (M12 * M21 * M34 * M43)
                                                                                          + (M13 * M24 * M31 * M42) -
            (M13 * M24 * M32 * M41) + (M13 * M21 * M32 * M44) - (M13 * M21 * M34 * M42)
            + (M13 * M22 * M34 * M41) - (M13 * M22 * M31 * M44) - (M14 * M21 * M32 * M43) + (M14 * M21 * M33 * M42)
            - (M14 * M22 * M33 * M41) + (M14 * M22 * M31 * M43) - (M14 * M23 * M31 * M42) + (M14 * M23 * M32 * M41);

        public Vector4 Column1
        {
            get => new Vector4(M11, M21, M31, M41);
            set { M11 = value.X; M21 = value.Y; M31 = value.Z; M41 = value.W; }
        }

        public Vector4 Column2
        {
            get => new Vector4(M12, M22, M32, M42);
            set { M12 = value.X; M22 = value.Y; M32 = value.Z; M42 = value.W; }
        }

        public Vector4 Column3
        {
            get => new Vector4(M13, M23, M33, M43);
            set { M13 = value.X; M23 = value.Y; M33 = value.Z; M43 = value.W; }
        }

        public Vector4 Column4
        {
            get => new Vector4(M14, M24, M34, M44);
            set { M14 = value.X; M24 = value.Y; M34 = value.Z; M44 = value.W; }
        }

        public Vector4 Diagonal
        {
            get => new Vector4(Row1.X, Row2.Y, Row3.Z, Row4.W);
            set
            {
                M11 = value.X;
                M22 = value.Y;
                M33 = value.Z;
                M44 = value.W;
            }
        }

        public Vector3 Right
        {
            get => new Vector3(M11, M12, M13);
            set
            {
                M11 = value.X;
                M12 = value.Y;
                M13 = value.Z;
            }
        }

        public Vector3 Up
        {
            get => new Vector3(M21, M22, M23);
            set
            {
                M21 = value.X;
                M22 = value.Y;
                M23 = value.Z;
            }
        }

        public Vector3 Forward
        {
            get => new Vector3(-M31, -M32, -M33);
            set
            {
                M31 = -value.X;
                M32 = -value.Y;
                M33 = -value.Z;
            }
        }

        public float Trace => Row1.X + Row2.Y + Row3.Z + Row4.W;

        public float this[int rowIndex, int columnIndex]
        {
            get =>
                rowIndex switch
                {
                    0 => Row1[columnIndex],
                    1 => Row2[columnIndex],
                    2 => Row3[columnIndex],
                    3 => Row4[columnIndex],
                    _ => throw new IndexOutOfRangeException("You tried to access this matrix at: (" + rowIndex + ", " + columnIndex + ")")
                };
            set
            {
                switch (rowIndex)
                {
                    case 0:
                        switch (columnIndex)
                        {
                            case 0:
                                M11 = value;
                                break;
                            case 1:
                                M12 = value;
                                break;
                            case 2:
                                M13 = value;
                                break;
                            case 3:
                                M14 = value;
                                break;
                        }
                        break;
                    case 1:
                        switch (columnIndex)
                        {
                            case 0:
                                M21 = value;
                                break;
                            case 1:
                                M22 = value;
                                break;
                            case 2:
                                M23 = value;
                                break;
                            case 3:
                                M24 = value;
                                break;
                        }
                        break;
                    case 2:
                        switch (columnIndex)
                        {
                            case 0:
                                M31 = value;
                                break;
                            case 1:
                                M32 = value;
                                break;
                            case 2:
                                M33 = value;
                                break;
                            case 3:
                                M34 = value;
                                break;
                        }
                        break;
                    case 3:
                        switch (columnIndex)
                        {
                            case 0:
                                M41 = value;
                                break;
                            case 1:
                                M42 = value;
                                break;
                            case 2:
                                M43 = value;
                                break;
                            case 3:
                                M44 = value;
                                break;
                        }
                        break;
                    default:
                        throw new IndexOutOfRangeException("You tried to set this matrix at: (" + rowIndex + ", " +
                                                           columnIndex + ")");
                }
            }
        }

        public Matrix4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
        {
            (float m11, float m12, float m13, float m14) = row1;
            (float m21, float m22, float m23, float m24) = row2;
            (float m31, float m32, float m33, float m34) = row3;
            (float m41, float m42, float m43, float m44) = row4;

            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;

            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;

            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;

            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        public Matrix4
        (
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44
        )
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;

            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;

            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;

            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        public Matrix4(Matrix3 topLeft)
        {
            M11 = topLeft.Row0.X;
            M12 = topLeft.Row0.Y;
            M13 = topLeft.Row0.Z;
            M14 = 0;
            M21 = topLeft.Row1.X;
            M22 = topLeft.Row1.Y;
            M23 = topLeft.Row1.Z;
            M24 = 0;
            M31 = topLeft.Row2.X;
            M32 = topLeft.Row2.Y;
            M33 = topLeft.Row2.Z;
            M34 = 0;
            M41 = 0;
            M42 = 0;
            M43 = 0;
            M44 = 1;
        }

        public void Invert()
        {
            this = Invert(this);
        }

        public void Transpose()
        {
            this = Transpose(this);
        }

        public Matrix4 Normalized()
        {
            Matrix4 m = this;
            m.Normalize();
            return m;
        }

        public void Normalize()
        {
            float determinant = Determinant;
            Row1 /= determinant;
            Row2 /= determinant;
            Row3 /= determinant;
            Row4 /= determinant;
        }

        public Matrix4 Inverted()
        {
            Matrix4 m = this;
            if (!m.Determinant.IsZero())
            {
                m.Invert();
            }

            return m;
        }

        public Matrix4 ClearTranslation()
        {
            Matrix4 m = this;
            m.M41 = 0f;
            m.M42 = 0f;
            m.M43 = 0f;
            return m;
        }

        public Matrix4 ClearScale()
        {
            Matrix4 m = this;
            (float m11, float m12, float m13) = m.Row1.Xyz.Normalized;
            (float m21, float m22, float m23) = m.Row2.Xyz.Normalized;
            (float m31, float m32, float m33) = m.Row3.Xyz.Normalized;

            m.M11 = m11;
            m.M12 = m12;
            m.M13 = m13;

            m.M21 = m21;
            m.M22 = m22;
            m.M23 = m23;

            m.M31 = m31;
            m.M32 = m32;
            m.M33 = m33;

            return m;
        }

        public Matrix4 ClearRotation()
        {
            Matrix4 m = this;

            m.M11 = m.Row1.Xyz.Length;
            m.M12 = 0f;
            m.M13 = 0f;

            m.M21 = 0f;
            m.M22 = m.Row2.Xyz.Length;
            m.M23 = 0f;

            m.M31 = 0f;
            m.M32 = 0f;
            m.M33 = m.Row3.Xyz.Length;

            return m;
        }

        public Matrix4 ClearProjection()
        {
            Matrix4 m = this;
            m.Column4 = Vector4.Zero;
            return m;
        }

        public Vector3 ExtractTranslation()
        {
            return Row4.Xyz;
        }

        public Vector3 ExtractScale()
        {
            return new Vector3(Row1.Xyz.Length, Row2.Xyz.Length, Row3.Xyz.Length);
        }

        public Quaternion ExtractRotation(bool rowNormalize = true)
        {
            Vector3 row0 = Row1.Xyz;
            Vector3 row1 = Row2.Xyz;
            Vector3 row2 = Row3.Xyz;

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

            return q.Normalized;
        }

        public Vector4 ExtractProjection()
        {
            return Column4;
        }

        public float[] ToFloat()
        {
            return new[]
            {
                M11, M12, M13, M14,
                M21, M22, M23, M24,
                M31, M32, M33, M34,
                M41, M42, M43, M44
            };
        }

        public static Matrix4 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            Matrix4 result = Identity;
            axis = axis.Normalized;

            Vector3 sinAxis = (float) System.Math.Sin(angle) * axis;
            float c = (float) System.Math.Cos(angle);
            Vector3 cosAxis = (1 - c) * axis;

            float tmp = cosAxis.X * axis.Y;
//            result.Row0.Y = tmp - sinAxis.Z;
            result.M12 = tmp - sinAxis.Z;
//            result.Row1.X = tmp + sinAxis.Z;
            result.M21 = tmp + sinAxis.Z;

            tmp = cosAxis.X * axis.Z;
//            result.Row0.Z = tmp + sinAxis.Y;
            result.M13 = tmp + sinAxis.Y;
//            result.Row2.X = tmp - sinAxis.Y;
            result.M31 = tmp - sinAxis.Y;

            tmp = cosAxis.Y * axis.Z;
//            result.Row1.Z = tmp - sinAxis.X;
            result.M23 = tmp - sinAxis.X;
//            result.Row2.Y = tmp + sinAxis.X;
            result.M32 = tmp + sinAxis.X;

            result.Diagonal = new Vector4(cosAxis * axis + new Vector3(c), 1f);

            result.Transpose();
            return result;
        }

        public static Matrix4 CreateFromQuaternion(Quaternion q)
        {
            Vector4 axisAngle = q.AxisAngle;
            return CreateFromAxisAngle(axisAngle.Xyz, axisAngle.W);
        }

        public static Matrix4 CreateFromTranslationRotationScale(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            Matrix4 result = Identity;

            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            result.M11 = scale.X * (1f - (2f * (yy + zz)));
            result.M12 = scale.X * (2f * (xy + zw));
            result.M13 = scale.X * (2f * (zx - yw));
            result.M14 = 0f;
            result.M21 = scale.Y * (2f * (xy - zw));
            result.M22 = scale.Y * (1f - (2f * (zz + xx)));
            result.M23 = scale.Y * (2f * (yz + xw));
            result.M24 = 0f;
            result.M31 = scale.Z * (2f * (zx + yw));
            result.M32 = scale.Z * (2f * (yz - xw));
            result.M33 = scale.Z * (1f - (2f * (yy + xx)));
            result.M34 = 0f;
            result.M41 = translation.X;
            result.M42 = translation.Y;
            result.M43 = translation.Z;
            result.M44 = 1f;

            return result;
        }

        public static Matrix4 CreateRotationX(float angle)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);

            Matrix4 result = Identity;

            result.M22 = cos;
            result.M23 = sin;
            result.M32 = -sin;
            result.M33 = cos;

            return result;
        }

        public static Matrix4 CreateRotationY(float angle)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);

            Matrix4 result = Identity;

            result.M11 = cos;
            result.M13 = -sin;
            result.M31 = sin;
            result.M33 = cos;

            return result;
        }

        public static Matrix4 CreateRotationZ(float angle)
        {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);

            Matrix4 result = Identity;

            result.M11 = cos;
            result.M12 = sin;
            result.M21 = -sin;
            result.M22 = cos;

            return result;
        }

        public static Matrix4 CreateTranslation(float x, float y, float z)
        {
            Matrix4 result = Identity;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;

            return result;
        }

        public static Matrix4 CreateTranslation(Vector3 vector)
        {
            (float x, float y, float z) = vector;
            return CreateTranslation(x, y, z);
        }

        public static Matrix4 CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        public static Matrix4 CreateScale(Vector3 scale)
        {
            (float x, float y, float z) = scale;
            return CreateScale(x, y, z);
        }

        public static Matrix4 CreateScale(float x, float y, float z)
        {
            Matrix4 result = Identity;
            result.M11 = x;
            result.M22 = y;
            result.M33 = z;
            return result;
        }

        public static Matrix4 CreateOrthographic(float width, float height, float depthNear, float depthFar)
        {
            return CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, depthNear, depthFar);
        }

        public static Matrix4 CreateOrthographicOffCenter
        (
            float left,
            float right,
            float bottom,
            float top,
            float depthNear,
            float depthFar
        )
        {
            Matrix4 result = Identity;

            float invRl = 1.0f / (right - left);
            float invTb = 1.0f / (top - bottom);
            float invFn = 1.0f / (depthFar - depthNear);

            result.M11 = 2 * invRl;
            result.M22 = 2 * invTb;
            result.M33 = -2 * invFn;

            result.M41 = -(right + left) * invRl;
            result.M42 = -(top + bottom) * invTb;
            result.M43 = -(depthFar + depthNear) * invFn;

            return result;
        }

        public static Matrix4 CreatePerspectiveFieldOfView
        (
            float fovy,
            float aspect,
            float depthNear,
            float depthFar
        )
        {
            if (fovy <= 0 || fovy > System.Math.PI)
            {
                throw new ArgumentOutOfRangeException(nameof(fovy));
            }

            if (aspect <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(aspect));
            }

            if (depthNear <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depthNear));
            }

            if (depthFar <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depthFar));
            }

            float maxY = depthNear * (float)System.Math.Tan(0.5f * fovy);
            float minY = -maxY;
            float minX = minY * aspect;
            float maxX = maxY * aspect;

            return CreatePerspectiveOffCenter(minX, maxX, minY, maxY, depthNear, depthFar);
        }

        public static Matrix4 CreatePerspectiveOffCenter
        (
            float left,
            float right,
            float bottom,
            float top,
            float depthNear,
            float depthFar
        )
        {
            if (depthNear <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depthNear));
            }

            if (depthFar <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depthFar));
            }

            if (depthNear >= depthFar)
            {
                throw new ArgumentOutOfRangeException(nameof(depthNear));
            }

            float x = 2.0f * depthNear / (right - left);
            float y = 2.0f * depthNear / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(depthFar + depthNear) / (depthFar - depthNear);
            float d = -(2.0f * depthFar * depthNear) / (depthFar - depthNear);

            Matrix4 result = Identity;

            result.M11 = x;
            result.M12 = 0;
            result.M13 = 0;
            result.M14 = 0;

            result.M21 = 0;
            result.M22 = y;
            result.M23 = 0;
            result.M24 = 0;

            result.M31 = a;
            result.M32 = b;
            result.M33 = c;
            result.M34 = -1;

            result.M41 = 0;
            result.M42 = 0;
            result.M43 = d;
            result.M44 = 0;

            return result;
        }

        public static Matrix4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 z = (eye - target).Normalized;
            Vector3 x = Vector3.Cross(up, z).Normalized;
            (float f, float y, float f1) = Vector3.Cross(z, x).Normalized;

            Matrix4 result = Identity;

            result.M11 = x.X;
            result.M12 = f;
            result.M13 = z.X;
            result.M14 = 0;

            result.M21 = x.Y;
            result.M22 = y;
            result.M23 = z.Y;
            result.M24 = 0;

            result.M31 = x.Z;
            result.M32 = f1;
            result.M33 = z.Z;
            result.M34 = 0;

            result.M41 = -((x.X * eye.X) + (x.Y * eye.Y) + (x.Z * eye.Z));
            result.M42 = -((f * eye.X) + (y * eye.Y) + (f1 * eye.Z));
            result.M43 = -((z.X * eye.X) + (z.Y * eye.Y) + (z.Z * eye.Z));
            result.M44 = 1;

            return result;
        }

        public static Matrix4 LookAt
        (
            float eyeX,
            float eyeY,
            float eyeZ,
            float targetX,
            float targetY,
            float targetZ,
            float upX,
            float upY,
            float upZ
        )
        {
            return LookAt
            (
                new Vector3(eyeX, eyeY, eyeZ),
                new Vector3(targetX, targetY, targetZ),
                new Vector3(upX, upY, upZ)
            );
        }

        public static Matrix4 Add(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = Identity;

            result.Row1 = left.Row1 + right.Row1;
            result.Row2 = left.Row2 + right.Row2;
            result.Row3 = left.Row3 + right.Row3;
            result.Row4 = left.Row4 + right.Row4;

            return result;
        }

        public static Matrix4 Subtract(Matrix4 left, Matrix4 right)
        {
            Matrix4 result = Identity;

            result.Row1 = left.Row1 - right.Row1;
            result.Row2 = left.Row2 - right.Row2;
            result.Row3 = left.Row3 - right.Row3;
            result.Row4 = left.Row4 - right.Row4;

            return result;
        }

        public static Matrix4 Mult(Matrix4 left, Matrix4 right)
        {
            float leftM11 = left.Row1.X;
            float leftM12 = left.Row1.Y;
            float leftM13 = left.Row1.Z;
            float leftM14 = left.Row1.W;
            float leftM21 = left.Row2.X;
            float leftM22 = left.Row2.Y;
            float leftM23 = left.Row2.Z;
            float leftM24 = left.Row2.W;
            float leftM31 = left.Row3.X;
            float leftM32 = left.Row3.Y;
            float leftM33 = left.Row3.Z;
            float leftM34 = left.Row3.W;
            float leftM41 = left.Row4.X;
            float leftM42 = left.Row4.Y;
            float leftM43 = left.Row4.Z;
            float leftM44 = left.Row4.W;
            float rightM11 = right.Row1.X;
            float rightM12 = right.Row1.Y;
            float rightM13 = right.Row1.Z;
            float rightM14 = right.Row1.W;
            float rightM21 = right.Row2.X;
            float rightM22 = right.Row2.Y;
            float rightM23 = right.Row2.Z;
            float rightM24 = right.Row2.W;
            float rightM31 = right.Row3.X;
            float rightM32 = right.Row3.Y;
            float rightM33 = right.Row3.Z;
            float rightM34 = right.Row3.W;
            float rightM41 = right.Row4.X;
            float rightM42 = right.Row4.Y;
            float rightM43 = right.Row4.Z;
            float rightM44 = right.Row4.W;

            Matrix4 result = Identity;

            result.M11 = (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31) + (leftM14 * rightM41);
            result.M12 = (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32) + (leftM14 * rightM42);
            result.M13 = (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33) + (leftM14 * rightM43);
            result.M14 = (leftM11 * rightM14) + (leftM12 * rightM24) + (leftM13 * rightM34) + (leftM14 * rightM44);

            result.M21 = (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31) + (leftM24 * rightM41);
            result.M22 = (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32) + (leftM24 * rightM42);
            result.M23 = (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33) + (leftM24 * rightM43);
            result.M24 = (leftM21 * rightM14) + (leftM22 * rightM24) + (leftM23 * rightM34) + (leftM24 * rightM44);

            result.M31 = (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31) + (leftM34 * rightM41);
            result.M32 = (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32) + (leftM34 * rightM42);
            result.M33 = (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33) + (leftM34 * rightM43);
            result.M34 = (leftM31 * rightM14) + (leftM32 * rightM24) + (leftM33 * rightM34) + (leftM34 * rightM44);

            result.M41 = (leftM41 * rightM11) + (leftM42 * rightM21) + (leftM43 * rightM31) + (leftM44 * rightM41);
            result.M42 = (leftM41 * rightM12) + (leftM42 * rightM22) + (leftM43 * rightM32) + (leftM44 * rightM42);
            result.M43 = (leftM41 * rightM13) + (leftM42 * rightM23) + (leftM43 * rightM33) + (leftM44 * rightM43);
            result.M44 = (leftM41 * rightM14) + (leftM42 * rightM24) + (leftM43 * rightM34) + (leftM44 * rightM44);

            return result;
        }

        public static Matrix4 Mult(Matrix4 left, float right)
        {
            Matrix4 result = Identity;

            result.Row1 = left.Row1 * right;
            result.Row2 = left.Row2 * right;
            result.Row3 = left.Row3 * right;
            result.Row4 = left.Row4 * right;

            return result;
        }

        public static Matrix4 Invert(Matrix4 mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            float[,] inverse =
            {
                { mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W },
                { mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W },
                { mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W },
                { mat.Row4.X, mat.Row4.Y, mat.Row4.Z, mat.Row4.W }
            };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                float maxPivot = 0.0f;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] == 0) continue;
                    for (int k = 0; k < 4; ++k)
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
                            return mat;
                        }
                    }
                }

                ++pivotIdx[icol];

                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
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
                for (int k = 0; k < 4; ++k)
                {
                    inverse[icol, k] *= oneOverPivot;
                }

                for (int j = 0; j < 4; ++j)
                {
                    if (icol == j) continue;
                    float f = inverse[j, icol];
                    inverse[j, icol] = 0.0f;
                    for (int k = 0; k < 4; ++k)
                    {
                        inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            Matrix4 result = Identity;

            result.M11 = inverse[0, 0];
            result.M12 = inverse[0, 1];
            result.M13 = inverse[0, 2];
            result.M14 = inverse[0, 3];

            result.M21 = inverse[1, 0];
            result.M22 = inverse[1, 1];
            result.M23 = inverse[1, 2];
            result.M24 = inverse[1, 3];

            result.M31 = inverse[2, 0];
            result.M32 = inverse[2, 1];
            result.M33 = inverse[2, 2];
            result.M34 = inverse[2, 3];

            result.M41 = inverse[3, 0];
            result.M42 = inverse[3, 1];
            result.M43 = inverse[3, 2];
            result.M44 = inverse[3, 3];

            return result;
        }

        public static Matrix4 Transpose(Matrix4 mat)
        {
            return new Matrix4(mat.Column1, mat.Column2, mat.Column3, mat.Column4);
        }

        public static Matrix4 operator *(Matrix4 left, Matrix4 right)
        {
            return Mult(left, right);
        }

        public static Matrix4 operator *(Matrix4 left, float right)
        {
            return Mult(left, right);
        }

        public static Matrix4 operator +(Matrix4 left, Matrix4 right)
        {
            return Add(left, right);
        }

        public static Matrix4 operator -(Matrix4 left, Matrix4 right)
        {
            return Subtract(left, right);
        }

        public static bool operator ==(Matrix4 left, Matrix4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Matrix4 left, Matrix4 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"{Row1}\n{Row2}\n{Row3}\n{Row4}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Row1.GetHashCode();
                hashCode = (hashCode * 397) ^ Row2.GetHashCode();
                hashCode = (hashCode * 397) ^ Row3.GetHashCode();
                hashCode = (hashCode * 397) ^ Row4.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix4 matrix4 && Equals(matrix4);
        }

        public bool Equals(Matrix4 other)
        {
            return
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3 &&
                Row4 == other.Row4;
        }
    }
}