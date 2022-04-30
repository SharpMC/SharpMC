using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Binary.Special
{
    public class SlotData : IPacket
    {
        public bool Present { get; set; }
        public int? ItemId { get; set; }
        public byte? ItemCount { get; set; }
        public object Optional { get; set; }

        public void Decode(IMinecraftStream stream)
        {
            Present = stream.ReadBool();
            if (!Present)
                return;
            ItemId = stream.ReadVarInt();
            ItemCount = stream.ReadByte();
            Optional = stream.ReadOptNbt();
        }

        public void Encode(IMinecraftStream stream)
        {
            stream.WriteBool(Present);
            if (!Present)
                return;
            stream.WriteVarInt(ItemId.GetValueOrDefault());
            stream.WriteByte(ItemCount.GetValueOrDefault());
            stream.WriteOptNbt(Optional);
        }
    }
}