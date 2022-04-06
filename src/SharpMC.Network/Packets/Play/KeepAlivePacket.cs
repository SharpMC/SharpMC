using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class KeepAlivePacket : Packet<KeepAlivePacket>
	{
		public KeepAlivePacket()
		{
			PacketId = 0x1F; //Clientbound
		}

		public int KeepAliveid;

		public override void Decode(MinecraftStream stream)
		{
			KeepAliveid = stream.ReadVarInt();
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteVarInt(KeepAliveid);
		}
	}
}
