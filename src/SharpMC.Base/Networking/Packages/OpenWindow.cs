using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	internal class OpenWindow : Package<OpenWindow>
	{
		public byte NumberOfSlots = 0;
		public byte WindowId = 0;
		public string WindowTitle = "";
		public string WindowType = "";

		public OpenWindow(ClientWrapper client) : base(client)
		{
			SendId = 0x2D;
		}

		public OpenWindow(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x2D;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteByte(WindowId);
				Buffer.WriteString(WindowType);
				Buffer.WriteString(WindowTitle);
				Buffer.WriteByte(NumberOfSlots);
				Buffer.FlushData();
			}
		}
	}
}