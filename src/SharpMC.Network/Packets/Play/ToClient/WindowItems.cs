﻿using SharpMC.Network.API;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets.API;

namespace SharpMC.Network.Packets.Play.ToClient
{
    public class WindowItems : Packet<WindowItems>, IToClient
    {
        public byte ClientId => 0x14;

        public byte WindowId { get; set; }
        public int StateId { get; set; }
        public SlotData CarriedItem { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            WindowId = stream.ReadByte();
            StateId = stream.ReadVarInt();
            CarriedItem = (SlotData) stream.ReadSlot();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteByte(WindowId);
            stream.WriteVarInt(StateId);
            stream.WriteSlot(CarriedItem);
        }
    }
}
