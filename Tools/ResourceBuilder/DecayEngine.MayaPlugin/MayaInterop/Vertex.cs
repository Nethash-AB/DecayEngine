using System;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Math;

#pragma warning disable 659

namespace DecayEngine.MayaPlugin.MayaInterop
{
    public class Vertex : IEquatable<Vertex>
    {
        public int Id;
        public MFloatPoint Position;
        public MFloatVector Normal;
        public MFloatVector Tangent;
        public MFloatVector Bitangent;
        public float[] UvCoordinates;

        public bool Equals(Vertex other)
        {
            if (other == null) return false;

            if (!Position.x.IsApproximately(other.Position.x) ||
                !Position.y.IsApproximately(other.Position.y) ||
                !Position.z.IsApproximately(other.Position.z)
            )
            {
                return false;
            }

            if (!Normal.x.IsApproximately(other.Normal.x) ||
                !Normal.y.IsApproximately(other.Normal.y) ||
                !Normal.z.IsApproximately(other.Normal.z)
            )
            {
                return false;
            }

            if (!UvCoordinates[0].IsApproximately(other.UvCoordinates[0]) ||
                !UvCoordinates[1].IsApproximately(other.UvCoordinates[1])
            )
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals((Vertex) obj);
        }
    }
}