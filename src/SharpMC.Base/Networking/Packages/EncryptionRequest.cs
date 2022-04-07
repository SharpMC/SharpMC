using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking.Packages
{
	public class EncryptionRequest : Package<EncryptionRequest>
	{
		public byte[] PublicKey;
		public string ServerId = "";
		public byte[] VerificationToken;

		public EncryptionRequest(ClientWrapper client) : base(client)
		{
			SendId = 0x01;
		}

		public EncryptionRequest(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x01;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId);
				Buffer.WriteString(ServerId);
				Buffer.WriteVarInt(PublicKey.Length);
				Buffer.Write(PublicKey);
				Buffer.WriteVarInt(VerificationToken.Length);
				Buffer.Write(VerificationToken);
				Buffer.FlushData();
			}
		}
	}
}