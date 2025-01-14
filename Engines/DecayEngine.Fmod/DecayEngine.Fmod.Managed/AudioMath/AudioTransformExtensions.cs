using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.Fmod.Managed.FmodInterop;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.Fmod.Managed.AudioMath
{
    public static class AudioTransformExtensions
    {
        public static ATTRIBUTES_3D Get3DAttributes(this Transform transform)
        {
            Vector3 position = transform.Position;
            Vector3 forward = -transform.Forward;
            Vector3 up = transform.Up;

            return new ATTRIBUTES_3D
            {
                position = new VECTOR
                {
                    x = position.X,
                    y = position.Y,
                    z = position.Z
                },
                forward = new VECTOR
                {
                    x = forward.X,
                    y = forward.Y,
                    z = forward.Z
                },
                up = new VECTOR
                {
                    x = up.X,
                    y = up.Y,
                    z = up.Z
                }
            };
        }
    }
}