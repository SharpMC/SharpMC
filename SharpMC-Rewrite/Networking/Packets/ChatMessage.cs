using System.Text;
using System.Text.RegularExpressions;
using System;

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
            string MSG = buffer.ReadString ();
            Globals.Level.BroadcastChat ("<" + state.Player.Username + "> " + MSG.RemoveLineBreaks().Replace("\\","\\\\"));
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