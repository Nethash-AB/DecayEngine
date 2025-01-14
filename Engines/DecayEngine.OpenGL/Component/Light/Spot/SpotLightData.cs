using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Light;

namespace DecayEngine.OpenGL.Component.Light.Spot
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpotLightData
    {
        public readonly Vector4 Position;
        public readonly Vector4 Direction;
        public readonly Vector4 Color;
        public readonly float Radius;
        public readonly float CutoffAngle;
        public readonly float Strength;

        public SpotLightData(Matrix4 viewMatrix, ISpotLight light)
        {
            Matrix4 worldTransformMatrix = light.Parent.WorldSpaceTransform.TransformMatrix;
            Position = worldTransformMatrix.Row4 * viewMatrix;
            Direction = worldTransformMatrix.Row3 * viewMatrix;

            Color = new Vector4(light.Color, 1f);
            if (Color.X > 1f)
            {
                Color.X = 1f;
            }
            else if (Color.X < 0f)
            {
                Color.X = 0f;
            }

            if (Color.Y > 1f)
            {
                Color.Y = 1f;
            }
            else if (Color.Y < 0f)
            {
                Color.Y = 0f;
            }

            if (Color.Z > 1f)
            {
                Color.Z = 1f;
            }
            else if (Color.Z < 0f)
            {
                Color.Z = 0f;
            }

            Radius = light.Radius;
            CutoffAngle = light.CutoffAngle;
            Strength = light.Strength;
        }
    }
}