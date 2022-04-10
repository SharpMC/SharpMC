using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class BlockAction : Packet<BlockAction>, IToClient
    {
        public byte ClientId => 0x0b;

        public System.Numerics.Vector3 Location { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public int BlockId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Byte1 = stream.ReadByte();
            Byte2 = stream.ReadByte();
            BlockId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteByte(Byte1);
            stream.WriteByte(Byte2);
            stream.WriteVarInt(BlockId);
        }
    }
}
