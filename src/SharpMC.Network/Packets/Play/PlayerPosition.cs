using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class PlayerPosition : Packet<PlayerPosition>
	{
		public double X;
		public double FeetY;
		public double Z;
		public bool OnGround;

		public override void Decode(MinecraftStream stream)
		{	
			X = stream.ReadDouble();
			FeetY = stream.ReadDouble();
			Z = stream.ReadDouble();
			OnGround = stream.ReadBool();
		}

		public override void Encode(MinecraftStream stream)
		{
			
		}
	}
}
