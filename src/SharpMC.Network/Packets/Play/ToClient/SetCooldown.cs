using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SetCooldown : Packet<SetCooldown>, IToClient
    {
        public byte ClientId => 0x17;

        public int ItemID { get; set; }
        public int CooldownTicks { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ItemID = stream.ReadVarInt();
            CooldownTicks = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ItemID);
            stream.WriteVarInt(CooldownTicks);
        }
    }
}
