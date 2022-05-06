using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class GenerateStructure : Packet<GenerateStructure>, IToServer
    {
        public byte ServerId => 0x0e;

        public Vector3 Location { get; set; }
        public int Levels { get; set; }
        public bool KeepJigsaws { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Levels = stream.ReadVarInt();
            KeepJigsaws = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteVarInt(Levels);
            stream.WriteBool(KeepJigsaws);
        }
    }
}
