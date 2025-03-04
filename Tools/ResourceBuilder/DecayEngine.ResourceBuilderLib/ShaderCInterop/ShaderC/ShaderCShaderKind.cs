namespace DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderC
{
    public enum ShaderCShaderKind
    {
        VertexShader,
        FragmentShader,
        ComputeShader,
        GeometryShader,
        TessControlShader,
        TessEvaluationShader,
        GlslVertexShader = VertexShader,
        GlslFragmentShader = FragmentShader,
        GlslComputeShader = ComputeShader,
        GlslGeometryShader = GeometryShader,
        GlslTessControlShader = TessControlShader,
        GlslTessEvaluationShader = TessEvaluationShader,
        GlslInferFromSource,
        GlslDefaultVertexShader,
        GlslDefaultFragmentShader,
        GlslDefaultComputeShader,
        GlslDefaultGeometryShader,
        GlslDefaultTessControlShader,
        GlslDefaultTessEvaluationShader,
        SpirvAssembly,
        RaygenShader,
        AnyhitShader,
        ClosesthitShader,
        MissShader,
        IntersectionShader,
        CallableShader,
        GlslRaygenShader = RaygenShader,
        GlslAnyhitShader = AnyhitShader,
        GlslClosesthitShader = ClosesthitShader,
        GlslMissShader = MissShader,
        GlslIntersectionShader = IntersectionShader,
        GlslCallableShader = CallableShader,
        GlslDefaultRaygenShader,
        GlslDefaultAnyhitShader,
        GlslDefaultClosesthitShader,
        GlslDefaultMissShader,
        GlslDefaultIntersectionShader,
        GlslDefaultCallableShader,
        TaskShader,
        MeshShader,
        GlslTaskShader = TaskShader,
        GlslMeshShader = MeshShader,
        GlslDefaultTaskShader,
        GlslDefaultMeshShader,
    }
}