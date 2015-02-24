using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class SpawnPosition : Package<SpawnPosition>
	{
		public SpawnPosition(ClientWrapper client) : base(client)
		{
			SendId = 0x05;
		}

		public SpawnPosition(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x05;
		}

		public override void Write()
		{
			Vector3 D = Globals.Level.Generator.GetSpawnPoint();
			long Data = (((long)D.X & 0x3FFFFFF) << 38) | (((long)D.Y & 0xFFF) << 26) | ((long)D.Z & 0x3FFFFFF);
			Buffer.WriteVarInt(SendId);
			Buffer.WriteLong(Data);
			Buffer.FlushData();
		}
	}
}
