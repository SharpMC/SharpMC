using System;
using SharpMCRewrite.Networking;
using System.Net;

namespace SharpMCRewrite.Packets
{
    public class Handshake : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x00;
            }
        }

        public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            int Protocol = buffer.ReadVarInt ();
            string Host = buffer.ReadString ();
            short Port = buffer.ReadShort ();
            int State = buffer.ReadVarInt ();

            switch (State)
            {
                case 1:
                    HandleStatusRequest (state);
                    break;
                case 2:
                    HandleLoginRequest (state, buffer);
                    break;
            }
        }

        private void HandleStatusRequest (ClientWrapper state)
        {
            state.MinecraftStream.WriteVarInt (PacketID);
            state.MinecraftStream.WriteString("{\"version\": {\"name\": \"" + Globals.ProtocolName + "\",\"protocol\": " + Globals.ProtocolVersion + "},\"players\": {\"max\": " + Globals.MaxPlayers + ",\"online\": " + Globals.PlayersOnline + "},\"description\": {\"text\":\"" + Globals.ServerMOTD + "\"}}");
            state.MinecraftStream.FlushData();
        }

        private void HandleLoginRequest (ClientWrapper state, MSGBuffer buffer)
        {
            string Username = buffer.ReadUsername ();
            string UUID = getUUID (Username);

            new LoginSuccess().Write(state, new object[] {UUID, Username});
        }

        private string getUUID(string username)
        {
            WebClient wc = new WebClient();
            string result = wc.DownloadString("https://api.mojang.com/users/profiles/minecraft/" + username);
            string[] _result = result.Split('"');
            if (_result.Length > 1)
            {
                string UUID = _result [3];
                return new Guid(UUID).ToString();
            } 
            else
            {
                return "";
            }
        }

        public void Write(ClientWrapper state, object[] Arguments)
        {

        }
    }
}

