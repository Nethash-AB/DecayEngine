using System;
using System.Collections.Generic;

namespace DecayEngine.Bullet.Managed.BulletInterop
{
    public static class NativeObjectTracker
    {
        private static readonly Dictionary<IntPtr, NativeObject> NativeObjectsInternal;

        public static IEnumerable<NativeObject> NativeObjects => NativeObjectsInternal.Values;

        static NativeObjectTracker()
        {
            NativeObjectsInternal = new Dictionary<IntPtr, NativeObject>();
        }

        public static T Get<T>(IntPtr nativeHandle) where T : NativeObject
        {
            if (nativeHandle == IntPtr.Zero) return null;
            if (NativeObjectsInternal.ContainsKey(nativeHandle) && NativeObjectsInternal[nativeHandle] is T managedObject) return managedObject;
            return null;
        }

        public static void Add(NativeObject nativeObject)
        {
            IntPtr handle = nativeObject.NativeHandle;
            if (handle == IntPtr.Zero) return;

            if (NativeObjectsInternal.ContainsKey(handle)) return;
            NativeObjectsInternal[handle] = nativeObject;
        }

        public static void Remove(NativeObject nativeObject)
        {
            IntPtr handle = nativeObject.NativeHandle;
            if (handle == IntPtr.Zero) return;

            if (!NativeObjectsInternal.ContainsKey(handle)) return;
            NativeObjectsInternal.Remove(handle);
        }

        public static void Clear()
        {
            NativeObjectsInternal.Clear();
        }
    }
}