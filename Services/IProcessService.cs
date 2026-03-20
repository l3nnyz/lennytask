using ProcessKiller.Models;

namespace ProcessKiller.Services
{
    public interface IProcessService
    {
        List<ProcessInfo> GetAllProcesses();
        ProcessInfo? GetProcessById(int id);
        List<ProcessInfo> GetProcessesByName(string name);
        bool KillProcess(int id, bool force = false);
        bool KillProcessTree(int id, bool force = false);
    }
}
