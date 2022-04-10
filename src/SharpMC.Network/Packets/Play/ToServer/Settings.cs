using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class Settings : Packet<Settings>, IToServer
    {
        public byte ServerId => 0x05;

        public string Locale { get; set; }
        public sbyte ViewDistance { get; set; }
        public int ChatFlags { get; set; }
        public bool ChatColors { get; set; }
        public byte SkinParts { get; set; }
        public int MainHand { get; set; }
        public bool EnableTextFiltering { get; set; }
        public bool EnableServerListing { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Locale = stream.ReadString();
            ViewDistance = stream.ReadSByte();
            ChatFlags = stream.ReadVarInt();
            ChatColors = stream.ReadBool();
            SkinParts = stream.ReadByte();
            MainHand = stream.ReadVarInt();
            EnableTextFiltering = stream.ReadBool();
            EnableServerListing = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Locale);
            stream.WriteSByte(ViewDistance);
            stream.WriteVarInt(ChatFlags);
            stream.WriteBool(ChatColors);
            stream.WriteByte(SkinParts);
            stream.WriteVarInt(MainHand);
            stream.WriteBool(EnableTextFiltering);
            stream.WriteBool(EnableServerListing);
        }
    }
}
