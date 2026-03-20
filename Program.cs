using ProcessKiller.Services;

namespace ProcessKiller
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var processService = new ProcessService();
                var invoker = new CommandInvoker(processService);
                
                invoker.InitializeCommands(args);
                return invoker.ExecuteCommand(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur critique: {ex.Message}");
                return 1;
            }
        }
    }
}
