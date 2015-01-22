namespace SharpMCRewrite
{
    public class JoinGame : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x01;
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
            Player p = (Player)Arguments [0];
            buffer.WriteVarInt (PacketID);
            buffer.WriteInt (p.UniqueServerID);
            buffer.WriteByte ((byte)p.Gamemode);
            buffer.WriteByte ((byte)p.Dimension);
            buffer.WriteByte ((byte)Globals.Level.Difficulty);
            buffer.WriteByte ((byte)Globals.MaxPlayers);
            buffer.WriteString (Globals.Level.LevelType.ToString());
            buffer.WriteBool (false);
            buffer.FlushData ();
        }
    }
}

