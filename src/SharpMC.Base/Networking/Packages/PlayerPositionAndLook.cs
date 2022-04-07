using SharpMC.Core.Utils;
using SharpMC.Util;

namespace SharpMC.Core.Networking.Packages
{
	internal class PlayerPositionAndLook : Package<PlayerPositionAndLook>
	{
		public double X = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().X;
		public double Y = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().Y;
		public double Z = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint().Z;
		public float Yaw = 0f;
		public float Pitch = 0f;

		public PlayerPositionAndLook(ClientWrapper client) : base(client)
		{
			SendId = 0x08;
			ReadId = 0x06;
		}

		public PlayerPositionAndLook(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x08;
			ReadId = 0x06;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteDouble(X);
				Buffer.WriteDouble(Y);
				Buffer.WriteDouble(Z);
				Buffer.WriteFloat(Yaw);
				Buffer.WriteFloat(Pitch);
				Buffer.WriteByte(111);
				Buffer.FlushData();
			}
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var x = Buffer.ReadDouble();
				var feetY = Buffer.ReadDouble();
				var z = Buffer.ReadDouble();
				var yaw = Buffer.ReadFloat();
				var pitch = Buffer.ReadFloat();
				var onGround = Buffer.ReadBool();

				Client.Player.PositionChanged(
                    Vectors.Create(x, feetY, z), yaw, pitch, onGround);
			}
		}
	}
}