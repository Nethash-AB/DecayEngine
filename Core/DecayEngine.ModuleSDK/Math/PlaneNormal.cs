using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Math
{
    public class PlaneNormal
    {
        public Vector3 Normal { get; set; }
        public Vector3 Support { get; set; }

        public PlaneNormal(Vector3 normal, Vector3 support)
        {
            Normal = normal;
            Support = support;
        }
    }
}