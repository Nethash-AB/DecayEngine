namespace DecayEngine.OpenGL
{
    public static class OpenGlConstants
    {
        // Lighting
        public const int MaximumLightAmount = 99;

        // Uniforms
        public static class Uniforms
        {
            // Matrices
            public const string Model = "model";
            public const string View = "view";
            public const string InverseView = "inverseView";
            public const string Projection = "projection";
            public const string InverseProjection = "inverseProjection";

            // Solid Color
            public const string Color = "fragColor";

            // Pbr
            public const string AlbedoColor = "albedoColor";
            public const string UseAlbedoMap = "useAlbedoMap";
            public const string MetallicityFactor = "metalFactor";
            public const string UseMetallicityMap = "useMetalMap";
            public const string RoughnessFactor = "roughFactor";
            public const string UseRoughnessMap = "useRoughMap";
            public const string EmissionColor = "emissionColor";
            public const string UseEmissionMap = "useEmissionMap";

            // Lighting
            public const string PointLightAmount = "pntLightAmount";
            public const string DirectionalLightAmount = "dirLightAmount";
            public const string SpotLightAmount = "sptLightAmount";
            public const string AmbientColor = "ambientColor";
        }

        // Uniform Blocks
        public static class UniformBlocks
        {
            // Pbr
            public const string PointLights = "pointLights";
            public const string DirectionalLights = "dirLights";
            public const string SpotLights = "sptLights";
        }
    }
}