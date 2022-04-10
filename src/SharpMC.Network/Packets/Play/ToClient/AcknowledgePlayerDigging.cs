using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class AcknowledgePlayerDigging : Packet<AcknowledgePlayerDigging>, IToClient
    {
        public byte ClientId => 0x08;

        public System.Numerics.Vector3 Location { get; set; }
        public int Block { get; set; }
        public int Status { get; set; }
        public bool Successful { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Location = stream.ReadPosition();
            Block = stream.ReadVarInt();
            Status = stream.ReadVarInt();
            Successful = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WritePosition(Location);
            stream.WriteVarInt(Block);
            stream.WriteVarInt(Status);
            stream.WriteBool(Successful);
        }
    }
}
