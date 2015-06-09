using SharpMC.Core;

namespace SharpMC
{
	class Program
	{
		private static SharpMcServer _server;
		static void Main(string[] args)
		{
			_server= new SharpMcServer();
			_server.StartServer();
		}
	}
}
