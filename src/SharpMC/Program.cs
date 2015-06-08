using SharpMC.Core;

namespace SharpMC
{
	class Program
	{
		private static SharpMCServer _server;
		static void Main(string[] args)
		{
			_server= new SharpMCServer();
			_server.StartServer();
		}
	}
}
