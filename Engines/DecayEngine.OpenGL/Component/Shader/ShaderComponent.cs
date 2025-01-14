using System;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Shader;
using DecayEngine.ModuleSDK.Exports.BaseExports.Shader;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.Shader
{
    public abstract class ShaderComponent : IShader
    {
        private IGameObject _parentGameObject;
        private IScene _parentScene;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parentGameObject;
        public ByReference<IGameObject> ParentByRef => () => ref _parentGameObject;

        IScene IParentable<IScene>.Parent => _parentScene;
        ByReference<IScene> IParentable<IScene>.ParentByRef => () => ref _parentScene;

        public Type ExportType => typeof(ShaderExport);

        public ShaderResource Resource { get; set; }
        public bool Active
        {
            get => ShaderHandle > 0;
            set { }
        }

        public bool Persistent { get; set; }

        protected uint ShaderHandle;
        private readonly byte[] _binaryShaderData;
        private readonly string _rawShaderData;

        protected ShaderComponent(byte[] binaryShaderData)
        {
            _binaryShaderData = binaryShaderData;
        }

        protected ShaderComponent(string rawShaderData)
        {
            _rawShaderData = rawShaderData;
        }

        ~ShaderComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parentGameObject?.RemoveComponent(this);

            _parentScene?.RemoveComponent(this);
            _parentScene = null;

            parent?.AttachComponent(this);
            _parentGameObject = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        void IParentable<IScene>.SetParent(IScene parent)
        {
            _parentScene?.RemoveComponent(this);

            _parentGameObject?.RemoveComponent(this);
            _parentGameObject = null;

            parent?.AttachComponent(this);
            _parentScene = parent;
        }

        IParentable<IScene> IParentable<IScene>.AsParentable<T>()
        {
            return this;
        }

        public void Destroy()
        {
            Free();

            SetParent(null);

            Destroyed = true;
        }

        public virtual uint Compile()
        {
            if (ShaderHandle < 1) return 0;

            return GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (_binaryShaderData != null)
                {
                    GCHandle gcHandle = GCHandle.Alloc(_binaryShaderData, GCHandleType.Pinned);

                    GL.ShaderBinary(1, new []{ShaderHandle},
                        GlConsts.GL_SHADER_BINARY_FORMAT_SPIR_V,
                        gcHandle.AddrOfPinnedObject(), _binaryShaderData.Length
                    );
                    GL.SpecializeShader(ShaderHandle, "main", 0, IntPtr.Zero, IntPtr.Zero);

                    gcHandle.Free();
                }
                else
                {
                    GL.ShaderSource(ShaderHandle, _rawShaderData);
                    GL.CompileShader(ShaderHandle);
                }

                if (!GL.GetShaderCompileStatus(ShaderHandle))
                {
                    string error = GL.GetShaderInfoLog(ShaderHandle);
                    GameEngine.LogAppendLine(LogSeverity.CriticalError, "OpenGL", $"Failed to compile shader: \n{error}");
                    return 0u;
                }

                return ShaderHandle;
            });
        }

        public void Free()
        {
            if (ShaderHandle > 0)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    GL.DeleteShader(ShaderHandle);
                });
            }
        }
    }
}