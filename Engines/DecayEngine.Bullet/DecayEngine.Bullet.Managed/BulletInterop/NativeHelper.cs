using System;
using System.Runtime.InteropServices;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.Bullet.Managed.BulletInterop
{
    public static class NativeHelper
    {
        public static T GetManagedObjectFromUserHandle<T>(IntPtr nativeHandle) where T : NativeObject
        {
            if (nativeHandle == IntPtr.Zero) return null;
            try
            {
                return NativeObjectTracker.Get<T>(nativeHandle);
            }
            catch (InvalidOperationException)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning, "Bullet", "Tried to access disposed native handle.");
                return null;
            }
        }

        public static void FreeNativeHandle(IntPtr nativeHandle)
        {
            if (nativeHandle == IntPtr.Zero) return;
            try
            {
                GCHandle.FromIntPtr(nativeHandle).Free();
            }
            catch (InvalidOperationException)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning, "Bullet", "Tried to dispose already disposed native handle.");
            }
        }
    }
}