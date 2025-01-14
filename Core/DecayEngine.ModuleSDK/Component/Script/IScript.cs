using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Resource.RootElement.Script;
using DecayEngine.DecPakLib.Resource.Structure.Component.Script;
using DecayEngine.ModuleSDK.Capability;

namespace DecayEngine.ModuleSDK.Component.Script
{
    public interface IScript : ISceneAttachableComponent, IComponent<ScriptResource>, IScriptUpdateable, IRenderUpdateable
    {
        List<ScriptInjection> Injections { get; set; }

        Task OnInit();
        object GetProperty(string propertyName);
        void SetProperty(string propertyName, object value);
    }
}