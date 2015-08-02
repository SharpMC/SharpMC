using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class UpdateSign : Package<UpdateSign>
	{
		public Vector3 SignCoordinates;
		public string Line1 = "Line 1";
		public string Line2 = "Line 2";
		public string Line3 = "Line 3";
		public string Line4 = "Line 4";

		public UpdateSign(ClientWrapper client) : base(client)
		{
			SendId = 0x33;
			ReadId = 0x13;
		}

		public UpdateSign(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x33;
			ReadId = 0x13;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WritePosition(SignCoordinates);
				Buffer.WriteString(Line1);
				Buffer.WriteString(Line2);
				Buffer.WriteString(Line3);
				Buffer.WriteString(Line4);
				Buffer.FlushData();
			}
		}

		public override void Read()
		{
			if (Buffer != null)
			{
				var coords = Buffer.ReadPosition();
				var line1 = Buffer.ReadString();
				var line2 = Buffer.ReadString();
				var line3 = Buffer.ReadString();
				var line4 = Buffer.ReadString();

				Client.Player.Level.UpdateSign(coords, new string[] {line1, line2, line3, line4});
			}
		}
	}
}
