using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Exports.BaseExports.ShaderProgram;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.OpenGL.Component.Shader;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.ShaderProgram
{
    public class ShaderProgramComponent : IShaderProgram
    {
        private IGameObject _parentGameObject;
        private IScene _parentScene;

        private uint _programHandle;

        private readonly ConcurrentDictionary<string, int> _uniforms;
        private readonly ConcurrentDictionary<string, UniformBlock> _uniformBlocks;

        private bool IsBound
        {
            get
            {
                if (!Active) return false;

                return GameEngine.RenderEngine.ActiveShaderProgram == this;
            }
        }

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parentGameObject;
        public ByReference<IGameObject> ParentByRef => () => ref _parentGameObject;

        IScene IParentable<IScene>.Parent => _parentScene;
        ByReference<IScene> IParentable<IScene>.ParentByRef => () => ref _parentScene;

        public Type ExportType => typeof(ShaderProgramExport);

        public ShaderProgramResource Resource { get; set; }

        public bool Active
        {
            get
            {
                if (_parentScene == null && _parentGameObject == null)
                {
                    return false;
                }

                return _programHandle > 0;
            }
            set
            {
                if (!Active && value)
                {
                    Load();
                }
                else if (Active && !value)
                {
                    Unload();
                }
            }
        }

        public VertexShaderComponent VertexShader { get; set; }
        public GeometryShaderComponent GeometryShader { get; set; }
        public FragmentShaderComponent FragmentShader { get; set; }

        public bool Persistent { get; set; }

        public ShaderProgramComponent()
        {
            _uniforms = new ConcurrentDictionary<string, int>();
            _uniformBlocks = new ConcurrentDictionary<string, UniformBlock>();
        }

        ~ShaderProgramComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parentGameObject?.RemoveComponent(this);

            _parentScene?.RemoveComponent(this);
            _parentScene = null;

            if (VertexShader?.Parent == null)
            {
                VertexShader?.SetParent(parent);
            }
            if (GeometryShader?.Parent == null)
            {
                GeometryShader?.SetParent(parent);
            }
            if (FragmentShader?.Parent == null)
            {
                FragmentShader?.SetParent(parent);
            }

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

            if (VertexShader != null && VertexShader?.AsParentable<IGameObject>().Parent == null && ((IParentable<IScene>) VertexShader).Parent == null)
            {
                parent.AttachComponent(VertexShader);
            }

            if (GeometryShader != null && GeometryShader?.AsParentable<IGameObject>().Parent == null && ((IParentable<IScene>) GeometryShader).Parent == null)
            {
                parent.AttachComponent(GeometryShader);
            }

            if (FragmentShader != null && FragmentShader?.AsParentable<IGameObject>().Parent == null && ((IParentable<IScene>) FragmentShader).Parent == null)
            {
                parent.AttachComponent(FragmentShader);
            }

            parent?.AttachComponent(this);
            _parentScene = parent;
        }

        IParentable<IScene> IParentable<IScene>.AsParentable<T>()
        {
            return this;
        }

        public void Destroy()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(Unload);

            SetParent(null);

            Destroyed = true;
        }

        public void SetVariable(string name, ValueType value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            switch (value)
            {
                case float floatValue:
                    GL.Uniform1f(_uniforms[name], floatValue);
                    break;
                case double doubleValue:
                    GL.Uniform1f(_uniforms[name], (float) doubleValue);
                    break;
                case bool boolValue:
                    GL.Uniform1i(_uniforms[name], boolValue ? 1 : 0);
                    break;
                case int intValue:
                    GL.Uniform1i(_uniforms[name], intValue);
                    break;
                case uint uintValue:
                    GL.Uniform1ui(_uniforms[name], uintValue);
                    break;
            }
        }

        public void SetVariable(string name, float[] value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            GL.Uniform1fv(_uniforms[name], value.Length, value);
        }

        public void SetVariable(string name, Vector2 value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            (float x, float y) = value;
            GL.Uniform2f(_uniforms[name], x, y);
        }

        public void SetVariable(string name, Vector3 value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            (float x, float y, float z) = value;
            GL.Uniform3f(_uniforms[name], x, y, z);
        }

        public void SetVariable(string name, Vector4 value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            (float x, float y, float z, float w) = value;
            GL.Uniform4f(_uniforms[name], x, y, z, w);
        }

        public void SetVariable(string name, Matrix2 value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            GL.UniformMatrix2fv(_uniforms[name], 1, false, value.ToFloat());
        }

        public void SetVariable(string name, Matrix3 value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            GL.UniformMatrix3fv(_uniforms[name], 1, false, value.ToFloat());
        }

        public void SetVariable(string name, Matrix4 value)
        {
            if (!_uniforms.ContainsKey(name)) return;

            GL.UniformMatrix4fv(_uniforms[name], 1, false, value.ToFloat());
        }

        public void SetBlockVariable<T>(string name, T structure)
            where T : struct
        {
            if (!_uniformBlocks.ContainsKey(name)) return;

            UniformBlock uniformBlock = _uniformBlocks[name];

            int size = Marshal.SizeOf(structure);
            if (size > uniformBlock.MaximumSize)
            {
                throw new Exception($"Struct {typeof(T).Name}({size} bytes) is too big for uniform block {name}({uniformBlock.MaximumSize} bytes)");
            }

            byte[] data = new byte[size];
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);

            GL.UniformBlockBinding(_programHandle, (uint) uniformBlock.UniformIndex, (uint) uniformBlock.BindingPoint);
            GL.BindBufferBase(BufferTarget.UniformBuffer, (uint) uniformBlock.BindingPoint, uniformBlock.UniformBufferHandle);

            GL.BindBuffer(BufferTarget.UniformBuffer, uniformBlock.UniformBufferHandle);
            GL.BufferData(BufferTarget.UniformBuffer, new IntPtr(size), handle.AddrOfPinnedObject(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            handle.Free();
        }

        public void SetBlockVariable<T>(string name, T[] structureArray)
            where T : struct
        {
            if (!_uniformBlocks.ContainsKey(name)) return;

            UniformBlock uniformBlock = _uniformBlocks[name];

            int structureSize = Marshal.SizeOf<T>();
            int size = structureSize * structureArray.Length;
            if (size > uniformBlock.MaximumSize)
            {
                throw new Exception($"Struct {typeof(T).Name}({size} bytes) is too big for uniform block {name}({uniformBlock.MaximumSize} bytes)");
            }

            byte[] data = new byte[size];

            for (int i = 0; i < structureArray.Length; i++)
            {
                IntPtr buffer = Marshal.AllocHGlobal(structureSize);
                Marshal.StructureToPtr(structureArray[i], buffer, false);
                Marshal.Copy(buffer, data, structureSize * i, structureSize);
                Marshal.FreeHGlobal(buffer);
            }

            GL.UniformBlockBinding(_programHandle, (uint) uniformBlock.UniformIndex, (uint) uniformBlock.BindingPoint);
            GL.BindBufferBase(BufferTarget.UniformBuffer, (uint) uniformBlock.BindingPoint, uniformBlock.UniformBufferHandle);

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            GL.BindBuffer(BufferTarget.UniformBuffer, uniformBlock.UniformBufferHandle);
            GL.BufferData(BufferTarget.UniformBuffer, new IntPtr(size), handle.AddrOfPinnedObject(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            handle.Free();
        }

        public void Bind()
        {
            if (!Active || IsBound) return;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.UseProgram(_programHandle);
                GameEngine.RenderEngine.ActiveShaderProgram = this;

                SetVariable("time", GameEngine.EngineTime.TotalMilliseconds / 1000f);
            });
        }

        public void Unbind()
        {
            if (!Active || !IsBound) return;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.UseProgram(0);
                GameEngine.RenderEngine.ActiveShaderProgram = null;
            });
        }

        private int GetAttributeLocation(string name)
        {
            return GL.GetAttribLocation(_programHandle, name);
        }

        private int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(_programHandle, name);
        }

        private void Load()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                uint vertexShaderHandle = 0;
                if (VertexShader != null)
                {
                    vertexShaderHandle = VertexShader.Compile();
                }

                uint geometryShaderHandle = 0;
                if (GeometryShader != null)
                {
                    geometryShaderHandle = GeometryShader.Compile();
                }

                uint fragmentShaderHandle = 0;
                if (FragmentShader != null)
                {
                    fragmentShaderHandle = FragmentShader.Compile();
                }

                if (vertexShaderHandle < 1 && geometryShaderHandle < 1 && fragmentShaderHandle < 1)
                {
                    GameEngine.LogAppendLine(LogSeverity.Error, "OpenGL",
                        "Failed to create shader program, a minimum of one shader is required.");
                }

                _programHandle = GL.CreateProgram();
                if (vertexShaderHandle > 0)
                {
                    GL.AttachShader(_programHandle, vertexShaderHandle);
                }

                if (geometryShaderHandle > 0)
                {
                    GL.AttachShader(_programHandle, geometryShaderHandle);
                }

                if (fragmentShaderHandle > 0)
                {
                    GL.AttachShader(_programHandle, fragmentShaderHandle);
                }

                GL.LinkProgram(_programHandle);

                if (!GL.GetProgramLinkStatus(_programHandle))
                {
                    if (vertexShaderHandle > 0)
                    {
                        GL.DetachShader(_programHandle, vertexShaderHandle);
                        VertexShader?.Free();
                    }

                    if (geometryShaderHandle > 0)
                    {
                        GL.DetachShader(_programHandle, geometryShaderHandle);
                        GeometryShader?.Free();
                    }

                    if (fragmentShaderHandle > 0)
                    {
                        GL.DetachShader(_programHandle, fragmentShaderHandle);
                        FragmentShader?.Free();
                    }

                    GameEngine.LogAppendLine(LogSeverity.CriticalError, "OpenGL",
                        $"Failed to link shader program : {GL.GetProgramInfoLog(_programHandle)}");
                    return;
                }

                if (vertexShaderHandle > 0)
                {
                    GL.DetachShader(_programHandle, vertexShaderHandle);
                    VertexShader?.Free();
                }

                if (geometryShaderHandle > 0)
                {
                    GL.DetachShader(_programHandle, geometryShaderHandle);
                    GeometryShader?.Free();
                }

                if (fragmentShaderHandle > 0)
                {
                    GL.DetachShader(_programHandle, fragmentShaderHandle);
                    FragmentShader?.Free();
                }

                int[] count = new int[1];
                GL.GetProgramiv(_programHandle, ProgramParameter.ActiveUniforms, count);
                for (int i = 0; i < count[0]; i++)
                {
                    int[] length = new int[1];
                    int[] size = new int[1];
                    ActiveUniformType[] type = new ActiveUniformType[1];
                    StringBuilder nameBuilder = new StringBuilder();

                    GL.GetActiveUniform(_programHandle, i, 16, length, size, type, nameBuilder);

                    string name = nameBuilder.ToString();
                    Match match = Regex.Match(name, @"(.*)(\[[0-9]*\])");
                    if (match.Success)
                    {
                        name = match.Groups[1].Value;
                    }

                    int location = GetUniformLocation(name);
                    if (location < 0) continue;

                    _uniforms[name] = location;
                }

                GL.GetProgramiv(_programHandle, ProgramParameter.ActiveUniformBlocks, count);
                for (int i = 0; i < count[0]; i++)
                {
                    int[] length = new int[1];
                    StringBuilder nameBuilder = new StringBuilder();

                    GL.GetActiveUniformBlockName(_programHandle, (uint) i, 16, length, nameBuilder);

                    string name = nameBuilder.ToString();
//                    uint location = GL.GetUniformBlockIndex(_programHandle, name);
//                    if (location < 1) continue;
                    uint location = (uint) i;

                    int[] size = new int[1];
                    GL.GetActiveUniformBlockiv(_programHandle, location, ActiveUniformBlockParameter.UniformBlockDataSize, size);

                    uint blockBufferHandle = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.UniformBuffer, blockBufferHandle);
                    GL.BufferData(BufferTarget.UniformBuffer, new IntPtr(size[0]), IntPtr.Zero, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.UniformBuffer, 0);

                    int[] bindingPoint = new int[1];
                    GL.GetActiveUniformBlockiv(_programHandle, location, ActiveUniformBlockParameter.UniformBlockBinding, bindingPoint);
                    if (bindingPoint[0] < 0)
                    {
                        int highestBindingPoint = _uniformBlocks.Count > 0 ? _uniformBlocks.Max(ub => ub.Value.BindingPoint) : 0;
                        int nextBindingPoint = highestBindingPoint + 1;
                        GL.UniformBlockBinding(_programHandle, location, (uint) nextBindingPoint);
                        bindingPoint[0] = nextBindingPoint;
                    }

                    _uniformBlocks[name] = new UniformBlock(blockBufferHandle, bindingPoint[0], i, size[0]);
                }

                Unbind();
            });
        }

        private void Unload()
        {
            if (_programHandle > 0)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    GL.DeleteProgram(_programHandle);
                    _uniforms.Clear();
                });
            }
        }
    }
}