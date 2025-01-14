using System.Linq;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.ModuleSDK.Capability
{
    public static class TransformableExtensions
    {
        public static Transform GetWorldSpaceTransform(this ITransformable transformable)
        {
            if (transformable is IParentable<IScene> sceneParentable && sceneParentable.Parent != null)
            {
                return transformable.Transform;
            }

            if (!(transformable is IParentable<IGameObject> parentable))
            {
                return transformable.Transform;
            }

            Transform worldSpaceTransform = new Transform
            {
                Position = transformable.Transform.Position,
                Rotation = transformable.Transform.Rotation,
                Scale = transformable.Transform.Scale
            };
            IGameObject parent = parentable.Parent;
            while (parent != null)
            {
                worldSpaceTransform.Position += parent.Transform.Position;
                worldSpaceTransform.Rotation += parent.Transform.Rotation;
                worldSpaceTransform.Scale *= parent.Transform.Scale;

                parent = ((IParentable<IGameObject>) parent).Parent;
            }

            return worldSpaceTransform;
        }

        public static void MoveInMode(this ITransformable transformable, TransformMode mode, Transform transform)
        {
            MoveInMode(transformable, mode, transform.Position, transform.Rotation, transform.Scale);
        }

        public static void MoveInMode(this ITransformable transformable, TransformMode mode, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            switch (mode)
            {
                case TransformMode.Absolute:
                    transformable.Transform.Position = position;
                    transformable.Transform.Rotation = rotation;
                    transformable.Transform.Scale = scale;
                    break;
                case TransformMode.WorldSpace:
                    if (!(transformable is IParentable<IGameObject> parentable) || parentable.Parent == null)
                    {
                        transformable.Transform.Position = position;
                        transformable.Transform.Rotation = rotation;
                        transformable.Transform.Scale = scale;
                    }
                    else
                    {
                        transformable.Transform.Position = scale - parentable.Parent.Transform.Position;
                        transformable.Transform.Rotation = rotation * parentable.Parent.Transform.Rotation;
                        transformable.Transform.Scale = new Vector3(
                            scale.X / parentable.Parent.Transform.Scale.X,
                            scale.Y / parentable.Parent.Transform.Scale.Y,
                            scale.Z / parentable.Parent.Transform.Scale.Z
                        );
                    }
                    break;
                case TransformMode.OrthoRelative:
                    if (!(transformable is IDrawable drawable)) return;

                    ICameraOrtho cameraOrtho = GameEngine.RenderEngine.Cameras
                        .OfType<ICameraOrtho>()
                        .FirstOrDefault(camera => camera.Drawables.Contains(drawable));
                    if (cameraOrtho == null) return;

                    cameraOrtho.UpdateCameraProperties();

                    transformable.Transform.Position = new Vector3(
                        cameraOrtho.ViewSpaceBBox.X * position.X,
                        cameraOrtho.ViewSpaceBBox.Y * position.Y,
                        position.Z * (float) System.Math.Sqrt(System.Math.Pow(cameraOrtho.ViewSpaceBBox.X, 2) + System.Math.Pow(cameraOrtho.ViewSpaceBBox.Y, 2))
//                        position.Z * cameraOrtho.ZFar
                    );
                    transformable.Transform.Rotation = cameraOrtho.Transform.Rotation * rotation;
                    transformable.Transform.Scale = cameraOrtho.Transform.Scale * scale;
                    break;
            }
        }
    }
}