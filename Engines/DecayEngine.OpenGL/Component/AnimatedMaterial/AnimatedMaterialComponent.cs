using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedMaterial;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Texture.Texture2D;

namespace DecayEngine.OpenGL.Component.AnimatedMaterial
{
    public class AnimatedMaterialComponent : IAnimatedMaterial
    {
        private IGameObject _parent;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => typeof(AnimatedMaterialExport);

        public AnimatedMaterialResource Resource { get; set; }
        public bool Active
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }

                return DiffuseTexture != null && DiffuseTexture.Active || NormalTexture != null && NormalTexture.Active;
            }
            set
            {
                if (!Active && value)
                {
                    if (DiffuseTexture != null)
                    {
                        DiffuseTexture.Active = true;
                    }

                    if (NormalTexture != null)
                    {
                        NormalTexture.Active = true;
                    }
                }
                else if (Active && !value)
                {
                    if (DiffuseTexture != null)
                    {
                        DiffuseTexture.Active = false;
                    }

                    if (NormalTexture != null)
                    {
                        NormalTexture.Active = false;
                    }
                }
            }
        }

        public Vector2 AspectRatio
        {
            get
            {
                float width = 1f;
                float height = 1f;

                if (DiffuseTexture != null)
                {
                    if (DiffuseTexture.Size.X > DiffuseTexture.Size.Y)
                    {
                        height = DiffuseTexture.Size.Y / DiffuseTexture.Size.X;
                    }
                    else if (DiffuseTexture.Size.X < DiffuseTexture.Size.Y)
                    {
                        width = DiffuseTexture.Size.X / DiffuseTexture.Size.Y;
                    }
                }
                else if (NormalTexture != null)
                {
                    if (NormalTexture.Size.X > NormalTexture.Size.Y)
                    {
                        height = NormalTexture.Size.Y / NormalTexture.Size.X;
                    }
                    else if (NormalTexture.Size.X < NormalTexture.Size.Y)
                    {
                        width = NormalTexture.Size.X / NormalTexture.Size.Y;
                    }
                }

                return new Vector2(width, height);
            }
        }

        public IColorTexture DiffuseTexture { get; set; }
        public INormalTexture NormalTexture { get; set; }

        public List<AnimationFrameElement> AnimationFrames => Resource.AnimationFrames.ToList();

        public void Bind()
        {
            if (DiffuseTexture != null && DiffuseTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    DiffuseTexture.Bind();
                });
            }

            if (NormalTexture != null && NormalTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    NormalTexture.Bind();
                });
            }
        }

        public void Unbind()
        {
            if (DiffuseTexture != null && DiffuseTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    DiffuseTexture.Unbind();
                });
            }

            if (NormalTexture != null && NormalTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    NormalTexture.Unbind();
                });
            }
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

        ~AnimatedMaterialComponent()
        {
            Destroy();
        }

        public void Destroy()
        {
            SetParent(null);

            Destroyed = true;
        }
    }
}