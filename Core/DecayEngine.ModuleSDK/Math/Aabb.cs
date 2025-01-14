using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Math
{
    public class Aabb
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public Aabb(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public Aabb() : this(Vector3.Zero, Vector3.Zero)
        {
        }
    }
}