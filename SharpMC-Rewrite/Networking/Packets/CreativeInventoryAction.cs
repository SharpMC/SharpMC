namespace SharpMCRewrite
{
    public class CreativeInventoryAction : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x10;
            }
        }

        public bool IsPlayePacket
        {
            get
            {
                return true;
            }
        }

        public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            short slot = buffer.ReadShort ();
            ConsoleFunctions.WriteDebugLine ("SLOT: " + slot);
            short itemID = buffer.ReadShort ();
            if (itemID != -1)
            {
                byte ItemCount = (byte)buffer.ReadByte ();
                short ItemDamage = buffer.ReadShort ();
                byte NBT = (byte)buffer.ReadByte (); //We don't do anything with NBT at the moment.
                state.Player.PlayerInventory.SetSlot ((int)slot, itemID, ItemCount, ItemDamage);
            } 
            else
            {
                state.Player.PlayerInventory.SetSlot ((int)slot, itemID, 0, 0);
            }
        }

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {

        }
    }
}

