using System;
using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal class NativeReadOnlyList<T> : NativeReadOnlyCollection<T>, IReadOnlyList<T>
    {
        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return ElementAt(index);
            }
        }

        public NativeReadOnlyList(NativeCollectionElementAtDelegate<T> elementAtDelegate, NativeEnumeratorCountDelegate countDelegate)
            : base(elementAtDelegate, countDelegate)
        {
        }
    }
}