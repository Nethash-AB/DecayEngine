using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal class NativeReadOnlyCollection<T> : NativeEnumerable<T>, IReadOnlyCollection<T>
    {
        internal NativeReadOnlyCollection(NativeCollectionElementAtDelegate<T> elementAtDelegate, NativeEnumeratorCountDelegate countDelegate)
            : base(elementAtDelegate, countDelegate)
        {
        }
    }
}