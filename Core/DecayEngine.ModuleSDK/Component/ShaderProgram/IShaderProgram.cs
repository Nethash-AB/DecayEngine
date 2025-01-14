using System;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;

namespace DecayEngine.ModuleSDK.Component.ShaderProgram
{
    public interface IShaderProgram : IComponent<ShaderProgramResource>, ISceneAttachableComponent
    {
        void SetVariable(string name, ValueType value);
        void SetVariable(string name, float[] value);
        void SetVariable(string name, Vector2 value);
        void SetVariable(string name, Vector3 value);
        void SetVariable(string name, Vector4 value);
        void SetVariable(string name, Matrix2 value);
        void SetVariable(string name, Matrix3 value);
        void SetVariable(string name, Matrix4 value);
        void SetBlockVariable<T>(string name, T structure) where T : struct;
        void SetBlockVariable<T>(string name, T[] structureArray) where T : struct;

        void Bind();
        void Unbind();
    }
}