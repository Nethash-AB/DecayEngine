using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet.Values;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.OpenGL.Object.Material;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.FrameBuffer
{
    public class RenderFrameBuffer : FrameBuffer, IRenderFrameBuffer
    {
        public PostProcessingStage PostProcessingStage { get; set; }

        public RenderFrameBuffer()
        {
            FrameBufferMaterial = new RenderTargetMaterial();
        }

        public override void Draw(ICamera camera, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (FrameBufferMaterial == null)
            {
                return;
            }

            if (ShaderProgram == null)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    "Tried to draw framebuffer without a shader program."
                );
                return;
            }

            if (!ShaderProgram.Active)
            {
                ShaderProgram.Active = true;
            }

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                ShaderProgram.Bind();

                if (PostProcessingStage != null)
                {
                    if (PostProcessingStage.Kernel != null && PostProcessingStage.Kernel.Length == 9)
                    {
                        ShaderProgram.SetVariable("kernel", PostProcessingStage.Kernel);
                    }

                    if (PostProcessingStage.Properties?.PropertySheetValues != null)
                    {
                        foreach (IPropertySheetValue propertySheetValue in PostProcessingStage.Properties.PropertySheetValues)
                        {
                            switch (propertySheetValue)
                            {
                                case BoolPropertySheetValue value:
                                    ShaderProgram.SetVariable(value.Name, value.Value);
                                    break;
                                case NumericPropertySheetValue value:
                                    ShaderProgram.SetVariable(value.Name, value.Value);
                                    break;
                                case Vector2PropertySheetValue value:
                                    ShaderProgram.SetVariable(value.Name, value.Value);
                                    break;
                                case Vector3PropertySheetValue value:
                                    ShaderProgram.SetVariable(value.Name, value.Value);
                                    break;
                                case Vector4PropertySheetValue value:
                                    ShaderProgram.SetVariable(value.Name, value.Value);
                                    break;
                            }
                        }
                    }
                }

                BindVao();

                FrameBufferMaterial.BindAsRenderTarget();

                GL.DrawArrays(BeginMode.Triangles, 0, 6);

                FrameBufferMaterial.Unbind();

                UnbindVao();

                ShaderProgram.Unbind();
            });
        }
    }
}