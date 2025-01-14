using System;
using System.Collections.Generic;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Resource;

namespace DecayEngine.MayaPlugin.Scene.Node
{
    public class MayaUnknownNode : IMayaNode
    {
        public MayaScene Scene { get; set; }
        public MObject MayaObject { get; }
        public List<IMayaNode> Children { get; }

        public string DisplayId
        {
            get
            {
                MFnDagNode dagNodeFn = new MFnDagNode(MayaObject);
                return $"{dagNodeFn.name} [{GetType().Name}({dagNodeFn.typeName})]";
            }
        }

        public bool IsValid => false;

        public MayaUnknownNode(MObject mayaObject)
        {
            MayaObject = mayaObject;
            Children = new List<IMayaNode>();
        }

        public IEnumerable<IResource> ToDecayResource(Uri outputDirUri)
        {
            return new List<IResource>();
        }
    }
}