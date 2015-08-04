// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using Newtonsoft.Json;
using SharpMC.Core.Enums;
using SharpMC.Core.Utils;

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
				string message = JsonConvert.SerializeObject(Message);
				
				Buffer.WriteVarInt(SendId);
				//Buffer.WriteString("{ \"text\": \"" + Message + "\" }");
				Buffer.WriteString(message);
				Buffer.WriteByte((byte)MessageType);
				Buffer.FlushData();
			}
		}
	}
}