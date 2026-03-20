using ProcessKiller.Commands;
using ProcessKiller.Services;

namespace ProcessKiller
{
    public class CommandInvoker
    {
        private readonly IProcessService _processService;
        private readonly List<ICommand> _commands;

        public CommandInvoker(IProcessService processService)
        {
            _processService = processService;
            _commands = new List<ICommand>();
        }

        public void RegisterCommand(ICommand command)
        {
            _commands.Add(command);
        }

        public int ExecuteCommand(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            if (args[0].Equals("/help", StringComparison.OrdinalIgnoreCase) || 
                args[0].Equals("/h", StringComparison.OrdinalIgnoreCase) || 
                args[0].Equals("/?", StringComparison.OrdinalIgnoreCase))
            {
                ShowHelp();
                return 0;
            }

            foreach (var command in _commands)
            {
                if (command.CanExecute(args))
                {
                    return command.Execute();
                }
            }

            Console.WriteLine($"Commande inconnue: {args[0]}");
            ShowHelp();
            return 1;
        }

        private void ShowHelp()
        {
            Console.WriteLine("ProcessKiller - Gestionnaire de processus en ligne de commande");
            Console.WriteLine("Version 1.0 - Architecture SOLID avec Command Pattern");
            Console.WriteLine();
            Console.WriteLine("Utilisation:");
            Console.WriteLine("  ProcessKiller.exe [commande] [paramètres]");
            Console.WriteLine();
            Console.WriteLine("Commandes disponibles:");
            Console.WriteLine("  /list | /l [filtre] [/d]     - Lister les processus");
            Console.WriteLine("    filtre: ID du processus ou nom du processus (optionnel)");
            Console.WriteLine("    /d: Afficher les détails complets");
            Console.WriteLine();
            Console.WriteLine("  /kill | /k <cible> [/f] [/t] - Terminer un processus");
            Console.WriteLine("    cible: ID du processus ou nom du processus");
            Console.WriteLine("    /f: Forcer la terminaison");
            Console.WriteLine("    /t: Terminer l'arbre des processus enfants");
            Console.WriteLine();
            Console.WriteLine("  /help | /h | /?              - Afficher cette aide");
            Console.WriteLine();
            Console.WriteLine("Exemples:");
            Console.WriteLine("  ProcessKiller.exe /list");
            Console.WriteLine("  ProcessKiller.exe /list notepad");
            Console.WriteLine("  ProcessKiller.exe /list 1234 /d");
            Console.WriteLine("  ProcessKiller.exe /kill notepad");
            Console.WriteLine("  ProcessKiller.exe /kill 1234 /f");
            Console.WriteLine("  ProcessKiller.exe /kill chrome /t");
            Console.WriteLine();
            Console.WriteLine("Note: Certains processus système peuvent nécessiter des droits d'administrateur.");
        }

        public void InitializeCommands(string[] args)
        {
            _commands.Add(new ListProcessesCommand(_processService, args));
            _commands.Add(new KillProcessCommand(_processService, args));
        }
    }
}
