namespace DecayEngine.Bullet.Managed.BulletInterop.Collections.Generic
{
    internal delegate T NativeCollectionElementAtDelegate<out T>(int index);
    internal delegate int NativeEnumeratorCountDelegate();

    internal delegate int NativeCollectionIndexOfDelegate<in T>(T element);

    internal delegate void NativeCollectionAddDelegate<in T>(T element);
    internal delegate bool NativeCollectionRemoveDelegate<in T>(T element);
}