using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Single;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Component.Shader;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Expression
{
    public class QueryExpressionResolver
    {
        private readonly IQueryInitiatorExpression _rootExpression;

        public QueryExpressionResolver(IQueryInitiatorExpression rootExpression)
        {
            _rootExpression = rootExpression;
        }

        public object Resolve(IGameObject target)
        {
            return ResolveInitiator(_rootExpression, target);
        }

        public object Resolve(IScene target)
        {
            return ResolveInitiator(_rootExpression, target);
        }

        public object Resolve(IComponent target)
        {
            return ResolveInitiator(_rootExpression, target);
        }

        private object ResolveInitiator(IQueryInitiatorExpression initiator, object target)
        {
            IQueryExpression next;
            switch (initiator)
            {
                case SelectActiveSceneExpression selectActiveScene:
                    next = selectActiveScene.Next;
                    return next == null ? null : ResolveQuery(next, GameEngine.ActiveScene);
                case SelectThisExpression selectThis:
                    next = selectThis.Next;
                    if (!(target is IScene) && !(target is IGameObject)) return null;
                    return next == null ? null : ResolveQuery(next, target);
                case SelectGlobalExpression selectGlobal:
                    next = selectGlobal.Next;
                    return ResolveQuery(next, null);
                case SelectRootExpression selectRoot:
                    next = selectRoot.Next;
                    if (!(target is IGameObject targetGameObject)) return null;

                    IGameObject parent = targetGameObject;
                    while (((IParentable<IGameObject>) parent).Parent != null)
                    {
                        parent = ((IParentable<IGameObject>) parent).Parent;
                    }

                    return ResolveQuery(next, parent);
                default:
                    return null;
            }
        }

        private object ResolveQuery(IQueryExpression query, object target)
        {
            return query switch
            {
                IQueryCollectionExpression collectionExpression => ResolveCollection(collectionExpression, target),
                IQuerySingleExpression singleExpression => ResolveSingle(singleExpression, target),
                _ => null
            };
        }

        private object ResolveCollection(IQueryCollectionExpression query, object target)
        {
            switch (query)
            {
                case SelectChildrenExpression selectChildren:
                    IQueryCollectionExpression nextChildren = (IQueryCollectionExpression) selectChildren.Next;
                    if (nextChildren == null) return null;

                    if (!(target is IChildBearer<IGameObject> childBearer)) return null;
                    return ResolveCollection(nextChildren, childBearer.Children);
                case SelectComponentsExpression selectComponents:
                    IQueryCollectionExpression nextComponents = (IQueryCollectionExpression) selectComponents.Next;
                    if (nextComponents == null) return null;

                    return target switch
                    {
                        IComponentable componentable => ResolveCollection(nextComponents, componentable.Components),
                        IComponentable<ISceneAttachableComponent> sceneAttachableComponent =>
                            ResolveCollection(nextComponents, sceneAttachableComponent.Components),
                        _ => null
                    };
                case IQueryCollectionFilterExpression filterExpression:
                    return !(target is IEnumerable) ? null : ResolveCollectionFilter(filterExpression, (IEnumerable) target);
                case IQueryCollectionTerminatorExpression terminatorExpression:
                    if (target == null)
                    {
                        return ResolveCollectionTerminator(terminatorExpression, null);
                    }
                    return !(target is IEnumerable) ? null : ResolveCollectionTerminator(terminatorExpression, (IEnumerable) target);
                default:
                    return null;
            }
        }

        private object ResolveSingle(IQuerySingleExpression query, object target)
        {
            IQueryExpression nextQuery = query.Next;
            return query switch
            {
                SelectParentExpression _ => (target switch
                {
                    IParentable<IGameObject> goParentable => ResolveQuery(nextQuery, goParentable),
                    IParentable<IScene> sceneParentable => ResolveQuery(nextQuery, sceneParentable),
                    _ => null
                }),
                _ => null
            };
        }

        private object ResolveCollectionFilter(IQueryCollectionFilterExpression filter, IEnumerable target)
        {
            IQueryCollectionExpression next = (IQueryCollectionExpression) filter.Next;
            switch (filter)
            {
                case FilterByComponentTypeExpression byComponentType:
                    if (!(target is IEnumerable<IComponent> enumerableComponents)) return null;

                    return byComponentType.Type switch
                    {
                        ComponentType.Camera => ResolveCollection(next, enumerableComponents.OfType<ICamera>()),
                        ComponentType.CameraOrtho => ResolveCollection(next, enumerableComponents.OfType<ICameraOrtho>()),
                        ComponentType.CameraPersp => ResolveCollection(next, enumerableComponents.OfType<ICameraPersp>()),
                        ComponentType.AnimatedMaterial => ResolveCollection(next, enumerableComponents.OfType<IAnimatedMaterial>()),
                        ComponentType.ShaderProgram => ResolveCollection(next, enumerableComponents.OfType<IShaderProgram>()),
                        ComponentType.Shader => ResolveCollection(next, enumerableComponents.OfType<IShader>()),
                        ComponentType.Sound => ResolveCollection(next, enumerableComponents.OfType<ISound>()),
                        ComponentType.SoundBank => ResolveCollection(next, enumerableComponents.OfType<ISoundBank>()),
                        ComponentType.Script => ResolveCollection(next, enumerableComponents.OfType<IScript>()),
                        ComponentType.AnimatedSprite => ResolveCollection(next, enumerableComponents.OfType<IAnimatedSprite>()),
                        ComponentType.RigidBody => ResolveCollection(next, enumerableComponents.OfType<IRigidBody>()),
                        _ => null
                    };
                case FilterByNameExpression byName:
                    if (!(target is IEnumerable<INameable> nameables)) return null;
                    return ResolveCollection(next, nameables.Where(nameable => nameable.Name == byName.Name));
                default:
                    return null;
            }
        }

        private static object ResolveCollectionTerminator(IQueryCollectionTerminatorExpression terminator, IEnumerable target)
        {
            switch (terminator)
            {
                case SelectFirstCollectionTerminatorExpression _:
                    IEnumerator enumerator = target.GetEnumerator();
                    enumerator.MoveNext();
                    return enumerator.Current;
                case SelectAllCollectionTerminatorExpression _:
                    return target;
                case SelectFrameBufferTerminatorExpression frameBufferTerminatorExpression:
                    switch (target)
                    {
                        case null:
                            return GameEngine.RenderEngine.GlobalFrameBuffers.FirstOrDefault(fbo => fbo.Name == frameBufferTerminatorExpression.Name);
                        case IEnumerable<ICamera> cameras:
                            if (string.IsNullOrEmpty(frameBufferTerminatorExpression.Name)) return cameras.FirstOrDefault()?.FrameBuffers.LastOrDefault();

                            ICamera camera = cameras.FirstOrDefault();
                            if (camera == null) return null;

                            List<IFrameBuffer> frameBuffers = camera.FrameBuffers.ToList();
                            int index = frameBuffers.FindIndex(fbo => fbo.Name == frameBufferTerminatorExpression.Name);
                            if (index < 0) return null;

                            return index + 1 >= frameBuffers.Count ? frameBuffers.LastOrDefault() : frameBuffers[index + 1];

                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }
    }
}