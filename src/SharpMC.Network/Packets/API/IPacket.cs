using SharpMC.Network.Util;

namespace SharpMC.Network.Packets
{
	public interface IPacket : IPacket<IMinecraftStream>
	{
	}

	public interface IPacket<in TStream> where TStream : IMinecraftStream
	{
		void Encode(TStream stream);

		void Decode(TStream stream);
	}
}