using Newtonsoft.Json;
using SharpMC.Core.Utils;
using SharpMC.Enums;

namespace SharpMC.Core.Networking.Packages
{
	public class ChatMessage : Package<ChatMessage>
	{
		public McChatMessage Message;
		public ChatMessageType MessageType = ChatMessageType.ChatBox;

		public ChatMessage(ClientWrapper client) : base(client)
		{
			ReadId = 0x01;
			SendId = 0x02;
		}

		public ChatMessage(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x01;
			SendId = 0x02;
		}

		public override void Read()
		{
			var message = Buffer.ReadString();

			if (message.StartsWith(Globals.ChatHandler.Value.CommandPrefix.ToString()))
			{
				Globals.PluginManager.HandleCommand(message, Client.Player);
				return;
			}

			//string msg = Globals.CleanForJson(Globals.ChatHandler.Value.PrepareMessage(Client.Player, message));
			var msg = Globals.ChatHandler.Value.PrepareMessage(Client.Player, message);

			Globals.BroadcastChat(msg);
			ConsoleFunctions.WriteInfoLine(msg);
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				var message = JsonConvert.SerializeObject(Message);
				
				Buffer.WriteVarInt(SendId);
				//Buffer.WriteString("{ \"text\": \"" + Message + "\" }");
				Buffer.WriteString(message);
				Buffer.WriteByte((byte)MessageType);
				Buffer.FlushData();
			}
		}
	}
}