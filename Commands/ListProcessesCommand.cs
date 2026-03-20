using ProcessKiller.Services;

namespace ProcessKiller.Commands
{
    public class ListProcessesCommand : ICommand
    {
        private readonly IProcessService _processService;
        private readonly string[] _args;

        public ListProcessesCommand(IProcessService processService, string[] args)
        {
            _processService = processService;
            _args = args;
        }

        public bool CanExecute(string[] args)
        {
            return args.Length >= 1 && (args[0].Equals("/list", StringComparison.OrdinalIgnoreCase) || 
                                      args[0].Equals("/l", StringComparison.OrdinalIgnoreCase));
        }

        public int Execute()
        {
            string? filter = null;
            bool showDetails = false;

            for (int i = 1; i < _args.Length; i++)
            {
                switch (_args[i].ToUpper())
                {
                    case "/D":
                    case "/DETAILS":
                        showDetails = true;
                        break;
                    default:
                        if (_args[i].StartsWith("/") || _args[i].StartsWith("-"))
                        {
                            Console.WriteLine($"Paramètre inconnu: {_args[i]}");
                            return 1;
                        }
                        filter = _args[i];
                        break;
                }
            }

            List<ProcessKiller.Models.ProcessInfo> processes;

            if (!string.IsNullOrEmpty(filter))
            {
                if (int.TryParse(filter, out int processId))
                {
                    var process = _processService.GetProcessById(processId);
                    if (process == null)
                    {
                        Console.WriteLine($"Aucun processus trouvé avec l'ID {processId}");
                        return 1;
                    }
                    processes = new List<ProcessKiller.Models.ProcessInfo> { process };
                }
                else
                {
                    processes = _processService.GetProcessesByName(filter);
                    if (!processes.Any())
                    {
                        Console.WriteLine($"Aucun processus trouvé avec le nom '{filter}'");
                        return 1;
                    }
                }
            }
            else
            {
                processes = _processService.GetAllProcesses();
            }

            Console.WriteLine($"Liste des processus ({processes.Count} trouvé(s)):");
            Console.WriteLine(new string('-', 80));

            if (showDetails)
            {
                Console.WriteLine($"{"ID",-8} {"Nom",-25} {"Mémoire (MB)",-15} {"Démarrage",-10} {"Chemin"}");
                Console.WriteLine(new string('-', 80));
                
                foreach (var process in processes.OrderBy(p => p.Id))
                {
                    var memoryMB = process.MemoryUsage / 1024.0 / 1024.0;
                    var startTime = process.StartTime == DateTime.MinValue ? "N/A" : process.StartTime.ToString("HH:mm:ss");
                    var path = process.Path.Length > 30 ? process.Path.Substring(0, 27) + "..." : process.Path;
                    
                    Console.WriteLine($"{process.Id,-8} {process.Name,-25} {memoryMB,-15:F1} {startTime,-10} {path}");
                }
            }
            else
            {
                Console.WriteLine($"{"ID",-8} {"Nom",-30} {"Mémoire (MB)",-15} {"Démarrage",-10}");
                Console.WriteLine(new string('-', 65));
                
                foreach (var process in processes.OrderBy(p => p.Id))
                {
                    var memoryMB = process.MemoryUsage / 1024.0 / 1024.0;
                    var startTime = process.StartTime == DateTime.MinValue ? "N/A" : process.StartTime.ToString("HH:mm:ss");
                    
                    Console.WriteLine($"{process.Id,-8} {process.Name,-30} {memoryMB,-15:F1} {startTime,-10}");
                }
            }

            return 0;
        }
    }
}
