namespace SharpMCRewrite
{
    public class PlayerListItem : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x38;
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
            Player player = (Player)Arguments [0];
            buffer.WriteVarInt (PacketID);
            buffer.WriteVarInt ((int)Arguments [1]);
            buffer.WriteVarInt (1);
            //foreach(Player player in Globals.Level.OnlinePlayers)
            //{
                buffer.WriteUUID (new System.Guid (player.UUID));
                switch ((int)Arguments [1])
                {
                    case 0:
                        buffer.WriteString (player.Username);
                        buffer.WriteVarInt (0);
                        buffer.WriteVarInt ((byte)player.Gamemode);
                        buffer.WriteVarInt (0);
                        buffer.WriteBool (false);
                        break;
                    case 4:
                        break;
                }
            //}
            buffer.FlushData ();
        }
    }
}

