using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Logging;

#if DEBUG

namespace DecayEngine.SDL2.Desktop
{
    public static class RenderDocDebugger
    {
        public static void WaitForGraphicsDebugger()
        {
            GameEngine.LogAppendLine(LogSeverity.Info, "SDL2",
                $"Waiting for graphics debugger. Current PID: {Process.GetCurrentProcess().Id}.");

            while (true)
            {
                ProcessModule module = Process.GetCurrentProcess().Modules.Cast<ProcessModule>().FirstOrDefault(m =>
                        m.ModuleName == "renderdoc.dll" || m.ModuleName == "librenderdoc.so" || m.ModuleName == "libVkLayer_GLES_RenderDoc.so");

                if (module != null)
                {
                    return;
                }

                Task.Delay(250).Wait();
            }
        }
    }
}

#endif