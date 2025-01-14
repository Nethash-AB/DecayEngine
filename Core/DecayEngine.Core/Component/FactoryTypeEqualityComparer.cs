using System;
using System.Collections.Generic;

namespace DecayEngine.Core.Component
{
    public class FactoryTypeEqualityComparer : IEqualityComparer<Type>
    {
        public bool Equals(Type x, Type y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return y.IsAssignableFrom(x);
        }

        public int GetHashCode(Type obj)
        {
            return obj.GetHashCode();
        }
    }
}