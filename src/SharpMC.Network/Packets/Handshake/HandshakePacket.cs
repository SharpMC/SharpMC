using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Handshake
{
	public class HandshakePacket : Packet<HandshakePacket>
	{
		public int ProtocolVersion;
		public string ServerAddress;
		public ushort ServerPort;
		public ConnectionState NextState;

		public override void Decode(MinecraftStream stream)
		{
			ProtocolVersion = stream.ReadVarInt();
			ServerAddress = stream.ReadString();
			ServerPort = stream.ReadUShort();
			NextState = (ConnectionState) stream.ReadVarInt();
		}

		public override void Encode(MinecraftStream stream)
		{
			
		}
	}
}
