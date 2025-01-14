using System;
using DecayEngine.ModuleSDK.Capability;

namespace DecayEngine.Bullet.Managed.BulletInterop
{
    public abstract class NativeObject : IDestroyable
    {
        private IntPtr _nativeHandle;

        internal IntPtr NativeHandle
        {
            get => _nativeHandle;
            set
            {
                if (IsNativeHandleInitialized && value != IntPtr.Zero)
                {
                    throw new InvalidOperationException("Native Handle is already initialized.");
                }

                if (value == IntPtr.Zero)
                {
                    NativeObjectTracker.Remove(this);
                    FreeUnmanagedHandles();
                    _nativeHandle = IntPtr.Zero;
                }
                else
                {
                    _nativeHandle = value;
                    NativeObjectTracker.Add(this);
//                    AllocateUserObject();
                }
            }
        }

        internal bool IsNativeHandleInitialized => _nativeHandle != IntPtr.Zero;

        public bool Destroyed { get; private set; }

        ~NativeObject()
        {
            Destroy();
        }

        public virtual void Destroy()
        {
            if (Destroyed) return;

            _nativeHandle = IntPtr.Zero;

            Destroyed = true;
        }

//        protected abstract void AllocateUserObject();
        protected abstract void FreeUnmanagedHandles();
    }
}