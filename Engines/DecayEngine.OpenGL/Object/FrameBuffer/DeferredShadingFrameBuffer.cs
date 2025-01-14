using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.OpenGL.Component.Light.Directional;
using DecayEngine.OpenGL.Component.Light.Point;
using DecayEngine.OpenGL.Component.Light.Spot;
using DecayEngine.OpenGL.Object.Material;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.FrameBuffer
{
    public class DeferredShadingFrameBuffer : FrameBuffer, IDeferredShadingFrameBuffer
    {
        public DeferredShadingFrameBuffer()
        {
            FrameBufferMaterial = new DeferredShadingMaterial();
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

            PointLightData[] pointLightDataArray = null;
            DirectionalLightData[] directionalLightDataArray = null;
            SpotLightData[] spotLightDataArray = null;
            if (camera is ICameraPersp cameraPersp)
            {
                ((DeferredShadingMaterial) FrameBufferMaterial).EnvironmentTexture = cameraPersp.EnvironmentTexture;

                List<IPointLight> pointLights = camera.Lights.OfType<IPointLight>().Where(l => l.ActiveInGraph()).ToList();
                pointLightDataArray = new PointLightData[Math.Min(pointLights.Count, OpenGlConstants.MaximumLightAmount)];

                for (int i = 0; i < pointLightDataArray.Length; i++)
                {
                    pointLightDataArray[i] = new PointLightData(viewMatrix, pointLights[i]);
                }

                List<IDirectionalLight> directionalLights = camera.Lights.OfType<IDirectionalLight>().Where(l => l.ActiveInGraph()).ToList();
                directionalLightDataArray = new DirectionalLightData[Math.Min(directionalLights.Count, OpenGlConstants.MaximumLightAmount)];

                for (int i = 0; i < directionalLightDataArray.Length; i++)
                {
                    directionalLightDataArray[i] = new DirectionalLightData(viewMatrix, directionalLights[i]);
                }

                List<ISpotLight> spotLights = camera.Lights.OfType<ISpotLight>().Where(l => l.ActiveInGraph()).ToList();
                spotLightDataArray = new SpotLightData[Math.Min(spotLights.Count, OpenGlConstants.MaximumLightAmount)];

                for (int i = 0; i < spotLightDataArray.Length; i++)
                {
                    spotLightDataArray[i] = new SpotLightData(viewMatrix, spotLights[i]);
                }
            }

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                ShaderProgram.Bind();
                if (camera != null)
                {
                    ShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
                    ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);

                    if (pointLightDataArray != null)
                    {
                        ShaderProgram.SetVariable(OpenGlConstants.Uniforms.PointLightAmount, pointLightDataArray.Length);
                        if (pointLightDataArray.Length > 0)
                        {
                            ShaderProgram.SetBlockVariable(OpenGlConstants.UniformBlocks.PointLights, pointLightDataArray);
                        }
                    }

                    if (directionalLightDataArray != null)
                    {
                        ShaderProgram.SetVariable(OpenGlConstants.Uniforms.DirectionalLightAmount, directionalLightDataArray.Length);
                        if (directionalLightDataArray.Length > 0)
                        {
                            ShaderProgram.SetBlockVariable(OpenGlConstants.UniformBlocks.DirectionalLights, directionalLightDataArray);
                        }
                    }

                    if (spotLightDataArray != null)
                    {
                        ShaderProgram.SetVariable(OpenGlConstants.Uniforms.SpotLightAmount, spotLightDataArray.Length);
                        if (spotLightDataArray.Length > 0)
                        {
                            ShaderProgram.SetBlockVariable(OpenGlConstants.UniformBlocks.SpotLights, spotLightDataArray);
                        }
                    }

                    ShaderProgram.SetVariable(OpenGlConstants.Uniforms.AmbientColor, new Vector4(1f, 1f, 1f, 1f));
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