using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;
using DecayEngine.Fmod.Managed.FmodInterop.Studio;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Exports.BaseExports.SoundBank;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.Fmod.Managed.Component.SoundBank
{
    public class SoundBankComponent : ISoundBank
    {
        private IGameObject _parentGameObject;
        private IScene _parentScene;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parentGameObject;
        public ByReference<IGameObject> ParentByRef => () => ref _parentGameObject;

        IScene IParentable<IScene>.Parent => _parentScene;
        ByReference<IScene> IParentable<IScene>.ParentByRef => () => ref _parentScene;

        public Type ExportType => typeof(SoundBankExport);
        public SoundBankResource Resource { get; internal set; }

        public bool Persistent { get; set; }

        public bool Active
        {
            get
            {
                if (_parentScene == null && _parentGameObject == null)
                {
                    return false;
                }

                return _soundBankHandle.hasHandle() && _soundBankHandle.isValid();
            }
            set
            {
                if (!Active && value)
                {
                    Load();
                }
                else if (Active && !value)
                {
                    Unload();
                }
            }
        }

        public List<SoundBankComponent> RequiredBanks { get; }

        private Bank _soundBankHandle;

        public SoundBankComponent()
        {
            RequiredBanks = new List<SoundBankComponent>();
        }

        ~SoundBankComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parentGameObject?.RemoveComponent(this);

            _parentScene?.RemoveComponent(this);
            _parentScene = null;

            parent?.AttachComponent(this);
            _parentGameObject = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        void IParentable<IScene>.SetParent(IScene parent)
        {
            _parentScene?.RemoveComponent(this);

            _parentGameObject?.RemoveComponent(this);
            _parentGameObject = null;

            parent?.AttachComponent(this);
            _parentScene = parent;
        }

        IParentable<IScene> IParentable<IScene>.AsParentable<T>()
        {
            return this;
        }

        private void Load()
        {
            foreach (SoundBankComponent requiredBank in RequiredBanks.Where(bank => !bank.Active))
            {
                requiredBank.Active = true;
            }

            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                _soundBankHandle = (Bank) GameEngine.SoundEngine.LoadBank(Resource.Source.GetDataAsByteArray()).Result;
                _soundBankHandle.loadSampleData();
            });
        }

        private void Unload()
        {
            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                _soundBankHandle.unloadSampleData();
                _soundBankHandle.unload();
            });
        }

        public void Destroy()
        {
            GameEngine.SoundEngine.UntrackSoundBank(this);
            Unload();

            SetParent(null);

            Resource = null;

            Destroyed = true;
        }
    }
}