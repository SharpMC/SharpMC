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

            double XMove = targetplayer.Coordinates.X - X;
            double YMove = targetplayer.Coordinates.Y - FeetY;
            double ZMove = targetplayer.Coordinates.Z - Z;

            targetplayer.Coordinates = new Vector3 (X, FeetY, Z);
            state.Player.OnGround = OnGround;
            state.Player.SendChunksFromPosition ();
            Globals.Level.BroadcastPacket (new EntityTeleport (), new object[] {state.Player});
           // Globals.Level.BroadcastPacket (new EntityRelativeMove (), new object[] {state.Player, XMove, YMove, ZMove, OnGround});
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

