using System;
using System.IO;
using SharpMC.Generator.Pakkit;
using SharpMC.Generator.Prismarine;
using SharpMC.Generator.Prismarine.Data;

namespace SharpMC.Generator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {            
            if (args is not {Length: 3})
            {
                var ns = typeof(Program).Namespace;
                Console.WriteLine($"Usage: {ns} [mode] [source] [target]");
                return;
            }
            var mode = args[0][0];
            var source = Path.GetFullPath(args[1]);
            var target = Path.GetFullPath(args[2]);
            Console.WriteLine($"Mode   => {mode}");
            Console.WriteLine($"Source => {source}");
            Console.WriteLine($"Target => {target}");
            if (mode == 'j')
            {
                PrismarineReader.Start(source, target);
            }
            else if (mode == 'd')
            {
                DataPriReader.Start(source, target);
            }
            else if (mode == 'p')
            {
                PakkitReader.Start(source, target);
            }
            Console.WriteLine("Done.");
        }
    }
}