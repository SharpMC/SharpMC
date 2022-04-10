using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Map : Packet<Map>, IToClient
    {
        public byte ClientId => 0x27;

        public int ItemDamage { get; set; }
        public sbyte Scale { get; set; }
        public bool Locked { get; set; }
        public byte Columns { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ItemDamage = stream.ReadVarInt();
            Scale = stream.ReadSByte();
            Locked = stream.ReadBool();
            Columns = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ItemDamage);
            stream.WriteSByte(Scale);
            stream.WriteBool(Locked);
            stream.WriteByte(Columns);
        }
    }
}
