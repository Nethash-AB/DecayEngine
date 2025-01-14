using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.ModuleSDK;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.ConstraintSolver
{
    public class SequentialImpulseConstraintSolver : NativeObject
    {
        public SequentialImpulseConstraintSolver()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btSequentialImpulseConstraintSolver_new();
            });
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btConstraintSolver_delete(NativeHandle);
            });
        }
    }
}