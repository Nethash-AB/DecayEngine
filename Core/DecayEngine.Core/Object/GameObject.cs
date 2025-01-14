using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.Core.Object
{
    public class GameObject : IGameObject
    {
        private bool _active;
        private Transform _transform;
        private readonly List<IGameObject> _children;
        private IGameObject _parentGameObject;
        private IScene _parentScene;
        private readonly List<IComponent> _components;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }
        public bool Active
        {
            get
            {
                if (_parentScene == null && _parentGameObject == null)
                {
                    return false;
                }

                return _active;
            }
            set
            {
                if (!Active && value)
                {
                    _active = true;

                    foreach (IScript script in Components.OfType<IScript>())
                    {
                        script.OnInit().Wait();
                    }
                }
                else if (_active && !value)
                {
                    _active = false;
                }
            }
        }

        public bool Persistent { get; set; }

        public Transform Transform => _transform;
        public ByReference<Transform> TransformByRef => () => ref _transform;
        public Transform WorldSpaceTransform => this.GetWorldSpaceTransform();

        public PrefabResource Resource { get; set; }
        public IEnumerable<IGameObject> Children => _children;

        IScene IParentable<IScene>.Parent => _parentScene;
        ByReference<IScene> IParentable<IScene>.ParentByRef => () => ref _parentScene;

        IGameObject IParentable<IGameObject>.Parent => _parentGameObject;
        ByReference<IGameObject> IParentable<IGameObject>.ParentByRef => () => ref _parentGameObject;

        public IEnumerable<IComponent> Components => _components;

        internal GameObject()
        {
            _transform = new Transform();
            _children = new List<IGameObject>();
            _components = new List<IComponent>();
            Persistent = false;
        }

        ~GameObject()
        {
            Destroy();
        }

        public void Destroy()
        {
            Active = false;

            ((IGameObject) this).SetParent((IGameObject) null);

            foreach (IGameObject child in _children)
            {
                child.Destroy();
            }
            _children.Clear();

            foreach (IComponent component in _components)
            {
                component.Destroy();
            }
            _components.Clear();

            _transform = null;

            Destroyed = true;
        }

        void IParentable<IGameObject>.SetParent(IGameObject parent)
        {
            _parentGameObject?.RemoveChild(this);

            _parentScene?.RemoveChild(this);
            _parentScene = null;

            parent?.AddChild(this);
            _parentGameObject = parent;
        }

        IParentable<IGameObject> IParentable<IGameObject>.AsParentable<T>()
        {
            return this;
        }

        void IParentable<IScene>.SetParent(IScene parent)
        {
            _parentScene?.RemoveChild(this);

            _parentGameObject?.RemoveChild(this);
            _parentGameObject = null;

            parent?.AddChild(this);
            _parentScene = parent;
        }

        IParentable<IScene> IParentable<IScene>.AsParentable<T>()
        {
            return this;
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

        public void AttachComponent(IComponent component)
        {
            if (_components.Contains(component)) return;

            if (component is IRigidBody && _components.OfType<IRigidBody>().Any())
            {
                throw new ArgumentException(
                    $"Tried to attach Rigid Body ({component.Name}) to Game Object ({Name}) but this Game Object already contains a Rigid Body.",
                    nameof(component),
                    new Exception("Only one RigidBody allowed per Game Object.")
                );
            }

            _components.Add(component);
            component.SetParent(this);
        }

        public void AttachComponents(IEnumerable<IComponent> components)
        {
            foreach (IComponent component in components)
            {
                if (_components.Contains(component)) continue;

                if (component is IRigidBody && _components.OfType<IRigidBody>().Any())
                {
                    throw new ArgumentException(
                        $"Tried to attach Rigid Body ({component.Name}) to Game Object ({Name}) but this Game Object already contains a Rigid Body.",
                        nameof(component),
                        new Exception("Only one RigidBody allowed per Game Object.")
                    );
                }

                component.ClearParent();

                _components.Add(component);
                component.SetParent(this);
            }
        }

        public void RemoveComponent(IComponent component)
        {
            if (!_components.Contains(component)) return;

            _components.Remove(component);
            component.SetParent(null);
        }

        public void RemoveComponents(IEnumerable<IComponent> components)
        {
            foreach (IComponent component in components.Where(component => _components.Contains(component)))
            {
                component.SetParent(null);
            }
        }

        public void RemoveComponents(Func<IComponent, bool> predicate)
        {
            foreach (IComponent component in _components.Where(predicate))
            {
                component.SetParent(null);
            }
        }
    }
}