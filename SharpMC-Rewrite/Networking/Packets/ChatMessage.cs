using System.Text;

namespace SharpMCRewrite
{
    public class ChatMessage : IPacket
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
            string MSG = RemoveSpecialCharacters(buffer.ReadString ());
            Globals.Level.BroadcastChat ("<" + state.Player.Username + "> " + MSG);
        }

        string RemoveSpecialCharacters(string str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            buffer.WriteVarInt (0x02);
            buffer.WriteString ("{ \"text\": \"" + (string)Arguments[0] + "\" }");
            buffer.WriteByte ((byte)0);
            buffer.FlushData ();
        }
    }
}
//string MSG = "{ \"text\": \"Hello world\" }";