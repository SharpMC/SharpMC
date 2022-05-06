using SharpMC.Network.Packets.API;
using System;
using SharpMC.Network.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class Chat : Packet<Chat>, IToClient
    {
        public byte ClientId => 0x0f;

        public string Message { get; set; }
        public sbyte Position { get; set; }
        public Guid Sender { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            Message = stream.ReadString();
            Position = stream.ReadSByte();
            Sender = stream.ReadUuid();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteString(Message);
            stream.WriteSByte(Position);
            stream.WriteUuid(Sender);
        }
    }
}
