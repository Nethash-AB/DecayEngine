using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.GameStructure;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Capability
{
    public static class ActivableExtensions
    {
        public static bool ActiveInGraph(this IActivable activable)
        {
            switch (activable)
            {
                case IParentable<IGameObject> parentable:
                {
                    if (!activable.Active) return false;

                    IGameStructure parent = parentable.Parent;
                    if (parent == null || !parent.Active) return false;

                    while (parent != null)
                    {
                        switch (parent)
                        {
                            case IScene _:
                                parent = ((IParentable<IScene>) parent).Parent;
                                if (parent != null && !parent.Active)
                                {
                                    return false;
                                }
                                continue;
                            case IGameObject _:
                                parent = ((IParentable<IGameObject>) parent).Parent;
                                if (parent != null && !parent.Active)
                                {
                                    return false;
                                }
                                continue;
                            default:
                                parent = null;
                                break;
                        }
                    }

                    return true;
                }
                case IParentable<IScene> sceneParentable:
                {
                    if (!activable.Active) return false;

                    IGameStructure parent = sceneParentable.Parent;
                    if (parent == null) return false;

                    while (parent != null)
                    {
                        switch (parent)
                        {
                            case IScene _:
                                parent = ((IParentable<IScene>) parent).Parent;
                                if (parent != null && !parent.Active)
                                {
                                    return false;
                                }
                                continue;
                            case IGameObject _:
                                parent = ((IParentable<IGameObject>) parent).Parent;
                                if (parent != null && !parent.Active)
                                {
                                    return false;
                                }
                                continue;
                            default:
                                parent = null;
                                break;
                        }
                    }

                    return true;
                }
                default:
                    return activable.Active;
            }
        }
    }
}