using System;
using System.Collections.Generic;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using DecayEngine.DecPakLib.Resource.Expression.Statement.GameObject;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using DecayEngine.ModuleSDK.Math;

namespace DecayEngine.MayaPlugin.Scene.Node
{
    public class MayaTransformNode : IMayaNode
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

        public MayaTransformNode(MObject mayaObject)
        {
            MayaObject = mayaObject;
            Children = new List<IMayaNode>();
        }

        public IEnumerable<IResource> ToDecayResource(Uri outputDirUri)
        {
            MFnDagNode dagNodeFn = new MFnDagNode(MayaObject);
            MTransformationMatrix transformationMatrix = new MTransformationMatrix(dagNodeFn.transformationMatrix);

            MVector position = transformationMatrix.translation(MSpace.Space.kTransform);
            Vector3Structure positionStructure = new Vector3Structure
            {
                X = (float) position.x,
                Y = (float) position.y,
                Z = (float) position.z
            };

            MEulerRotation rotation = transformationMatrix.eulerRotation;
            MEulerRotation.RotationOrder rotationOrder = transformationMatrix.rotationOrder;
            Vector3Structure rotationStructure = new Vector3Structure();
            switch (rotationOrder)
            {
                case MEulerRotation.RotationOrder.kXYZ:
                    rotationStructure.X = (float) rotation.x;
                    rotationStructure.Y = (float) rotation.y;
                    rotationStructure.Z = (float) rotation.z;
                    break;
                case MEulerRotation.RotationOrder.kXZY:
                    rotationStructure.X = (float) rotation.x;
                    rotationStructure.Y = (float) rotation.z;
                    rotationStructure.Z = (float) rotation.y;
                    break;
                case MEulerRotation.RotationOrder.kYXZ:
                    rotationStructure.X = (float) rotation.y;
                    rotationStructure.Y = (float) rotation.x;
                    rotationStructure.Z = (float) rotation.z;
                    break;
                case MEulerRotation.RotationOrder.kYZX:
                    rotationStructure.X = (float) rotation.y;
                    rotationStructure.Y = (float) rotation.z;
                    rotationStructure.Z = (float) rotation.x;
                    break;
                case MEulerRotation.RotationOrder.kZXY:
                    rotationStructure.X = (float) rotation.z;
                    rotationStructure.Y = (float) rotation.x;
                    rotationStructure.Z = (float) rotation.y;
                    break;
                case MEulerRotation.RotationOrder.kZYX:
                    rotationStructure.X = (float) rotation.z;
                    rotationStructure.Y = (float) rotation.y;
                    rotationStructure.Z = (float) rotation.x;
                    break;
            }

            rotationStructure = new Vector3Structure
            {
                X = MathHelper.RadiansToDegrees(rotationStructure.X),
                Y = MathHelper.RadiansToDegrees(rotationStructure.Y),
                Z = MathHelper.RadiansToDegrees(rotationStructure.Z)
            };

            double[] scaleComponents = new double[3];
            transformationMatrix.getScale(scaleComponents, MSpace.Space.kTransform);
            Vector3Structure scaleStructure = new Vector3Structure
            {
                X = (float) scaleComponents[0],
                Y = (float) scaleComponents[1],
                Z = (float) scaleComponents[2]
            };

            TransformStructure transformStructure = new TransformStructure
            {
                Position = positionStructure,
                Rotation = rotationStructure,
                Scale = scaleStructure
            };

            CreateGameObjectExpression gameObjectExpression = new CreateGameObjectExpression
            {
                Name = dagNodeFn.name,
                Active = true,
                Children = new List<IStatementExpression>(),
                Transform = transformStructure
            };

            List<IResource> resources = new List<IResource> {gameObjectExpression};

            foreach (IMayaNode mayaSceneNode in Children)
            {
                foreach (IResource resource in mayaSceneNode.ToDecayResource(outputDirUri))
                {
                    switch (resource)
                    {
                        case IStatementExpression statement:
                            gameObjectExpression.Children.Add(statement);
                            break;
                        case IRootResource rootResource:
                            resources.Add(rootResource);
                            break;
                    }
                }
            }

            return resources;
        }
    }
}