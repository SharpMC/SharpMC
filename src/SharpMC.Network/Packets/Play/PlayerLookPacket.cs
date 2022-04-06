using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class PlayerLookPacket : Packet<PlayerLookPacket>
	{
		public float Yaw;
		public float Pitch;
		public bool OnGround;

		public override void Decode(MinecraftStream stream)
		{
			Yaw = stream.ReadFloat();
			Pitch = stream.ReadFloat();
			OnGround = stream.ReadBool();
		}

		public override void Encode(MinecraftStream stream)
		{
			
		}
	}
}
