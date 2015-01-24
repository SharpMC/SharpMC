namespace SharpMCRewrite
{
    public class EntityTeleport : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x18;
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
            if (state.Player != target)
            {
                buffer.WriteVarInt (PacketID);
                buffer.WriteVarInt (target.UniqueServerID);
                buffer.WriteInt ((int)target.Coordinates.X * 32);
                buffer.WriteInt ((int)target.Coordinates.Y * 32);
                buffer.WriteInt ((int)target.Coordinates.Z * 32);
                buffer.WriteByte ((byte)target.Yaw);
                buffer.WriteByte ((byte)target.Pitch);
                buffer.WriteBool (target.OnGround);
                buffer.FlushData ();
            }
        }
    }
}

