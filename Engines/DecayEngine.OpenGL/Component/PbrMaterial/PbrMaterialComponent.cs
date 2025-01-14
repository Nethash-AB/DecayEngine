using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Texture.Texture2D;

namespace DecayEngine.OpenGL.Component.PbrMaterial
{
    public class PbrMaterialComponent : IPbrMaterial
    {
        private IGameObject _parent;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => null;

        public PbrMaterialResource Resource { get; set; }
        public bool Active
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }

                return
                    AlbedoTexture != null && AlbedoTexture.Active
                    || NormalTexture != null && NormalTexture.Active
                    || MetallicityTexture != null && MetallicityTexture.Active
                    || RoughnessTexture != null && RoughnessTexture.Active
                    || EmissionTexture != null && EmissionTexture.Active;
            }
            set
            {
                if (!Active && value)
                {
                    if (AlbedoTexture != null)
                    {
                        AlbedoTexture.Active = true;
                    }

                    if (NormalTexture != null)
                    {
                        NormalTexture.Active = true;
                    }

                    if (MetallicityTexture != null)
                    {
                        MetallicityTexture.Active = true;
                    }

                    if (RoughnessTexture != null)
                    {
                        RoughnessTexture.Active = true;
                    }

                    if (EmissionTexture != null)
                    {
                        EmissionTexture.Active = true;
                    }
                }
                else if (Active && !value)
                {
                    if (AlbedoTexture != null)
                    {
                        AlbedoTexture.Active = false;
                    }

                    if (NormalTexture != null)
                    {
                        NormalTexture.Active = false;
                    }

                    if (MetallicityTexture != null)
                    {
                        MetallicityTexture.Active = false;
                    }

                    if (RoughnessTexture != null)
                    {
                        RoughnessTexture.Active = false;
                    }

                    if (EmissionTexture != null)
                    {
                        EmissionTexture.Active = false;
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

                if (AlbedoTexture != null)
                {
                    if (AlbedoTexture.Size.X > AlbedoTexture.Size.Y)
                    {
                        height = AlbedoTexture.Size.Y / AlbedoTexture.Size.X;
                    }
                    else if (AlbedoTexture.Size.X < AlbedoTexture.Size.Y)
                    {
                        width = AlbedoTexture.Size.X / AlbedoTexture.Size.Y;
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
                else if (MetallicityTexture != null)
                {
                    if (MetallicityTexture.Size.X > MetallicityTexture.Size.Y)
                    {
                        height = MetallicityTexture.Size.Y / MetallicityTexture.Size.X;
                    }
                    else if (MetallicityTexture.Size.X < MetallicityTexture.Size.Y)
                    {
                        width = MetallicityTexture.Size.X / MetallicityTexture.Size.Y;
                    }
                }
                else if (RoughnessTexture != null)
                {
                    if (RoughnessTexture.Size.X > RoughnessTexture.Size.Y)
                    {
                        height = RoughnessTexture.Size.Y / RoughnessTexture.Size.X;
                    }
                    else if (RoughnessTexture.Size.X < RoughnessTexture.Size.Y)
                    {
                        width = RoughnessTexture.Size.X / RoughnessTexture.Size.Y;
                    }
                }
                else if (EmissionTexture != null)
                {
                    if (EmissionTexture.Size.X > EmissionTexture.Size.Y)
                    {
                        height = EmissionTexture.Size.Y / EmissionTexture.Size.X;
                    }
                    else if (EmissionTexture.Size.X < EmissionTexture.Size.Y)
                    {
                        width = EmissionTexture.Size.X / EmissionTexture.Size.Y;
                    }
                }

                return new Vector2(width, height);
            }
        }

        public IColorTexture AlbedoTexture { get; set; }
        public Vector4 AlbedoColor { get; set; }
        public INormalTexture NormalTexture { get; set; }
        public IMetallicityTexture MetallicityTexture { get; set; }
        public float MetallicityFactor { get; set; }
        public IRoughnessTexture RoughnessTexture { get; set; }
        public float RoughnessFactor { get; set; }
        public IEmissionTexture EmissionTexture { get; set; }
        public Vector4 EmissionColor { get; set; }

        public void Bind()
        {
            if (AlbedoTexture != null && AlbedoTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    AlbedoTexture.Bind();
                });
            }

            if (NormalTexture != null && NormalTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    NormalTexture.Bind();
                });
            }

            if (MetallicityTexture != null && MetallicityTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    MetallicityTexture.Bind();
                });
            }

            if (RoughnessTexture != null && RoughnessTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    RoughnessTexture.Bind();
                });
            }

            if (EmissionTexture != null && EmissionTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    EmissionTexture.Bind();
                });
            }
        }

        public void Unbind()
        {
            if (AlbedoTexture != null && AlbedoTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    AlbedoTexture.Unbind();
                });
            }

            if (NormalTexture != null && NormalTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    NormalTexture.Unbind();
                });
            }

            if (MetallicityTexture != null && MetallicityTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    MetallicityTexture.Unbind();
                });
            }

            if (RoughnessTexture != null && RoughnessTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    RoughnessTexture.Unbind();
                });
            }

            if (EmissionTexture != null && EmissionTexture.Active)
            {
                GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
                {
                    EmissionTexture.Unbind();
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

        ~PbrMaterialComponent()
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