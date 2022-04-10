using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class ResourcePackReceive : Packet<ResourcePackReceive>, IToServer
    {
        public byte ServerId => 0x21;

        public int Result { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Result = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(Result);
        }
    }
}
