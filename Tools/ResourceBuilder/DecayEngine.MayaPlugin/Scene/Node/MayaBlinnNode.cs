using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Property.Reference;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.PbrMaterial;
using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.MayaPlugin.Scene.Node
{
    public class MayaBlinnNode : IMayaNode
    {
        public MayaScene Scene { get; set; }
        public MObject MayaObject { get; }
        public List<IMayaNode> Children { get; }

        public MayaBlinnNode(MObject mayaObject)
        {
            MayaObject = mayaObject;
            Children = new List<IMayaNode>();
        }

        public bool IsValid => Children.Count < 1;

        public string DisplayId
        {
            get
            {
                MFnDependencyNode dependencyNodeFn = new MFnDependencyNode(MayaObject);
                return $"{dependencyNodeFn.name} [{GetType().Name}({dependencyNodeFn.typeName})]";
            }
        }

        public IEnumerable<IResource> ToDecayResource(Uri outputDirUri)
        {
            List<IResource> resources = new List<IResource>();

//            MFnBlinnShader blinnNodeFn = new MFnBlinnShader(MayaObject);
//            MFnDependencyNode dependencyNodeFn = new MFnDependencyNode(MayaObject);
//
//            IPropertyExpression albedoTexture = null;
//            MFnDependencyNode albedoTextureNodeFn = GetTexture(dependencyNodeFn, "color");
//            if (albedoTextureNodeFn != null)
//            {
//                albedoTexture = new ResourceReferenceExpression
//                {
//                    ReferenceId = albedoTextureNodeFn.name
//                };
//            }
//
//            Vector4Structure albedoColor = new Vector4Structure
//            {
//                X = blinnNodeFn.ambientColor.r,
//                Y = blinnNodeFn.ambientColor.g,
//                Z = blinnNodeFn.ambientColor.b,
//                W = blinnNodeFn.ambientColor.a,
//            };
//
//            IPropertyExpression normalTexture = null;
//            MFnDependencyNode normalTextureNodeFn = GetNormalTexture(dependencyNodeFn, out float normalFactor);
//            if (normalTextureNodeFn != null)
//            {
//                normalTexture = new ResourceReferenceExpression
//                {
//                    ReferenceId = normalTextureNodeFn.name
//                };
//            }
//
//            IPropertyExpression specularTexture = null;
//            MFnDependencyNode specularTextureNodeFn = GetTexture(dependencyNodeFn, "specularColor");
//            if (specularTextureNodeFn != null)
//            {
//                specularTexture = new ResourceReferenceExpression
//                {
//                    ReferenceId = specularTextureNodeFn.name
//                };
//            }
//
//            IPropertyExpression emissionTexture = null;
//            MFnDependencyNode emissionTextureNodeFn = GetTexture(dependencyNodeFn, "incandescence");
//            if (emissionTextureNodeFn != null)
//            {
//                emissionTexture = new ResourceReferenceExpression
//                {
//                    ReferenceId = emissionTextureNodeFn.name
//                };
//            }
//
//            Vector4Structure emissionColor = new Vector4Structure
//            {
//                X = blinnNodeFn.incandescence.r,
//                Y = blinnNodeFn.incandescence.g,
//                Z = blinnNodeFn.incandescence.b,
//                W = blinnNodeFn.incandescence.a,
//            };
//
//            PbrMaterialResource materialResource = new PbrMaterialResource
//            {
//                Id = blinnNodeFn.name,
//                MetaFilePath = $"{Path.GetFileNameWithoutExtension(Scene.MetaFileName)}_materials.meta",
//                AlbedoTexture = albedoTexture,
//                AlbedoColor = albedoColor,
//                SpecularTexture = specularTexture,
//                Smoothness = blinnNodeFn.specularRollOff,
//                NormalTexture = normalTexture,
//                NormalFactor = normalFactor,
//                EmissionTexture = emissionTexture,
//                EmissionColor = emissionColor
//            };
//
//            resources.Add(materialResource);
//
//            CreatePbrMaterialComponentExpression pbrMaterialExpression = new CreatePbrMaterialComponentExpression
//            {
//                Name = $"material_{materialResource.Id}",
//                Active = true,
//                Template = new ResourceReferenceExpression
//                {
//                    ReferenceId = materialResource.Id
//                },
//            };
//
//            resources.Add(pbrMaterialExpression);

            return resources;
        }

        private static MFnDependencyNode GetTexture(MFnDependencyNode material, string plugName)
        {
            MPlug plug = material.findPlug(plugName);

            MPlugArray connectedPlugs = new MPlugArray();
            plug.connectedTo(connectedPlugs, true, false);

            return (
                from p in connectedPlugs
                    select p.node into plugNode
                        where plugNode.apiType == MFn.Type.kFileTexture
                            select new MFnDependencyNode(plugNode)
                ).FirstOrDefault();
        }

        private static MFnDependencyNode GetNormalTexture(MFnDependencyNode material, out float normalFactor)
        {
            normalFactor = 1f;

            MPlug plug = material.findPlug("normalCamera");

            MPlugArray connectedPlugs = new MPlugArray();
            plug.connectedTo(connectedPlugs, true, false);

            foreach (MPlug p in connectedPlugs)
            {
                MObject plugNode = p.node;
                if (plugNode.apiType != MFn.Type.kBump) continue;

                MFnDependencyNode bumpDependencyNode = new MFnDependencyNode(plugNode);

                MPlug bumpDepthPlug = bumpDependencyNode.findPlug("bumpDepth");
                if (bumpDepthPlug != null)
                {
                    normalFactor = bumpDepthPlug.asFloatProperty;
                }

                MPlug bumpValuePlug = bumpDependencyNode.findPlug("bumpValue");
                if (bumpValuePlug == null) continue;

                MPlugArray bumpValueConnections = new MPlugArray();
                bumpValuePlug.connectedTo(bumpValueConnections, true, false);

                foreach (MPlug bvP in bumpValueConnections)
                {
                    if (bvP.node.apiType == MFn.Type.kFileTexture)
                    {
                        return new MFnDependencyNode(bvP.node);
                    }
                }
            }

            return null;
        }
    }
}