using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SharpMCRewrite
{
    public class MSGBuffer
    {
        private ClientWrapper _Client;
        public int Size = 0;
        private int LastByte = 0;
        public byte[] BufferedData = new byte[4096];

        public MSGBuffer (ClientWrapper client)
        {
            _Client = client;
        }

        private int ReadByte()
        {
            byte returnData = BufferedData [LastByte];
            LastByte++;
            return returnData;
        }

        private byte[] Read(int Length)
        {
            byte[] Buffered = new byte[Length];
            Array.Copy (BufferedData, LastByte, Buffered, 0, Length);
            LastByte += Length;
            return Buffered;
        }

        #region Reader
        public int ReadVarInt()
        {
            int value = 0;
            int size = 0;
            int b;
            while (((b = ReadByte()) & 0x80) == 0x80) {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5) {
                    throw new IOException("VarInt too long. Hehe that's punny.");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        public long ReadVarLong()
        {
            int value = 0;
            int size = 0;
            int b;
            while (((b = ReadByte()) & 0x80) == 0x80) {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 10) {
                    throw new IOException("VarLong too long. That's what she said.");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        public short ReadShort()
        {
            int o = ReadByte ();
            int i = ReadByte ();

            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(new byte[2] {(byte)i , (byte)o }, 0);
            else
                return BitConverter.ToInt16(new byte[2] {(byte)o , (byte)i }, 0);

        }

        public string ReadString()
        {
            int Length = ReadVarInt ();
            byte[] StringValue = Read (Length);
            return Encoding.UTF8.GetString (StringValue);
        }

        /// <summary>
        /// Reads the username. (We cannot just use ReadString() because of some bug)...
        /// Idk what happend, but it seems to send an extra Short for the username there...
        /// Also, worst solution there is xD
        /// </summary>
        /// <returns>The username.</returns>
        public string ReadUsername()
        {
            byte[] NoEdit = Encoding.UTF8.GetBytes(ReadString ());
            List<byte> t = new List<byte> ();

            int D = 0;
            foreach (byte i in NoEdit)
            {
                if (D > 1)
                {
                    t.Add (i);
                }
                D++;
            }
            return Encoding.UTF8.GetString(t.ToArray());
        }
        #endregion
    }
}

