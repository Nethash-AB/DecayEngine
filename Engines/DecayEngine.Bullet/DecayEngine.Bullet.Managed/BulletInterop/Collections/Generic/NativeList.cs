using System;
using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal class NativeList<T> : NativeCollection<T>, IList<T>
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
            set => Insert(index, value);
        }

        internal NativeList(
            NativeCollectionElementAtDelegate<T> elementAtDelegate, NativeEnumeratorCountDelegate countDelegate,
            NativeCollectionIndexOfDelegate<T> indexOfDelegate, NativeCollectionAddDelegate<T> addDelegate, NativeCollectionRemoveDelegate<T> removeDelegate
        ) : base(elementAtDelegate, countDelegate, indexOfDelegate, addDelegate, removeDelegate)
        {
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));

            T item = ElementAt(index);
            Remove(item);
        }
    }
}