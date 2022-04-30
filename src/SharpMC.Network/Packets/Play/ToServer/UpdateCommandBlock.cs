using SharpMC.Network.API;
using System.Numerics;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UpdateCommandBlock : Packet<UpdateCommandBlock>, IToServer
    {
        public byte ServerId => 0x26;

        public Vector3 Location { get; set; }
        public string Command { get; set; }
        public int Mode { get; set; }
        public byte Flags { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Command = stream.ReadString();
            Mode = stream.ReadVarInt();
            Flags = stream.ReadByte();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteString(Command);
            stream.WriteVarInt(Mode);
            stream.WriteByte(Flags);
        }
    }
}
