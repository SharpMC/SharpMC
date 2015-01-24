using System;

namespace SharpMCRewrite
{
    public class SpawnPlayer : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x0C;
            }
        }

        public bool IsPlayePacket
        {
            get
            {
                return true;
            }
        }

        public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            Player target = (Player)Arguments [0];

            buffer.WriteVarInt (PacketID);
            buffer.Write (new Guid (target.UUID).ToByteArray());
            buffer.WriteInt((int)target.Coordinates.X * 32);
            buffer.WriteInt((int)target.Coordinates.Y * 32);
            buffer.WriteInt((int)target.Coordinates.Z * 32);
            buffer.WriteByte ((byte)target.Yaw);
            buffer.WriteByte ((byte)target.Pitch);
            buffer.WriteShort (0);
            buffer.WriteByte ((0 << 5 | 0 & 0x1F) & 0xFF);
            buffer.FlushData ();
        }
    }
}

