using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class JoinGamePacket : Packet<JoinGamePacket>
	{
		public JoinGamePacket()
		{
			PacketId = 0x23;
		}

		public int EntityId;
		public byte Gamemode;
		public int Dimension;
		public byte Difficulty;
		//public byte MaxPlayers;
		public string LevelType;
		public bool ReducedDebugInfo;

		public override void Decode(MinecraftStream stream)
		{
			EntityId = stream.ReadInt();
			Gamemode = (byte) stream.ReadByte();
			Dimension = stream.ReadInt();
			Difficulty = (byte) stream.ReadByte();
			stream.ReadByte();
			LevelType = stream.ReadString();
			ReducedDebugInfo = stream.ReadBool();
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteInt(EntityId);
			stream.WriteByte(Gamemode);
			stream.WriteInt(Dimension);
			stream.WriteByte(Difficulty);
			stream.WriteByte(255);
			stream.WriteString(LevelType);
			stream.WriteBool(ReducedDebugInfo);
		}
	}
}
