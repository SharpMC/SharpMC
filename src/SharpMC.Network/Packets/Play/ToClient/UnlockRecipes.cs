using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UnlockRecipes : Packet<UnlockRecipes>, IToClient
    {
        public byte ClientId => 0x39;

        public int Action { get; set; }
        public bool CraftingBookOpen { get; set; }
        public bool FilteringCraftable { get; set; }
        public bool SmeltingBookOpen { get; set; }
        public bool FilteringSmeltable { get; set; }
        public bool BlastFurnaceOpen { get; set; }
        public bool FilteringBlastFurnace { get; set; }
        public bool SmokerBookOpen { get; set; }
        public bool FilteringSmoker { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Action = stream.ReadVarInt();
            CraftingBookOpen = stream.ReadBool();
            FilteringCraftable = stream.ReadBool();
            SmeltingBookOpen = stream.ReadBool();
            FilteringSmeltable = stream.ReadBool();
            BlastFurnaceOpen = stream.ReadBool();
            FilteringBlastFurnace = stream.ReadBool();
            SmokerBookOpen = stream.ReadBool();
            FilteringSmoker = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Action);
            stream.WriteBool(CraftingBookOpen);
            stream.WriteBool(FilteringCraftable);
            stream.WriteBool(SmeltingBookOpen);
            stream.WriteBool(FilteringSmeltable);
            stream.WriteBool(BlastFurnaceOpen);
            stream.WriteBool(FilteringBlastFurnace);
            stream.WriteBool(SmokerBookOpen);
            stream.WriteBool(FilteringSmoker);
        }
    }
}
