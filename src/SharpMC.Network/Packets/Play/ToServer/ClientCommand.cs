using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class ClientCommand : Packet<ClientCommand>, IToServer
    {
        public byte ServerId => 0x04;

        public int ActionId { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ActionId = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ActionId);
        }
    }
}
