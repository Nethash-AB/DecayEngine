using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property.Reference;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.ShaderProgram;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.MayaPlugin.MayaInterop;
using DecayEngine.MayaPlugin.Scene.Node;

namespace DecayEngine.MayaPlugin.Scene
{
    public class MayaScene
    {
        public List<IMayaNode> Children { get; }
        public string MetaFileName { get; }

        public MayaScene(string metaFileName = "prefabs.meta")
        {
            Children = new List<IMayaNode>();
            MetaFileName = metaFileName;
        }

        public void AddDependencyNode(MObject mayaObject)
        {
            IMayaNode sceneNode = FindNode(mayaObject);
            if (sceneNode != null) return;

            MFnDependencyNode dependencyNode = new MFnDependencyNode(mayaObject);
            if (mayaObject.hasFn(MFn.Type.kWorld))
            {
                AddPrefab(mayaObject);
            }
//            else if (mayaObject.hasFn(MFn.Type.kBlinn))
//            {
//                AddBlinn(mayaObject);
//            }
            else if (mayaObject.hasFn(MFn.Type.kFileTexture))
            {
                AddTexture(mayaObject);
            }
            else if (dependencyNode.typeName == "StingrayPBS")
            {
                AddStingrayPbs(mayaObject);
            }
        }

        public ResourceBundle Build(Uri outputDirUri)
        {
            foreach (IMayaNode node in Children.ToList())
            {
                foreach (IMayaNode child in node.Children.ToList())
                {
                    SanitizeNodes(node, child);
                }

                if (node.IsValid || !Children.Contains(node)) continue;

                MGlobal.displayInfo($"Removing invalid node: {node.DisplayId}");
                Children.Remove(node);
            }

            List<IResource> resources = new List<IResource>();
            foreach (IMayaNode node in Children.ToList())
            {
                resources.AddRange(node.ToDecayResource(outputDirUri));
            }

            List<PrefabResource> prefabs = resources.OfType<PrefabResource>().ToList();
            foreach (IStatementExpression statementExpression in resources.OfType<IStatementExpression>())
            {
                foreach (PrefabResource prefabResource in prefabs)
                {
                    if (statementExpression is CreateComponentExpression createComponentExpression)
                    {
                        if (prefabResource.Children.All(c =>
                            !(c is CreateComponentExpression prefabComponentExpression) || prefabComponentExpression.Name != createComponentExpression.Name))
                        {
                            prefabResource.Children.Add(statementExpression);
                            continue;
                        }
                    }
                    prefabResource.Children.Add(statementExpression);
                }
            }

            foreach (PrefabResource prefabResource in prefabs)
            {
                List<CreateComponentExpression> componentExpressions = prefabResource.Children.OfType<CreateComponentExpression>().ToList();
                if (componentExpressions.Count < 1 || componentExpressions.All(c => c.ComponentType != ComponentType.ShaderProgram))
                {
                    prefabResource.Children.Add(new CreateShaderProgramComponentExpression
                    {
                        Active = true,
                        Name = "default_pbr_shader_program",
                        Template = new ResourceReferenceExpression
                        {
                            ReferenceId = "default_pbr_shader_program"
                        }
                    });
                }
            }

            return new ResourceBundle
            {
                Resources = resources.Where(res => res is IRootResource).ToList()
            };
        }

        public string ToSceneString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SceneRoot");
            sb.AppendLine("{");
            foreach (IMayaNode childNode in Children)
            {
                childNode.ToHierarchyString(sb, 1);
            }
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void AddPrefab(MObject mayaObject)
        {
            MayaPrefabNode node = new MayaPrefabNode(mayaObject)
            {
                Scene = this
            };
            Children.Add(node);
            AddChildren(node);
        }

//        private void AddBlinn(MObject mayaObject)
//        {
//            MayaBlinnNode node = new MayaBlinnNode(mayaObject)
//            {
//                Scene = this
//            };
//            Children.Add(node);
//        }

        private void AddStingrayPbs(MObject mayaObject)
        {
            MayaStingrayPbsNode node = new MayaStingrayPbsNode(mayaObject)
            {
                Scene = this
            };
            Children.Add(node);
        }

        private void AddTexture(MObject mayaObject)
        {
            MayaTextureNode node = new MayaTextureNode(mayaObject)
            {
                Scene = this
            };
            Children.Add(node);
        }

        private void AddTransform(IMayaNode parent, MObject mayaObject)
        {
            MayaTransformNode node = new MayaTransformNode(mayaObject)
            {
                Scene = this
            };
            parent.Children.Add(node);
            AddChildren(node);
        }

        private void AddMesh(IMayaNode parent, MObject mayaObject)
        {
            MayaMeshNode node = new MayaMeshNode(mayaObject)
            {
                Scene = this
            };
            parent.Children.Add(node);
        }

        private void AddUnknown(IMayaNode parent, MObject mayaObject)
        {
            MayaUnknownNode node = new MayaUnknownNode(mayaObject)
            {
                Scene = this
            };
            parent.Children.Add(node);
        }

        private void AddChildren(IMayaNode parent)
        {
            MFnDagNode dagNodeFn = new MFnDagNode(parent.MayaObject);
            for (uint i = 0; i < dagNodeFn.childCount; i++)
            {
                MObject childObject = dagNodeFn.child(i);

                if (!childObject.IsVisible()) continue;

                IMayaNode sceneNode = FindNode(childObject);
                if (sceneNode != null)
                {
                    parent.Children.Add(sceneNode);
                    AddChildren(sceneNode);
                }
                else
                {
                    switch (childObject.apiType)
                    {
                        case MFn.Type.kTransform:
                            AddTransform(parent, childObject);
                            break;
                        case MFn.Type.kMesh:
                            AddMesh(parent, childObject);
                            break;
                        default:
                            AddUnknown(parent, childObject);
                            break;
                    }
                }
            }
        }

        private IMayaNode FindNode(MObject mayaObject)
        {
            Stack<IMayaNode> stack = new Stack<IMayaNode>();

            foreach (IMayaNode mayaSceneNode in Children)
            {
                stack.Push(mayaSceneNode);
            }

            while (stack.Any())
            {
                IMayaNode next = stack.Pop();
                if (next.MayaObject.equalEqual(mayaObject))
                {
                    return next;
                }

                foreach (IMayaNode mayaSceneNode in next.Children)
                {
                    stack.Push(mayaSceneNode);
                }
            }

            return null;
        }

        private static void SanitizeNodes(IMayaNode parent, IMayaNode node)
        {
            foreach (IMayaNode child in node.Children.ToList())
            {
                SanitizeNodes(node, child);
            }

            if (node.IsValid || !parent.Children.Contains(node)) return;

            MGlobal.displayInfo($"Removing invalid node: {node.DisplayId}");
            parent.Children.Remove(node);
        }
    }
}