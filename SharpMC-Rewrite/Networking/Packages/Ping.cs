using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class Ping : Package<Ping>
	{
		public Ping(ClientWrapper client) : base(client)
		{
			ReadId = 0x01;
		}

		public Ping(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x01;
		}

		public override void Read()
		{
			Client.SendData(Buffer.BufferedData);
		}
	}
}
