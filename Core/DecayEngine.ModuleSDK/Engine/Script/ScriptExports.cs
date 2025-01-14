using System;
using System.Collections.Generic;
using System.Reflection;

namespace DecayEngine.ModuleSDK.Engine.Script
{
    public class ScriptExports
    {
        public List<MethodInfo> Functions { get; }
        public Dictionary<string, object> GlobalVariables { get; }
        public List<Type> Types { get; }

        public ScriptExports()
        {
            Functions = new List<MethodInfo>();
            GlobalVariables = new Dictionary<string, object>();
            Types = new List<Type>();
        }
    }
}