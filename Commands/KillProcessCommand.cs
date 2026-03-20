using ProcessKiller.Services;

namespace ProcessKiller.Commands
{
    public class KillProcessCommand : ICommand
    {
        private readonly IProcessService _processService;
        private readonly string[] _args;

        public KillProcessCommand(IProcessService processService, string[] args)
        {
            _processService = processService;
            _args = args;
        }

        public bool CanExecute(string[] args)
        {
            return args.Length >= 2 && (args[0].Equals("/kill", StringComparison.OrdinalIgnoreCase) || 
                                      args[0].Equals("/k", StringComparison.OrdinalIgnoreCase));
        }

        public int Execute()
        {
            bool force = false;
            bool tree = false;
            string? target = null;

            for (int i = 1; i < _args.Length; i++)
            {
                switch (_args[i].ToUpper())
                {
                    case "/F":
                        force = true;
                        break;
                    case "/T":
                        tree = true;
                        break;
                    default:
                        if (_args[i].StartsWith("/") || _args[i].StartsWith("-"))
                        {
                            Console.WriteLine($"Paramètre inconnu: {_args[i]}");
                            return 1;
                        }
                        target = _args[i];
                        break;
                }
            }

            if (string.IsNullOrEmpty(target))
            {
                Console.WriteLine("Erreur: Aucun processus spécifié. Utilisez /kill <ID ou nom> [/F] [/T]");
                return 1;
            }

            int successCount = 0;
            int failCount = 0;

            if (int.TryParse(target, out int processId))
            {
                var process = _processService.GetProcessById(processId);
                if (process == null)
                {
                    Console.WriteLine($"Erreur: Processus avec ID {processId} non trouvé.");
                    return 1;
                }

                Console.WriteLine($"Processus trouvé: {process.Name} (ID: {process.Id})");
                
                bool success;
                if (tree)
                {
                    Console.WriteLine($"Tentative de terminaison de l'arbre du processus {processId}...");
                    success = _processService.KillProcessTree(processId, force);
                }
                else
                {
                    Console.WriteLine($"Tentative de terminaison du processus {processId} {(force ? "avec force" : "normalement")}...");
                    success = _processService.KillProcess(processId, force);
                }

                if (success)
                {
                    Console.WriteLine($"Processus {processId} terminé avec succès.");
                    successCount++;
                }
                else
                {
                    Console.WriteLine($"Échec de la terminaison du processus {processId}.");
                    failCount++;
                }
            }
            else
            {
                var processes = _processService.GetProcessesByName(target);
                if (!processes.Any())
                {
                    Console.WriteLine($"Erreur: Aucun processus trouvé avec le nom '{target}'.");
                    return 1;
                }

                Console.WriteLine($"{processes.Count} processus trouvés avec le nom '{target}':");
                foreach (var process in processes)
                {
                    Console.WriteLine($"  - {process}");
                }

                foreach (var process in processes)
                {
                    bool success;
                    if (tree)
                    {
                        Console.WriteLine($"Tentative de terminaison de l'arbre du processus {process.Id}...");
                        success = _processService.KillProcessTree(process.Id, force);
                    }
                    else
                    {
                        Console.WriteLine($"Tentative de terminaison du processus {process.Id} {(force ? "avec force" : "normalement")}...");
                        success = _processService.KillProcess(process.Id, force);
                    }

                    if (success)
                    {
                        Console.WriteLine($"Processus {process.Id} terminé avec succès.");
                        successCount++;
                    }
                    else
                    {
                        Console.WriteLine($"Échec de la terminaison du processus {process.Id}.");
                        failCount++;
                    }
                }
            }

            Console.WriteLine($"\nTerminé: {successCount} processus terminés, {failCount} échecs.");
            return failCount > 0 ? 1 : 0;
        }
    }
}
