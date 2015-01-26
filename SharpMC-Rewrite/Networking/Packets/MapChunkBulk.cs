using SharpMCRewrite.Worlds;

namespace SharpMCRewrite
{
    public class MapChunkBulk : IPacket
    {
        public int PacketID
        {
            get
            {
                return 0x26;
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

        public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
        {
			ChunkColumn[] chunks = (ChunkColumn[])Arguments [0];

            buffer.WriteVarInt (PacketID);
            buffer.WriteBool (true);
			buffer.WriteVarInt (chunks.Length);
			foreach (ChunkColumn i in chunks)
			{
				buffer.Write(i.GetChunkMeta());
			}
			foreach (ChunkColumn i in chunks)
			{
				buffer.Write(i.GetChunkData());
			}
            buffer.FlushData ();
        }
    }
}

