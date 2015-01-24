namespace SharpMCRewrite
{
    public class PlayerPosition : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x04;
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
            Player targetplayer = state.Player;
            double X = buffer.ReadDouble ();
            double FeetY = buffer.ReadDouble ();
            double Z = buffer.ReadDouble ();
            bool OnGround = buffer.ReadBool ();
            targetplayer.Coordinates = new Vector3 (X, FeetY, Z);
            state.Player.OnGround = OnGround;
            state.Player.SendChunksFromPosition ();
            Globals.Level.BroadcastPacket (new EntityTeleport (), new object[] {state.Player});
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

