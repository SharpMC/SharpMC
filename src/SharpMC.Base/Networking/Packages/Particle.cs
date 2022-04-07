using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class Particle : Package<Particle>
	{
		public int[] Data;
		public bool LongDistance = false;
		public float OffsetX = 0f;
		public float OffsetY = 0f;
		public float OffsetZ = 0f;
		public int ParticleCount = 1;
		public float ParticleData = 0f;
		public int ParticleId = 0;
		public float X = 0f;
		public float Y = 0f;
		public float Z = 0f;

		public Particle(ClientWrapper client) : base(client)
		{
			SendId = 0x2A;
		}

		public Particle(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x2A;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteInt(ParticleId);
				Buffer.WriteBool(LongDistance);
				Buffer.WriteFloat(X);
				Buffer.WriteFloat(Y);
				Buffer.WriteFloat(Z);
				Buffer.WriteFloat(OffsetX);
				Buffer.WriteFloat(OffsetY);
				Buffer.WriteFloat(OffsetZ);
				Buffer.WriteFloat(ParticleData);
				Buffer.WriteInt(ParticleCount);
				foreach (var i in Data)
				{
					Buffer.WriteVarInt(i);
				}
				Buffer.FlushData();
			}
		}
	}
}