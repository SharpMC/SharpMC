using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Login
{
	public class LoginStartPacket : Packet<LoginStartPacket>
	{
		public string Username;
		public override void Decode(MinecraftStream stream)
		{
			Username = stream.ReadString();
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteString(Username);
		}
	}
}
