using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class SignEditorOpen : Package<SignEditorOpen>
	{
		public Vector3 Coordinates;
		public SignEditorOpen(ClientWrapper client) : base(client)
		{
			SendId = 0x36;
		}

		public SignEditorOpen(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x36;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WritePosition(Coordinates);
				Buffer.FlushData();
			}
		}
	}
}
