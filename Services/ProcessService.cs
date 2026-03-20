using System.Diagnostics;
using ProcessKiller.Models;

namespace ProcessKiller.Services
{
    public class ProcessService : IProcessService
    {
        public List<ProcessInfo> GetAllProcesses()
        {
            var processes = new List<ProcessInfo>();
            
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    processes.Add(new ProcessInfo
                    {
                        Id = process.Id,
                        Name = process.ProcessName,
                        Path = process.MainModule?.FileName ?? "Unknown",
                        MemoryUsage = process.WorkingSet64,
                        StartTime = process.StartTime
                    });
                }
                catch
                {
                    processes.Add(new ProcessInfo
                    {
                        Id = process.Id,
                        Name = process.ProcessName,
                        Path = "Access Denied",
                        MemoryUsage = process.WorkingSet64,
                        StartTime = DateTime.MinValue
                    });
                }
            }
            
            return processes;
        }

        public ProcessInfo? GetProcessById(int id)
        {
            try
            {
                var process = Process.GetProcessById(id);
                return new ProcessInfo
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    Path = process.MainModule?.FileName ?? "Unknown",
                    MemoryUsage = process.WorkingSet64,
                    StartTime = process.StartTime
                };
            }
            catch
            {
                return null;
            }
        }

        public List<ProcessInfo> GetProcessesByName(string name)
        {
            var processes = new List<ProcessInfo>();
            var allProcesses = Process.GetProcessesByName(name);
            
            foreach (var process in allProcesses)
            {
                try
                {
                    processes.Add(new ProcessInfo
                    {
                        Id = process.Id,
                        Name = process.ProcessName,
                        Path = process.MainModule?.FileName ?? "Unknown",
                        MemoryUsage = process.WorkingSet64,
                        StartTime = process.StartTime
                    });
                }
                catch
                {
                    processes.Add(new ProcessInfo
                    {
                        Id = process.Id,
                        Name = process.ProcessName,
                        Path = "Access Denied",
                        MemoryUsage = process.WorkingSet64,
                        StartTime = DateTime.MinValue
                    });
                }
            }
            
            return processes;
        }

        public bool KillProcess(int id, bool force = false)
        {
            try
            {
                var process = Process.GetProcessById(id);
                
                if (force)
                {
                    process.Kill();
                }
                else
                {
                    process.CloseMainWindow();
                    if (!process.WaitForExit(5000))
                    {
                        process.Kill();
                    }
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool KillProcessTree(int id, bool force = false)
        {
            try
            {
                var parentProcess = Process.GetProcessById(id);
                var children = GetChildProcesses(parentProcess.Id);
                
                foreach (var child in children)
                {
                    KillProcessTree(child.Id, force);
                }
                
                return KillProcess(id, force);
            }
            catch
            {
                return false;
            }
        }

        private List<Process> GetChildProcesses(int parentId)
        {
            var children = new List<Process>();
            
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var handle = CreateToolhelp32Snapshot(0x00000002, 0);
                    if (handle != IntPtr.Zero)
                    {
                        var processEntry = new PROCESSENTRY32 { dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(PROCESSENTRY32)) };
                        
                        if (Process32First(handle, ref processEntry))
                        {
                            do
                            {
                                if (processEntry.th32ParentProcessID == parentId)
                                {
                                    children.Add(Process.GetProcessById((int)processEntry.th32ProcessID));
                                }
                            } while (Process32Next(handle, ref processEntry));
                        }
                        
                        CloseHandle(handle);
                    }
                }
                catch
                {
                    // Ignorer les erreurs d'accès
                }
            }
            
            return children;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool Process32First(IntPtr snapshot, ref PROCESSENTRY32 entry);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool Process32Next(IntPtr snapshot, ref PROCESSENTRY32 entry);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr handle);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public uint th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
    }
}
