namespace SharpMCRewrite
{
    public class SetSlot : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x2F;
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

        }

        /// <summary>
        /// Please parse:
        /// 
        /// The Window to change (byte)
        /// The Slot to set (short)
        /// The Item ID (short) (-1 for empty)
        /// 
        /// The following data only has to be provied if the item id is NOT -1
        /// 
        /// Item count (byte)
        /// Item Damage (short)
        /// NBT Data (byte)
        /// 
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="buffer">Buffer.</param>
        /// <param name="Arguments">Arguments.</param>
        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            buffer.WriteVarInt (PacketID);
            buffer.WriteByte ((byte)Arguments [0]);
            buffer.WriteShort ((short)Arguments [1]);
            buffer.WriteShort ((short)Arguments [2]);
            if ((short)Arguments [2] != -1)
            {
                buffer.WriteByte ((byte)Arguments [3]);
                buffer.WriteShort ((short)Arguments [4]);
                buffer.WriteByte ((byte)Arguments [5]);
            }
            buffer.FlushData ();
        }
    }
}

