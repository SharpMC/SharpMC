using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets
{
	public abstract class Packet : IPacket<IMinecraftStream>
	{
		public int PacketId { get; set; } = -1;

		public abstract void Decode(IMinecraftStream stream);

		public abstract void Encode(IMinecraftStream stream);
	}

	public abstract class Packet<TPacket> : Packet where TPacket : Packet<TPacket>, new()
	{
		public static TPacket CreateObject()
		{
			return new TPacket();
		}
	}
}