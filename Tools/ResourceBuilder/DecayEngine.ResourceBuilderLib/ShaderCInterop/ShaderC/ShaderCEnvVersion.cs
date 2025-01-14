namespace DecayEngine.ResourceBuilderLib.ShaderCInterop.ShaderC
{
    public enum ShaderCEnvVersion : uint
    {
        Vulkan10 = (uint) 1 << 22,
        Vulkan11 = ((uint)1 << 22) | (1 << 12),
        Opengl45 = 450,
        Webgpu,
    }
}