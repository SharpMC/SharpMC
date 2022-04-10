using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class UpdateCommandBlockMinecart : Packet<UpdateCommandBlockMinecart>, IToServer
    {
        public byte ServerId => 0x27;

        public int EntityId { get; set; }
        public string Command { get; set; }
        public bool TrackOutput { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            Command = stream.ReadString();
            TrackOutput = stream.ReadBool();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteString(Command);
            stream.WriteBool(TrackOutput);
        }
    }
}
