using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Handshaking.ToServer
{
    public class SetProtocol : Packet<SetProtocol>, IToServer
    {
        public byte ServerId => 0x00;

        public int ProtocolVersion { get; set; }
        public string ServerHost { get; set; }
        public ushort ServerPort { get; set; }
        public int NextState { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            ProtocolVersion = stream.ReadVarInt();
            ServerHost = stream.ReadString();
            ServerPort = stream.ReadUShort();
            NextState = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(ProtocolVersion);
            stream.WriteString(ServerHost);
            stream.WriteUShort(ServerPort);
            stream.WriteVarInt(NextState);
        }
    }
}