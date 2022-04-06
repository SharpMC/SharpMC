using SharpCore;
using SharpMC.Core;
using TestPlugin;

namespace SharpMC
{
	internal static class Program
	{
		private static SharpMcServer _server;

		private static void Main(string[] args)
		{
			// ReSharper disable ObjectCreationAsStatement
			new Main();
            new Test();
			// ReSharper restore ObjectCreationAsStatement

			_server = new SharpMcServer();
			_server.StartServer();
		}
	}
}