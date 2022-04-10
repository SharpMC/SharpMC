using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class TradeList : Packet<TradeList>, IToClient
    {
        public byte ClientId => 0x28;

        public int WindowId { get; set; }
        public int VillagerLevel { get; set; }
        public int Experience { get; set; }
        public bool IsRegularVillager { get; set; }
        public bool CanRestock { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadVarInt();
            VillagerLevel = stream.ReadVarInt();
            Experience = stream.ReadVarInt();
            IsRegularVillager = stream.ReadBool();
            CanRestock = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(WindowId);
            stream.WriteVarInt(VillagerLevel);
            stream.WriteVarInt(Experience);
            stream.WriteBool(IsRegularVillager);
            stream.WriteBool(CanRestock);
        }
    }
}
