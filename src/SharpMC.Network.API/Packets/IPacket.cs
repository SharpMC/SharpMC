using SharpMC.Network.API;

namespace SharpMC.Network.Packets.API
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