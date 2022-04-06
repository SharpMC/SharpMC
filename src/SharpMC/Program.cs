using SharpCore;
using SharpMC.Core;

namespace SharpMC
{
	internal static class Program
	{
		private static SharpMcServer _server;

		private static void Main(string[] args)
		{
			// ReSharper disable ObjectCreationAsStatement
			new Main();
			// ReSharper restore ObjectCreationAsStatement

			_server = new SharpMcServer();
			_server.StartServer();
		}
	}
}