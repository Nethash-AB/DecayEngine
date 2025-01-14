using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Light;

namespace DecayEngine.OpenGL.Component.Light.Directional
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DirectionalLightData
    {
        public readonly Vector4 Direction;
        public readonly Vector4 Color;
        public readonly float Strength;

        public DirectionalLightData(Matrix4 viewMatrix, IDirectionalLight light)
        {
            Direction = light.Parent.WorldSpaceTransform.RotationMatrix.Row3 * viewMatrix;

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

            Strength = light.Strength;
        }
    }
}