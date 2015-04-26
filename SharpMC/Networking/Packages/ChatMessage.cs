using SharpMC.Classes;
using SharpMC.ExtentionMethods;

namespace SharpMC.Networking.Packages
{
	public class ChatMessage : Package<ChatMessage>
	{
		public string Message;

		public ChatMessage(ClientWrapper client) : base(client)
		{
			ReadId = 0x01;
			SendId = 0x02;
		}

		public ChatMessage(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x01;
			SendId = 0x02;
		}

		public override void Read()
		{
			Message = Buffer.ReadString();
			Globals.Level.BroadcastChat("<" + Client.Player.Username + "> " +
			                            Message.RemoveLineBreaks().Replace("\\", "\\\\").Replace("\"", "\'\'"));

			ConsoleFunctions.WriteInfoLine("<" + Client.Player.Username + "> " +
			                               Message.RemoveLineBreaks().Replace("\\", "\\\\").Replace("\"", "\'\'"));
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString("{ \"text\": \"" + Message + "\" }");
				Buffer.WriteByte(0);
				Buffer.FlushData();
			}
		}
	}
}