using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class UpdateHealth : Packet<UpdateHealth>, IToClient
    {
        public byte ClientId => 0x52;

        public float Health { get; set; }
        public int Food { get; set; }
        public float FoodSaturation { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Health = stream.ReadFloat();
            Food = stream.ReadVarInt();
            FoodSaturation = stream.ReadFloat();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteFloat(Health);
            stream.WriteVarInt(Food);
            stream.WriteFloat(FoodSaturation);
        }
    }
}
