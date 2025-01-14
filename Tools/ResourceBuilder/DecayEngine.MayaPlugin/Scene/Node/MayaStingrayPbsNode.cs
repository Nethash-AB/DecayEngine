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
    public class MayaStingrayPbsNode : IMayaNode
    {
        public MayaScene Scene { get; set; }
        public MObject MayaObject { get; }
        public List<IMayaNode> Children { get; }

        public MayaStingrayPbsNode(MObject mayaObject)
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

            MFnDependencyNode dependencyNodeFn = new MFnDependencyNode(MayaObject);

            // Albedo
            IPropertyExpression albedoTexture = null;
            Vector4Structure albedoColor = new Vector4Structure
            {
                X = 1f,
                Y = 1f,
                Z = 1f,
                W = 1f
            };

            bool useColorMap = dependencyNodeFn.findPlug("use_color_map").asBoolProperty;
            if (useColorMap)
            {
                MFnDependencyNode colorTextureFn = GetTexture(dependencyNodeFn, "TEX_color_map");
                if (colorTextureFn != null)
                {
                    albedoTexture = new ResourceReferenceExpression
                    {
                        ReferenceId = colorTextureFn.name
                    };
                }
            }
            else
            {
                MPlug colorPlug = dependencyNodeFn.findPlug("base_color");
            }

            // Normal
            IPropertyExpression normalTexture = null;

            bool useNormalMap = dependencyNodeFn.findPlug("use_normal_map").asBoolProperty;
            if (useNormalMap)
            {
                MFnDependencyNode normalTextureFn = GetTexture(dependencyNodeFn, "TEX_normal_map");
                if (normalTextureFn != null)
                {
                    normalTexture = new ResourceReferenceExpression
                    {
                        ReferenceId = normalTextureFn.name
                    };
                }
            }

            // Metallicity
            IPropertyExpression metallicityTexture = null;
            float metallicityFactor = 1f;

            bool useMetallicityMap = dependencyNodeFn.findPlug("use_metallic_map").asBoolProperty;
            if (useMetallicityMap)
            {
                MFnDependencyNode metallicityTextureFn = GetTexture(dependencyNodeFn, "TEX_metallic_map");
                if (metallicityTextureFn != null)
                {
                    metallicityTexture = new ResourceReferenceExpression
                    {
                        ReferenceId = metallicityTextureFn.name
                    };
                }
            }
            else
            {
                MPlug metallicityPlug = dependencyNodeFn.findPlug("metallic");
                metallicityFactor = metallicityPlug.asFloatProperty;
            }

            // Roughness
            IPropertyExpression roughnessTexture = null;
            float roughnessFactor = 1f;

            bool useRoughnessMap = dependencyNodeFn.findPlug("use_roughness_map").asBoolProperty;
            if (useRoughnessMap)
            {
                MFnDependencyNode roughnessTextureFn = GetTexture(dependencyNodeFn, "TEX_roughness_map");
                if (roughnessTextureFn != null)
                {
                    roughnessTexture = new ResourceReferenceExpression
                    {
                        ReferenceId = roughnessTextureFn.name
                    };
                }
            }
            else
            {
                MPlug roughnessPlug = dependencyNodeFn.findPlug("roughness");
                roughnessFactor = roughnessPlug.asFloatProperty;
            }

            // Emission
            IPropertyExpression emissionTexture = null;
            Vector4Structure emissionColor = new Vector4Structure
            {
                X = 1f,
                Y = 1f,
                Z = 1f,
                W = 1f
            };

            bool useEmissionMap = dependencyNodeFn.findPlug("use_emissive_map").asBoolProperty;
            if (useEmissionMap)
            {
                MFnDependencyNode emissionTextureFn = GetTexture(dependencyNodeFn, "TEX_emissive_map");
                if (emissionTextureFn != null)
                {
                    emissionTexture = new ResourceReferenceExpression
                    {
                        ReferenceId = emissionTextureFn.name
                    };
                }
            }
            else
            {
                MPlug emissionPlug = dependencyNodeFn.findPlug("emissive");
                float emissionIntensity = dependencyNodeFn.findPlug("emissive_intensity").asFloatProperty;

                emissionColor = new Vector4Structure
                {
                    X = emissionColor.X * emissionIntensity,
                    Y = emissionColor.Y * emissionIntensity,
                    Z = emissionColor.Z * emissionIntensity,
                    W = emissionColor.W * emissionIntensity
                };
            }

            PbrMaterialResource materialResource = new PbrMaterialResource
            {
                Id = dependencyNodeFn.name,
                MetaFilePath = $"{Path.GetFileNameWithoutExtension(Scene.MetaFileName)}_materials.meta",
                AlbedoTexture = albedoTexture,
                AlbedoColor = albedoColor,
                NormalTexture = normalTexture,
                MetallicityTexture = metallicityTexture,
                MetallicityFactor = metallicityFactor,
                RoughnessTexture = roughnessTexture,
                RoughnessFactor = roughnessFactor,
                EmissionTexture = emissionTexture,
                EmissionColor = emissionColor
            };

            resources.Add(materialResource);

            CreatePbrMaterialComponentExpression pbrMaterialExpression = new CreatePbrMaterialComponentExpression
            {
                Name = $"material_{materialResource.Id}",
                Active = true,
                Template = new ResourceReferenceExpression
                {
                    ReferenceId = materialResource.Id
                },
            };

            resources.Add(pbrMaterialExpression);

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
    }
}