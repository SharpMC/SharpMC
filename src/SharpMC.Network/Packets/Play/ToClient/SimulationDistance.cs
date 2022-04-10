using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class SimulationDistance : Packet<SimulationDistance>, IToClient
    {
        public byte ClientId => 0x57;

        public int Distance { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Distance = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Distance);
        }
    }
}
