using System.Runtime.InteropServices;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.BulletInterop.Enums.Flags;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.Collision.World;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace DecayEngine.Bullet.Managed.Debug
{
    internal class BulletDebugDrawer : NativeObject
    {
        private readonly DrawAabbUnmanagedDelegate _drawAabb;
        private readonly DrawArcUnmanagedDelegate _drawArc;
        private readonly DrawBoxUnmanagedDelegate _drawBox;
        private readonly DrawCapsuleUnmanagedDelegate _drawCapsule;
        private readonly DrawConeUnmanagedDelegate _drawCone;
        private readonly DrawContactPointUnmanagedDelegate _drawContactPoint;
        private readonly DrawCylinderUnmanagedDelegate _drawCylinder;
        private readonly DrawLineUnmanagedDelegate _drawLine;
        private readonly DrawPlaneUnmanagedDelegate _drawPlane;
        private readonly DrawSphereUnmanagedDelegate _drawSphere;
        private readonly DrawSpherePatchUnmanagedDelegate _drawSpherePatch;
        private readonly DrawTransformUnmanagedDelegate _drawTransform;
        private readonly DrawTriangleUnmanagedDelegate _drawTriangle;
        private readonly GetDebugModeUnmanagedDelegate _getDebugMode;
        private readonly SimpleCallback _cb;

        public IPhysicsWorld PhysicsWorld { get; }
        public DebugDrawFlags DebugDrawFlags { get; set; }

        public BulletDebugDrawer(IPhysicsWorld physicsWorld, DebugDrawFlags debugDrawFlags)
        {
	        _drawAabb = DrawAabb;
	        _drawArc = DrawArc;
	        _drawBox = DrawBox;
	        _drawCapsule = DrawCapsule;
	        _drawCone = DrawCone;
	        _drawContactPoint = DrawContactPoint;
	        _drawCylinder = DrawCylinder;
	        _drawLine = DrawLine;
	        _drawPlane = DrawPlane;
	        _drawSphere = DrawSphere;
	        _drawSpherePatch = DrawSpherePatch;
	        _drawTransform = DrawTransform;
	        _drawTriangle = DrawTriangle;
	        _getDebugMode = GetDebugModeUnmanaged;
	        _cb = SimpleCallbackUnmanaged;

	        PhysicsWorld = physicsWorld;
	        DebugDrawFlags = debugDrawFlags;

	        GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
	        {
		        NativeHandle = btIDebugDrawWrapper_new(
			        GCHandle.ToIntPtr(GCHandle.Alloc(this)),
			        Marshal.GetFunctionPointerForDelegate(_drawAabb),
			        Marshal.GetFunctionPointerForDelegate(_drawArc),
			        Marshal.GetFunctionPointerForDelegate(_drawBox),
			        Marshal.GetFunctionPointerForDelegate(_drawCapsule),
			        Marshal.GetFunctionPointerForDelegate(_drawCone),
			        Marshal.GetFunctionPointerForDelegate(_drawContactPoint),
			        Marshal.GetFunctionPointerForDelegate(_drawCylinder),
			        Marshal.GetFunctionPointerForDelegate(_drawLine),
			        Marshal.GetFunctionPointerForDelegate(_drawPlane),
			        Marshal.GetFunctionPointerForDelegate(_drawSphere),
			        Marshal.GetFunctionPointerForDelegate(_drawSpherePatch),
			        Marshal.GetFunctionPointerForDelegate(_drawTransform),
			        Marshal.GetFunctionPointerForDelegate(_drawTriangle),
			        Marshal.GetFunctionPointerForDelegate(_getDebugMode),
			        Marshal.GetFunctionPointerForDelegate(_cb)
			    );
	        });
        }

        private static void SimpleCallbackUnmanaged(int x)
        {
        }

        private DebugDrawFlags GetDebugModeUnmanaged()
        {
	        return DebugDrawFlags;
        }

        public void DrawLine(Vector3 @from, Vector3 to, Vector3 color)
        {
            DrawLine(ref @from, ref to, ref color);
        }

        private void DrawLine(ref Vector3 @from, ref Vector3 to, ref Vector3 color)
        {
            if (!PhysicsWorld.DrawDebug) return;

            PhysicsWorld.DebugCamera?.DebugDrawer.AddLine(
                new Vector3(@from.X, @from.Y, @from.Z),
                new Vector3(to.X, to.Y, to.Z),
                new Vector4(color.X, color.Y, color.Z, 1f)
            );
        }

        private void DrawAabb(ref Vector3 from, ref Vector3 to, ref Vector3 color)
        {
            Vector3 a = from;
            a.X = to.X;
            DrawLine(ref from, ref a, ref color);

            Vector3 b = to;
            b.Y = from.Y;
            DrawLine(ref b, ref to, ref color);
            DrawLine(ref a, ref b, ref color);

            Vector3 c = from;
            c.Z = to.Z;
            DrawLine(ref from, ref c, ref color);
            DrawLine(ref b, ref c, ref color);

            b.Y = to.Y;
            b.Z = from.Z;
            DrawLine(ref b, ref to, ref color);
            DrawLine(ref a, ref b, ref color);

            a.Y = to.Y;
            a.X = from.X;
            DrawLine(ref from, ref a, ref color);
            DrawLine(ref a, ref b, ref color);

            b.X = from.X;
            b.Z = to.Z;
            DrawLine(ref c, ref b, ref color);
            DrawLine(ref a, ref b, ref color);
            DrawLine(ref b, ref to, ref color);
        }

        private void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB,
            float minAngle, float maxAngle, ref Vector3 color, bool drawSect, float stepDegrees = 10.0f)
        {
            Vector3 xAxis = radiusA * axis;
            Vector3 yAxis = radiusB * Vector3.Cross(normal, axis);
            float step = MathHelper.DegreesToRadians(stepDegrees);
            int nSteps = (int)((maxAngle - minAngle) / step);
            if (nSteps == 0)
            {
                nSteps = 1;
            }
            Vector3 prev = center + xAxis * (float)System.Math.Cos(minAngle) + yAxis * (float)System.Math.Sin(minAngle);
            if (drawSect)
            {
                DrawLine(ref center, ref prev, ref color);
            }
            for (int i = 1; i <= nSteps; i++)
            {
                float angle = minAngle + (maxAngle - minAngle) * i / nSteps;
                Vector3 next = center + xAxis * (float)System.Math.Cos(angle) + yAxis * (float)System.Math.Sin(angle);
                DrawLine(ref prev, ref next, ref color);
                prev = next;
            }
            if (drawSect)
            {
                DrawLine(ref center, ref prev, ref color);
            }
        }

        private void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, ref Matrix4 trans, ref Vector3 color)
        {
            Vector3 point = bbMin;
            Vector3 p1 = point * trans;
            point.X = bbMax.X;

            Vector3 p2 = point * trans;
            point.Y = bbMax.Y;

            Vector3 p3 = point * trans;
            point.X = bbMin.X;

            Vector3 p4 = point * trans;
            point.Z = bbMax.Z;

            Vector3 p8 = point * trans;
            point.X = bbMax.X;

            Vector3 p7 = point * trans;
            point.Y = bbMin.Y;

            Vector3 p6 = point * trans;
            point.X = bbMin.X;

            Vector3 p5 = point * trans;

            DrawLine(ref p1, ref p2, ref color);
            DrawLine(ref p2, ref p3, ref color);
            DrawLine(ref p3, ref p4, ref color);
            DrawLine(ref p4, ref p1, ref color);

            DrawLine(ref p1, ref p5, ref color);
            DrawLine(ref p2, ref p6, ref color);
            DrawLine(ref p3, ref p7, ref color);
            DrawLine(ref p4, ref p8, ref color);

            DrawLine(ref p5, ref p6, ref color);
            DrawLine(ref p6, ref p7, ref color);
            DrawLine(ref p7, ref p8, ref color);
            DrawLine(ref p8, ref p5, ref color);
        }

        private void DrawCapsule(float radius, float halfHeight, int upAxis, ref Matrix4 transform, ref Vector3 color)
        {
            Vector3 capStart = Vector3.Zero;
            capStart[upAxis] = -halfHeight;

            Vector3 capEnd = Vector3.Zero;
            capEnd[upAxis] = halfHeight;

            // Draw the ends
            Matrix4 childTransform = transform;
            childTransform.Origin = capStart * transform;
            DrawSphere(radius, ref childTransform, ref color);

            childTransform.Origin = capEnd * transform;
            DrawSphere(radius, ref childTransform, ref color);

            // Draw some additional lines
            Vector3 start = transform.Origin;
            Matrix4 basis = transform.Basis;

            capStart[(upAxis + 1) % 3] = radius;
            capEnd[(upAxis + 1) % 3] = radius;
            DrawLine(start + capStart * basis, start + capEnd * basis, color);

            capStart[(upAxis + 1) % 3] = -radius;
            capEnd[(upAxis + 1) % 3] = -radius;
            DrawLine(start + capStart * basis, start + capEnd * basis, color);

            capStart[(upAxis + 2) % 3] = radius;
            capEnd[(upAxis + 2) % 3] = radius;
            DrawLine(start + capStart * basis, start + capEnd * basis, color);

            capStart[(upAxis + 2) % 3] = -radius;
            capEnd[(upAxis + 2) % 3] = -radius;
            DrawLine(start + capStart * basis, start + capEnd * basis, color);
        }

        private void DrawCone(float radius, float height, int upAxis, ref Matrix4 transform, ref Vector3 color)
        {
            Vector3 start = transform.Origin;

            Vector3 offsetHeight = Vector3.Zero;
            offsetHeight[upAxis] = height * 0.5f;
            Vector3 offsetRadius = Vector3.Zero;
            offsetRadius[(upAxis + 1) % 3] = radius;

            Vector3 offset2Radius = Vector3.Zero;
            offsetRadius[(upAxis + 2) % 3] = radius;

            Matrix4 basis = transform.Basis;
            Vector3 from = start + offsetHeight * basis;
            Vector3 to = start + -offsetHeight * basis;
            DrawLine(from, to + offsetRadius, color);
            DrawLine(from, to - offsetRadius, color);
            DrawLine(from, to + offset2Radius, color);
            DrawLine(from, to - offset2Radius, color);
        }

        private void DrawContactPoint(ref Vector3 pointOnB, ref Vector3 normalOnB, float distance, int lifeTime, ref Vector3 color)
        {
            Vector3 to = pointOnB + normalOnB * 1f; // distance
            DrawLine(ref pointOnB, ref to, ref color);
        }

        private void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix4 transform, ref Vector3 color)
        {
            Vector3 start = transform.Origin;
            Matrix4 basis = transform.Basis;
            Vector3 offsetHeight = Vector3.Zero;
            offsetHeight[upAxis] = halfHeight;
            Vector3 offsetRadius = Vector3.Zero;
            offsetRadius[(upAxis + 1) % 3] = radius;
            DrawLine(start + (offsetHeight + offsetRadius) * basis, start + (-offsetHeight + offsetRadius) * basis, color);
            DrawLine(start + (offsetHeight - offsetRadius) * basis, start + (-offsetHeight - offsetRadius) * basis, color);
        }

        private void DrawPlane(ref Vector3 planeNormal, float planeConst, ref Matrix4 transform, ref Vector3 color)
        {
            Vector3 planeOrigin = planeNormal * planeConst;
            PlaneSpace1(ref planeNormal, out Vector3 vec0, out Vector3 vec1);

            const float vecLen = 100f;
            Vector3 pt0 = (planeOrigin + vec0 * vecLen) * transform;
            Vector3 pt1 = (planeOrigin - vec0 * vecLen) * transform;
            Vector3 pt2 = (planeOrigin + vec1 * vecLen) * transform;
            Vector3 pt3 = (planeOrigin - vec1 * vecLen) * transform;

            DrawLine(ref pt0, ref pt1, ref color);
            DrawLine(ref pt2, ref pt3, ref color);
        }

        private void DrawSphere(float radius, ref Matrix4 transform, ref Vector3 color)
        {
            Vector3 start = transform.Origin;
            Matrix4 basis = transform.Basis;

            Vector3 xoffs = new Vector3(radius, 0, 0) * basis;
            Vector3 yoffs = new Vector3(0, radius, 0) * basis;
            Vector3 zoffs = new Vector3(0, 0, radius) * basis;

            Vector3 xn = start - xoffs;
            Vector3 xp = start + xoffs;
            Vector3 yn = start - yoffs;
            Vector3 yp = start + yoffs;
            Vector3 zn = start - zoffs;
            Vector3 zp = start + zoffs;

            // XY
            DrawLine(ref xn, ref yp, ref color);
            DrawLine(ref yp, ref xp, ref color);
            DrawLine(ref xp, ref yn, ref color);
            DrawLine(ref yn, ref xn, ref color);

            // XZ
            DrawLine(ref xn, ref zp, ref color);
            DrawLine(ref zp, ref xp, ref color);
            DrawLine(ref xp, ref zn, ref color);
            DrawLine(ref zn, ref xn, ref color);

            // YZ
            DrawLine(ref yn, ref zp, ref color);
            DrawLine(ref zp, ref yp, ref color);
            DrawLine(ref yp, ref zn, ref color);
            DrawLine(ref zn, ref yn, ref color);
        }

        private void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius,
			float minTh, float maxTh, float minPs, float maxPs, ref Vector3 color, float stepDegrees)
		{
			Vector3 npole = center + up * radius;
			Vector3 spole = center - up * radius;
			Vector3 arcStart = Vector3.Zero;
			float step = MathHelper.DegreesToRadians(stepDegrees);
			Vector3 kv = up;
			Vector3 iv = axis;

			Vector3 jv = Vector3.Cross(kv, iv);
			bool drawN = false;
			bool drawS = false;
			if (minTh <= -MathHelper.PiOver2)
			{
				minTh = -MathHelper.PiOver2 + step;
				drawN = true;
			}
			if (maxTh >= MathHelper.PiOver2)
			{
				maxTh = MathHelper.PiOver2 - step;
				drawS = true;
			}
			if (minTh > maxTh)
			{
				minTh = -MathHelper.PiOver2 + step;
				maxTh = MathHelper.PiOver2 - step;
				drawN = drawS = true;
			}
			int nHor = (int)((maxTh - minTh) / step) + 1;
			if (nHor < 2) nHor = 2;
			float stepH = (maxTh - minTh) / (nHor - 1);
			bool isClosed;
			if (minPs > maxPs)
			{
				minPs = -MathHelper.Pi + step;
				maxPs = MathHelper.Pi;
				isClosed = true;
			}
			else if (maxPs - minPs >= MathHelper.Pi * 2f)
			{
				isClosed = true;
			}
			else
			{
				isClosed = false;
			}
			int nVert = (int)((maxPs - minPs) / step) + 1;
			if (nVert < 2) nVert = 2;

			Vector3[] vA = new Vector3[nVert];
			Vector3[] vB = new Vector3[nVert];
			Vector3[] pvA = vA; Vector3[] pvB = vB;

			float stepV = (maxPs - minPs) / (nVert - 1);
			for (int i = 0; i < nHor; i++)
			{
				float th = minTh + i * stepH;
				float sth = radius * (float)System.Math.Sin(th);
				float cth = radius * (float)System.Math.Cos(th);
				for (int j = 0; j < nVert; j++)
				{
					float psi = minPs + j * stepV;
					float sps = (float)System.Math.Sin(psi);
					float cps = (float)System.Math.Cos(psi);
					pvB[j] = center + cth * cps * iv + cth * sps * jv + sth * kv;
					if (i != 0)
					{
						DrawLine(ref pvA[j], ref pvB[j], ref color);
					}
					else if (drawS)
					{
						DrawLine(ref spole, ref pvB[j], ref color);
					}
					if (j != 0)
					{
						DrawLine(ref pvB[j - 1], ref pvB[j], ref color);
					}
					else
					{
						arcStart = pvB[j];
					}
					if (i == nHor - 1 && drawN)
					{
						DrawLine(ref npole, ref pvB[j], ref color);
					}
					if (isClosed)
					{
						if (j == nVert - 1)
						{
							DrawLine(ref arcStart, ref pvB[j], ref color);
						}
					}
					else
					{
						if ((i == 0 || i == nHor - 1) && (j == 0 || j == nVert - 1))
						{
							DrawLine(ref center, ref pvB[j], ref color);
						}
					}
				}
				Vector3[] pT = pvA; pvA = pvB; pvB = pT;
			}
		}

        private void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 color, float alpha)
        {
	        DrawLine(ref v0, ref v1, ref color);
	        DrawLine(ref v1, ref v2, ref color);
	        DrawLine(ref v2, ref v0, ref color);
        }

        private void DrawTransform(ref Matrix4 transform, float orthoLen)
        {
	        Vector3 start = transform.Origin;
	        Matrix4 basis = transform.Basis;

	        Vector3 ortho = new Vector3(orthoLen, 0, 0);
	        Vector3 colour = new Vector3(0.7f, 0, 0);
	        Vector3 temp = ortho * basis;
	        temp += start;
	        DrawLine(ref start, ref temp, ref colour);

	        ortho.X = 0;
	        ortho.Y = orthoLen;
	        colour.X = 0;
	        colour.Y = 0.7f;
	        temp = ortho * basis;
	        temp += start;
	        DrawLine(ref start, ref temp, ref colour);

	        ortho.Y = 0;
	        ortho.Z = orthoLen;
	        colour.Y = 0;
	        colour.Z = 0.7f;
	        temp = ortho * basis;
	        temp += start;
	        DrawLine(ref start, ref temp, ref colour);
        }

        private static void PlaneSpace1(ref Vector3 n, out Vector3 p, out Vector3 q)
        {
	        if (System.Math.Abs(n.Z) > MathHelper.Sqrt12)
	        {
		        float a = n.Y * n.Y + n.Z * n.Z;
		        float k = MathHelper.ReciprocalSqrt(a);
		        p = new Vector3(0, -n.Z * k, n.Y * k);
		        q = new Vector3(a * k, -n.X * p.Z, n.X * p.Y);
	        }
	        else
	        {
		        float a = n.X * n.X + n.Y * n.Y;
		        float k = MathHelper.ReciprocalSqrt(a);
		        p = new Vector3(-n.Y * k, n.X * k, 0);
		        q = new Vector3(-n.Z * p.Y, n.Z * p.X, a * k);
	        }
        }

        public void ReportErrorWarning(string warningString)
        {
            GameEngine.LogAppendLine(LogSeverity.Warning, "BulletDebugDrawer", warningString);
        }

        protected override void FreeUnmanagedHandles()
        {
	        if (!IsNativeHandleInitialized) return;

	        GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
	        {
		        btIDebugDraw_delete(NativeHandle);

		        // ToDo: Not sure if this is actually necessary, delete should invalidate the handle anyway.
//		        GCHandle handle = GCHandle.FromIntPtr(NativeHandle);
//		        if (handle.IsAllocated)
//		        {
//			        handle.Free();
//		        }
	        });
        }
    }
}