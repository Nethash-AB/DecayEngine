using System;
using System.IO;
using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable InconsistentNaming

namespace DecayEngine.Standalone
{
    public static class WindowsCrashHandler
    {
        [DllImport("Dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, IntPtr hFile, int dumpType, IntPtr exceptionParam, IntPtr userStreamParam, IntPtr callbackParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct MinidumpExceptionInformation
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            public int ClientPointers;
        }

        private const int MiniDumpWithFullMemory = 2;

        public static void ExceptionHandler(string minidumpDirPath)
        {
            try
            {
                if (!Directory.Exists(minidumpDirPath))
                {
                    Directory.CreateDirectory(minidumpDirPath);
                }

                string file = Path.Combine(minidumpDirPath, $"crash_{DateTime.Now:dd.MM.yyyy.HH.mm.ss}.dmp");
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    MiniDumpWriteDump(GetCurrentProcess(),
                        GetCurrentProcessId(),
                        fs.SafeFileHandle.DangerousGetHandle(),
                        MiniDumpWithFullMemory,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error writting minidump: {e.Message}");
            }
        }
    }
}