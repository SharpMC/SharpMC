using System;
using System.Collections.Generic;
using SharpMC.Network.API;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Util
{
	public class PacketFactory<TType, TStream, TPacket> 
        where TType : IComparable<TType> 
        where TStream : IMinecraftStream
        where TPacket : IPacket<TStream>
	{
		private Dictionary<Type, TType> IdMap { get; }
 		private Dictionary<TType, Func<TPacket>> Packets { get; }

		private readonly object _addLock = new();

		public PacketFactory()
		{
			IdMap = new Dictionary<Type, TType>();
			Packets = new Dictionary<TType, Func<TPacket>>();
		}

		public void Register(TType packetId, Func<TPacket> createPacket)
		{
			lock (_addLock)
			{
				if (Packets.ContainsKey(packetId))
				{
					throw new DuplicatePacketIdException<TType>(packetId);
				}

				Packets.Add(packetId, createPacket);
				IdMap.Add(createPacket().GetType(), packetId);
			}
		}

		public bool TryGetPacket(TType packetId, out TPacket packet) 
		{
			if (!Packets.TryGetValue(packetId, out var p))
			{
				packet = default;
				return false;
			}
			packet = p();
			return true;
		}

		public bool TryGetPacketId(Type type, out TType id)
		{
			if (IdMap.TryGetValue(type, out id))
			{
				return true;
			}
			id = default;
			return false;
		}

		public bool TryGetPacket<TPacketType>(out TPacketType packet) where TPacketType : TPacket
		{
			if (TryGetPacketId(typeof(TPacketType), out var id))
			{
				packet = (TPacketType) Packets[id]();
				return true;
			}
			packet = default;
			return false;
		}
	}
}