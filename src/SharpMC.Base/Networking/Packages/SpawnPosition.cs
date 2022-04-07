using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class SpawnPosition : Package<SpawnPosition>
	{
		public SpawnPosition(ClientWrapper client) : base(client)
		{
			SendId = 0x05;
		}

		public SpawnPosition(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x05;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				var d = Globals.LevelManager.MainLevel.Generator.GetSpawnPoint();
				var data = (((long) d.X & 0x3FFFFFF) << 38) | (((long) d.Y & 0xFFF) << 26) | ((long) d.Z & 0x3FFFFFF);
				Buffer.WriteVarInt(SendId);
				Buffer.WriteLong(data);
				Buffer.FlushData();
			}
		}
	}
}