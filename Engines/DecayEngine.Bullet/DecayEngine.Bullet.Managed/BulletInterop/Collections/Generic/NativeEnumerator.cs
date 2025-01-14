using System.Collections;
using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal class NativeEnumerator<T> : IEnumerator<T>
    {
        private readonly NativeCollectionElementAtDelegate<T> _elementAtDelegate;
        private readonly int _count;
        private int _i;

        public T Current => _elementAtDelegate(_i);
        object IEnumerator.Current => Current;

        internal NativeEnumerator(NativeCollectionElementAtDelegate<T> elementAtDelegate, int count)
        {
            _elementAtDelegate = elementAtDelegate;
            _count = count;
            _i = -1;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _i++;
            return _i < _count;
        }

        public void Reset()
        {
            _i = -1;
        }
    }
}