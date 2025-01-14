using System;
using System.Collections.Generic;
using DecayEngine.ModuleSDK.Component;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IComponentable<TComponent> where TComponent : IComponent
    {
        IEnumerable<TComponent> Components { get; }

        void AttachComponent(TComponent component);
        void AttachComponents(IEnumerable<TComponent> components);
        void RemoveComponent(TComponent component);
        void RemoveComponents(IEnumerable<TComponent> components);
        void RemoveComponents(Func<TComponent, bool> predicate);
    }

    public interface IComponentable : IComponentable<IComponent>
    {

    }
}