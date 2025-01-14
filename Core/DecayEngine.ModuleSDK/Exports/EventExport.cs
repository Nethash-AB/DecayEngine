using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports
{
    [ScriptExportClass("EventHandler", "Represents a wrapper for a Managed event handler.")]
    public class EventExport<TDelegate> : IManagedWrapper where TDelegate : Delegate
    {
        private readonly List<TDelegate> _handler;
        public int Type => (int) ManagedExportType.EventHandler;
        public string SubType => typeof(TDelegate).ToString();

        [ScriptExportConstructor]
        public EventExport()
        {
            _handler = new List<TDelegate>();
        }

        [ScriptExportMethod("Subscribes a function to the event.")]
        public void Subscribe(
            [ScriptExportParameter("The function to subscribe to the `EventHandler`.")] TDelegate func
        )
        {
            _handler.Add(func);
        }

        [ScriptExportMethod("Unsubscribes a function to the event.")]
        public void UnSubscribe(
            [ScriptExportParameter("The function to unsubscribe to the `EventHandler`.")] TDelegate func
        )
        {
            _handler.Remove(func);
        }

        [ScriptExportMethod("Clears all functions from the event.")]
        public void Clear()
        {
            _handler.Clear();
        }

        [ScriptExportMethod("Fires the event, calling all the subscribed functions with no guartanteed order.\n" +
        "Arguments must match the event signature.")]
        public void Fire(
            [ScriptExportParameter("The arguments of the `EventHandler`.")] params object[] args
        )
        {
            if (_handler.Count < 1) return;

            _handler.ToList().ForEach(eventDelegate => eventDelegate?.DynamicInvoke(args));
        }
    }
}