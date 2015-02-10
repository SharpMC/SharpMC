using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class Disconnect : Package<Disconnect>
	{
		public string Reason = "";
		public Disconnect(ClientWrapper client) : base(client)
		{
			SendId = 0x40;
		}

		public Disconnect(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x40;
		}

		public override void Write()
		{
			Buffer.WriteVarInt(SendId);
			Buffer.WriteString("{ \"text\": \"" + Reason + "\" }");
			Buffer.FlushData();
		}
	}
}
