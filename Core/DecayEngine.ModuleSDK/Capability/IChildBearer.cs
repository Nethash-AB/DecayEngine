using System;
using System.Collections.Generic;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IChildBearer<TChild>
    {
        IEnumerable<TChild> Children { get; }

        void AddChild(TChild child);
        void AddChildren(IEnumerable<TChild> children);
        void RemoveChild(TChild child);
        void RemoveChildren(IEnumerable<TChild> children);
        void RemoveChildren(Func<TChild, bool> predicate);
    }
}