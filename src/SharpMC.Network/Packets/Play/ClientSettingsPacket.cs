using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class ClientSettingsPacket : Packet<ClientSettingsPacket>
	{
		public string Locale;
		public byte ViewDistance;
		public int ChatMode;
		public bool ChatColors;
		public byte SkinParts;
		public int MainHand;

		public override void Decode(MinecraftStream stream)
		{
			Locale = stream.ReadString();
			ViewDistance = (byte) stream.ReadByte();
			ChatMode = stream.ReadVarInt();
			ChatColors = stream.ReadBool();
			SkinParts = (byte) stream.ReadByte();
			MainHand = stream.ReadVarInt();
		}

		public override void Encode(MinecraftStream stream)
		{
        }
	}
}