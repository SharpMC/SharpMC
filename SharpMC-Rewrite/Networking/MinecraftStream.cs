using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace SharpMCRewrite.Networking
{

    public class MinecraftStream : NetworkStream
    {

        public MinecraftStream(Socket i) : base(i)
        {

        }

        #region Writer
        public void WriteVarInt(int Integer)
        {
            while ((Integer & -128) != 0)
            {
                base.WriteByte((byte)(Integer & 127 | 128));
                Integer = (int)(((uint)Integer) >> 7);
            }
            base.WriteByte((byte)Integer);
        }

        public void WriteVarLong(long i)
        {
            long Fuck = i;
            while ((Fuck & ~0x7F) != 0) {
                base.WriteByte((byte)((Fuck & 0x7F) | 0x80));
                Fuck >>= 7;
            }
            base.WriteByte((byte)Fuck);
        }

        public void WriteInt(int Data)
        {
            byte[] Buffer = BitConverter.GetBytes (Data);
            base.Write (Buffer, 0, Buffer.Length);
        }

        public void WriteString(string Data)
        {
            byte[] StringData = Encoding.UTF8.GetBytes (Data);
            WriteVarInt (StringData.Length);
            base.Write (StringData, 0, StringData.Length);
        }

        public void WriteShort(short Data)
        {
            byte[] ShortData = BitConverter.GetBytes (Data);
            base.Write (ShortData, 0, ShortData.Length);
        }
        #endregion

        #region Reader
        public int ReadVarInt()
        {
            int value = 0;
            int size = 0;
            int b;
            while (((b = base.ReadByte()) & 0x80) == 0x80) {
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
            while (((b = base.ReadByte()) & 0x80) == 0x80) {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 10) {
                    throw new IOException("VarLong too long. That's what she said.");
                }
            }
            return value | ((b & 0x7F) << (size * 7));
        }

        public short ReadShort()
        {
            int o = base.ReadByte ();
            int i = base.ReadByte ();

            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt16(new byte[2] {(byte)i , (byte)o }, 0);
            else
                return BitConverter.ToInt16(new byte[2] {(byte)o , (byte)i }, 0);

        }

        public string ReadString()
        {
            int Length = ReadVarInt ();
            byte[] Bufffer = new byte[Length];
            base.Read (Bufffer, 0, Length);

            return Encoding.UTF8.GetString (Bufffer);
        }
        #endregion
           
    }
     
}

