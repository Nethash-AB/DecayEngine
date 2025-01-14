using DecayEngine.DecPakLib.Math.Vector;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Math
{
    [ProtoContract]
    public class ColorStructure : IResource
    {
        [ProtoMember(2000)]
        public float R { get; set; }
        [ProtoMember(2001)]
        public float G { get; set; }
        [ProtoMember(2002)]
        public float B { get; set; }
        [ProtoMember(2003)]
        public float A { get; set; }

        [ProtoMember(2004)]
        public string ColorName { get; set; }

        [ProtoMember(2005)]
        public bool WasStatic { get; set; }

        public ColorStructure()
        {
        }

        public ColorStructure(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static ColorStructure FromColorName(string color = null)
        {
            string normalizedColorName = color?.ToLowerInvariant().TrimStart().TrimEnd();
            ColorStructure colorStructure;

            switch (normalizedColorName)
            {
                case "red":
                    colorStructure = Red;
                    colorStructure.ColorName = normalizedColorName;
                    break;
                case "green":
                    colorStructure = Green;
                    colorStructure.ColorName = normalizedColorName;
                    break;
                case "blue":
                    colorStructure = Blue;
                    colorStructure.ColorName = normalizedColorName;
                    break;
                case "black":
                    colorStructure = Black;
                    colorStructure.ColorName = normalizedColorName;
                    break;
                case "white":
                    colorStructure = White;
                    colorStructure.ColorName = normalizedColorName;
                    break;
                default:
                    colorStructure = Transparency;
                    colorStructure.ColorName = "transparency";
                    break;
            }

            colorStructure.WasStatic = true;
            return colorStructure;
        }

        public static implicit operator Vector4(ColorStructure s)
        {
            return new Vector4(s.R, s.G, s.B, s.A);
        }

        [ProtoIgnore]
        private static ColorStructure Red => new ColorStructure(1f, 0f, 0f, 1f);
        [ProtoIgnore]
        private static ColorStructure Green => new ColorStructure(0f, 1f, 0f, 1f);
        [ProtoIgnore]
        private static ColorStructure Blue => new ColorStructure(0f, 0f, 1f, 1f);
        [ProtoIgnore]
        private static ColorStructure Black => new ColorStructure(0f, 0f, 0f, 1f);
        [ProtoIgnore]
        private static ColorStructure White => new ColorStructure(1f, 1f, 1f, 1f);
        [ProtoIgnore]
        private static ColorStructure Transparency => new ColorStructure(0f, 0f, 0f, 0f);
    }
}