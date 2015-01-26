namespace SharpMCRewrite
{
    public class EntityLook : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x16;
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
        /// Argument 0: Entity ID
        /// Argument 1: Yaw
        /// Argument 2: Pitch
        /// Argument 3: OnGround
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="buffer">Buffer.</param>
        /// <param name="Arguments">Arguments.</param>
        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
            if ((int)Arguments [0] != state.Player.UniqueServerID)
            {
                buffer.WriteVarInt (PacketID);
                buffer.WriteVarInt ((int)Arguments [0]);
                buffer.WriteByte ((byte)Arguments [1]);
                buffer.WriteByte ((byte)Arguments [2]);
                buffer.WriteBool ((bool)Arguments [3]);
                buffer.FlushData ();
            }
        }
    }
}

