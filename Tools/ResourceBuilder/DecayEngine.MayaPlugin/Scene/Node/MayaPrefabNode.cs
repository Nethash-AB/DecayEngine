using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;

namespace DecayEngine.MayaPlugin.Scene.Node
{
    public class MayaPrefabNode : IMayaNode
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

        public bool IsValid => Children.Count > 0;

        public MayaPrefabNode(MObject mayaObject)
        {
            MayaObject = mayaObject;
            Children = new List<IMayaNode>();
        }

        public IEnumerable<IResource> ToDecayResource(Uri outputDirUri)
        {
            List<IRootResource> rootResources = new List<IRootResource>();

            PrefabResource prefab = new PrefabResource
            {
                Id = Path.GetFileNameWithoutExtension(Scene.MetaFileName),
                MetaFilePath = Scene.MetaFileName,
                Children = new List<IStatementExpression>()
            };
            rootResources.Add(prefab);

            foreach (IMayaNode mayaSceneNode in Children)
            {
                foreach (IResource resource in mayaSceneNode.ToDecayResource(outputDirUri))
                {
                    switch (resource)
                    {
                        case IStatementExpression statement:
                            prefab.Children.Add(statement);
                            break;
                        case IRootResource rootResource:
                            rootResources.Add(rootResource);
                            break;
                    }
                }
            }

            return rootResources;
        }
    }
}