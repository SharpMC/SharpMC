using System;
using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play
{
	public class PlayerListItemPacket : Packet<PlayerListItemPacket>
	{
		public PlayerListAction Action;
		public Guid UUID;
		public string Name;
		public int Gamemode;
		public int Ping;
		public string Displayname = null;
		public PlayerListProperty[] Properties = null;

		public PlayerListItemPacket()
		{
			PacketId = 0x2D;
		}

		public override void Decode(MinecraftStream stream)
		{
			
		}

		public override void Encode(MinecraftStream stream)
		{
			stream.WriteVarInt((int) Action);
			stream.WriteVarInt(1);
			stream.WriteUuid(UUID);
			switch (Action)
			{
				case PlayerListAction.AddPlayer:
					stream.WriteString(Name);
					if (Properties == null)
					{
						stream.WriteVarInt(0);
					}
					else
					{
						stream.WriteVarInt(Properties.Length);
						foreach (var property in Properties)
						{
							stream.WriteString(property.Name);
							stream.WriteString(property.Value);
							stream.WriteBool(property.IsSigned);
							if (property.IsSigned)
							{
								stream.WriteString(property.Signature);
							}
						}
					}
					stream.WriteVarInt(Gamemode);
					stream.WriteVarInt(Ping);
					bool hasDisplayName = !string.IsNullOrEmpty(Displayname);
					stream.WriteBool(hasDisplayName);
					if (hasDisplayName)
					{
						stream.WriteString(Displayname);
					}
					break;
				case PlayerListAction.UpdateGamemode:
					stream.WriteVarInt(Gamemode);
					break;
				case PlayerListAction.UpdateLatency:
					stream.WriteVarInt(Ping);
					break;
				case PlayerListAction.UpdateDisplayName:
					bool hdn = !string.IsNullOrEmpty(Displayname);
					stream.WriteBool(hdn);
					if (hdn)
					{
						stream.WriteString(Displayname);
					}
					break;
				case PlayerListAction.RemovePlayer:
					break;
			}
		}
	}

	public enum PlayerListAction : int
	{
		AddPlayer = 0,
		UpdateGamemode = 1,
		UpdateLatency = 2,
		UpdateDisplayName = 3,
		RemovePlayer = 4
	}

	public sealed class PlayerListProperty
	{
		public string Name;
		public string Value;
		public bool IsSigned;
		public string Signature;
	}
}
