namespace SharpMCRewrite
{
    public class PlayerPositionAndLook : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x06;
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
            double X = buffer.ReadDouble ();
            double FeetY = buffer.ReadDouble ();
            double Z = buffer.ReadDouble ();
            float Yaw = buffer.ReadFloat ();
            float Pitch = buffer.ReadFloat ();
            bool OnGround = buffer.ReadBool ();

            state.Player.OnGround = OnGround;
            state.Player.Yaw = Yaw;
            state.Player.Pitch = Pitch;
            state.Player.Coordinates = new Vector3 (X, FeetY, Z);
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            buffer.WriteVarInt (0x08);
            buffer.WriteDouble (Globals.Level.Generator.GetSpawnPoint ().X);
            buffer.WriteDouble (Globals.Level.Generator.GetSpawnPoint().Y);
            buffer.WriteDouble (Globals.Level.Generator.GetSpawnPoint ().Z);
            buffer.WriteFloat (0f);
            buffer.WriteFloat (0f);
            buffer.WriteByte (111);
            buffer.FlushData ();
        }
    }
}

