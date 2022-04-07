using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class LoginSucces : Package<LoginSucces>
	{
		public string Username;
		public string Uuid;

		public LoginSucces(ClientWrapper client) : base(client)
		{
			ReadId = 0x02;
			SendId = 0x02;
		}

		public LoginSucces(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			ReadId = 0x02;
			SendId = 0x02;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString(Uuid);
				Buffer.WriteString(Username);
				Buffer.FlushData();
			}
		}
	}
}