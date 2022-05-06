using SharpMC.Network.Packets.API;
using System;
using SharpMC.Network.API;

namespace SharpMC.Network.Packets.Login.ToClient
{
    public class Success : Packet<Success>, IToClient
    {
        public byte ClientId => 0x02;

        public Guid Uuid { get; set; }
        public string Username { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Uuid = stream.ReadUuid();
            Username = stream.ReadString();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteUuid(Uuid);
            stream.WriteString(Username);
        }
    }
}
