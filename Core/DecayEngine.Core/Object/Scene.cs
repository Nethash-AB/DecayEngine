using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib.Resource.RootElement.Scene;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.Core.Object
{
    public class Scene : IScene
    {
        private readonly List<IGameObject> _children;
        private readonly List<ISceneAttachableComponent> _components;

        public string Name => Resource.Id;
        public bool Active
        {
            get => GameEngine.ActiveScene == this;
            set { }
        }

        public SceneResource Resource { get; }
        public IEnumerable<IGameObject> Children => _children;
        public IEnumerable<ISceneAttachableComponent> Components => _components;

        public Scene(SceneResource resource)
        {
            Resource = resource;
            _children = new List<IGameObject>();
            _components = new List<ISceneAttachableComponent>();
        }

        public void AddChild(IGameObject child)
        {
            if (_children.Contains(child) || child == null) return;

            _children.Add(child);
            child.SetParent(this);
        }

        public void AddChildren(IEnumerable<IGameObject> children)
        {
            foreach (IGameObject child in children)
            {
                if (_children.Contains(child) || child == null) continue;

                _children.Add(child);
                child.SetParent(this);
            }
        }

        public void RemoveChild(IGameObject child)
        {
            if (!_children.Contains(child) || child == null) return;

            _children.Remove(child);
            child.SetParent((IGameObject) null);
        }

        public void RemoveChildren(IEnumerable<IGameObject> children)
        {
            foreach (IGameObject child in _children.Where(child => _children.Contains(child)))
            {
                child.SetParent((IGameObject) null);
            }
        }

        public void RemoveChildren(Func<IGameObject, bool> predicate)
        {
            foreach (IGameObject child in _children.Where(predicate))
            {
                child.SetParent((IGameObject) null);
            }
        }

        public void AttachComponent(ISceneAttachableComponent component)
        {
            if (_components.Contains(component)) return;

            component.ClearParent();

            _components.Add(component);
            component.SetParent(this);
        }

        public void AttachComponents(IEnumerable<ISceneAttachableComponent> components)
        {
            foreach (ISceneAttachableComponent component in components)
            {
                if (_components.Contains(component)) continue;

                component.ClearParent();

                _components.Add(component);
                component.SetParent(this);
            }
        }

        public void RemoveComponent(ISceneAttachableComponent component)
        {
            if (!_components.Contains(component)) return;

            _components.Remove(component);
            component.SetParent((IScene) null);
        }

        public void RemoveComponents(IEnumerable<ISceneAttachableComponent> components)
        {
            foreach (ISceneAttachableComponent component in components.Where(component => _components.Contains(component)))
            {
                component.SetParent((IScene) null);
            }
        }

        public void RemoveComponents(Func<ISceneAttachableComponent, bool> predicate)
        {
            foreach (ISceneAttachableComponent component in _components.Where(predicate))
            {
                component.SetParent((IScene) null);
            }
        }
    }
}