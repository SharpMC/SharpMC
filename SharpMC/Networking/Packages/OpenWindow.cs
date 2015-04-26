using SharpMC.Classes;

namespace SharpMC.Networking.Packages
{
	internal class OpenWindow : Package<OpenWindow>
	{
		public byte NumberOfSlots;
		public byte WindowId;
		public string WindowTitle;
		public string WindowType;

		public OpenWindow(ClientWrapper client) : base(client)
		{
			SendId = 0x2D;
		}

		public OpenWindow(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
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