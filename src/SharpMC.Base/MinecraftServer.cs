using System.Net;
using log4net;
using SharpMC.API;
using SharpMC.Network;
using SharpMC.Network.Packets;
using SharpMC.Util;
using SharpMC.World;

namespace SharpMC
{
	public class MinecraftServer
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (MinecraftServer));

		private NetServer Server { get; }
		public ServerInfo Info { get; }
		public EncryptionHolder RsaEncryption { get; }

		public IPlayerFactory PlayerFactory { get; set; }
		public LevelManager LevelManager { get; }

		public MinecraftServer()
		{
			Log.Info("Initializing...");
			MCPacketFactory.Load();

			Server = new NetServer(new NetConfiguration()
			{
				Host = IPAddress.Any,
				Port = 25565,
				Protocol = ProtocolType.Tcp
			});

			Server.NetConnectionFactory = new ConnectionFactory(this);

			PlayerFactory = new DefaultPlayerFactory(this);

			LevelManager = new LevelManager();

			//Cache the level
			LevelManager.GetLevel(null, "default");

			Info = new ServerInfo(this);

			Log.Info("Generating RSA keypair...");
			//RsaEncryption = null;
			RsaEncryption = new EncryptionHolder();
		}

		public void Start()
		{
			Server.Start();
			Log.Info("Server ready for connections.");
		}

		public void Stop()
		{
			Server.Stop();
		}
	}
}
