using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Light;
using DecayEngine.ModuleSDK.Object.GameObject;

namespace DecayEngine.OpenGL.Component.Light.Directional
{
    public class DirectionalLightComponent : IDirectionalLight
    {
        private IGameObject _parent;

        private bool _active;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public virtual Type ExportType => null;

        public bool Active
        {
            get
            {
                if (_parent == null)
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
                }
                else if (Active && !value)
                {
                    _active = false;
                }
            }
        }

        public Vector3 Color { get; set; }
        public float Strength { get; set; }

        public DirectionalLightComponent()
        {
        }

        ~DirectionalLightComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parent?.RemoveComponent(this);

            parent?.AttachComponent(this);
            _parent = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        public void Destroy()
        {
            SetParent(null);

            Destroyed = true;
        }
    }
}