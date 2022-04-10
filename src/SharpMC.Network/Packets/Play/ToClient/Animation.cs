using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Animation : Packet<Animation>, IToClient
    {
        public byte ClientId => 0x06;

        public int EntityId { get; set; }
        public byte _Animation { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            _Animation = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteByte(_Animation);
        }
    }
}
