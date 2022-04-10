using System;

namespace SharpMC.Plugins
{
    internal class ConsoleCommandHandler
    {
        public void WaitForCommand()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    Globals.Instance.PluginManager.HandleCommand(input, Globals.Instance.ConsolePlayer);
                }
            }
        }
    }
}