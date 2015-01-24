using System;
using System.Net;

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
            buffer.WriteVarInt (target.UniqueServerID);
            buffer.WriteUUID (new Guid (target.UUID));
            buffer.WriteInt((int)target.Coordinates.X * 32);
            buffer.WriteInt((int)target.Coordinates.Y * 32);
            buffer.WriteInt((int)target.Coordinates.Z * 32);
            buffer.WriteByte ((byte)target.Yaw);
            buffer.WriteByte ((byte)target.Pitch);
            buffer.WriteShort (0);
            buffer.WriteByte (127);
            buffer.FlushData ();

        }
    }
}

