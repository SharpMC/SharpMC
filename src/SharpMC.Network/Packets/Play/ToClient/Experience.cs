using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Experience : Packet<Experience>, IToClient
    {
        public byte ClientId => 0x51;

        public float ExperienceBar { get; set; }
        public int Level { get; set; }
        public int TotalExperience { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ExperienceBar = stream.ReadFloat();
            Level = stream.ReadVarInt();
            TotalExperience = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteFloat(ExperienceBar);
            stream.WriteVarInt(Level);
            stream.WriteVarInt(TotalExperience);
        }
    }
}
