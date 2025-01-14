using System;
using System.Collections;
using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal class NativeEnumerable<T> : IEnumerable<T>
    {
        private readonly NativeCollectionElementAtDelegate<T> _elementAtDelegate;
        private readonly NativeEnumeratorCountDelegate _countDelegate;

        public int Count => _countDelegate();

        internal NativeEnumerable(NativeCollectionElementAtDelegate<T> elementAtDelegate, NativeEnumeratorCountDelegate countDelegate)
        {
            _elementAtDelegate = elementAtDelegate;
            _countDelegate = countDelegate;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new NativeEnumerator<T>(_elementAtDelegate, _countDelegate());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T ElementAt(int index)
        {
            if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
            return _elementAtDelegate(index);
        }
    }
}