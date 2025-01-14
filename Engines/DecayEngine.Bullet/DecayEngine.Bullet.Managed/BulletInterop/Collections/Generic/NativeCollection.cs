using System;
using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal class NativeCollection<T> : NativeEnumerable<T>, ICollection<T>
    {
        private readonly NativeCollectionIndexOfDelegate<T> _indexOfDelegate;
        private readonly NativeCollectionAddDelegate<T> _addDelegate;
        private readonly NativeCollectionRemoveDelegate<T> _removeDelegate;

        public bool IsReadOnly => false;

        internal NativeCollection(
            NativeCollectionElementAtDelegate<T> elementAtDelegate, NativeEnumeratorCountDelegate countDelegate,
            NativeCollectionIndexOfDelegate<T> indexOfDelegate, NativeCollectionAddDelegate<T> addDelegate, NativeCollectionRemoveDelegate<T> removeDelegate
        ) : base(elementAtDelegate, countDelegate)
        {
            _indexOfDelegate = indexOfDelegate;
            _addDelegate = addDelegate;
            _removeDelegate = removeDelegate;
        }

        public void Add(T item)
        {
            _addDelegate(item);
        }

        public void Clear()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                Remove(ElementAt(i));
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public int IndexOf(T item)
        {
            return _indexOfDelegate(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(array));

            int count = Count;
            if (arrayIndex + count > array.Length) throw new ArgumentException("Array too small.", nameof(array));

            for (int i = 0; i < count; i++)
            {
                array[arrayIndex + i] = ElementAt(i);
            }
        }

        public bool Remove(T item)
        {
            return _removeDelegate(item);
        }
    }
}