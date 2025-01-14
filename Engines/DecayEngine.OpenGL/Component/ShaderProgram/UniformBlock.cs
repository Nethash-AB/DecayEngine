namespace DecayEngine.OpenGL.Component.ShaderProgram
{
    public struct UniformBlock
    {
        public readonly uint UniformBufferHandle;
        public readonly int BindingPoint;
        public readonly int UniformIndex;
        public readonly int MaximumSize;

        public UniformBlock(uint uniformBufferHandle, int bindingPoint, int uniformIndex, int maximumSize)
        {
            UniformBufferHandle = uniformBufferHandle;
            BindingPoint = bindingPoint;
            UniformIndex = uniformIndex;
            MaximumSize = maximumSize;
        }
    }
}