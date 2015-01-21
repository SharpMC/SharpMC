using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace SharpMCRewrite
{
    public class ByteBuffer
    {
        private List<byte> bffr = new List<byte>();
        NetworkStream mStream;
        ClientWrapper Client;

        public ByteBuffer(NetworkStream str, ClientWrapper client)
        {
            mStream = str;
        }

        public void Write(byte[] Data, int Offset, int Length)
        {
            for (int i = 0; i < Length; i++)
            {
                bffr.Add (Data [i + Offset]);
            }
        }

        public void Write(byte[] Data)
        {
            foreach (byte i in Data)
            {
                bffr.Add (i);
            }
        }

        public void WriteVarInt(int Integer)
        {
            while ((Integer & -128) != 0)
            {
                bffr.Add((byte)(Integer & 127 | 128));
                Integer = (int)(((uint)Integer) >> 7);
            }
            bffr.Add((byte)Integer);
        }

        public void WriteVarLong(long i)
        {
           long Fuck = i;
            while ((Fuck & ~0x7F) != 0) {
                bffr.Add((byte)((Fuck & 0x7F) | 0x80));
                Fuck >>= 7;
            }
            bffr.Add((byte)Fuck);
        }

        public void WriteInt(int Data)
        {
            byte[] Buffer = BitConverter.GetBytes (Data);
            Write (Buffer);
        }

        public void WriteString(string Data)
        {
            byte[] StringData = Encoding.UTF8.GetBytes (Data);
            WriteVarInt (StringData.Length);
            Write (StringData);
        }

        public void WriteShort(short Data)
        {
            byte[] ShortData = BitConverter.GetBytes (Data);
            Write (ShortData);
        }

        public void WriteByte(byte Data)
        {
            bffr.Add (Data);
        }

        public void WriteBool(bool Data)
        {
            Write(BitConverter.GetBytes (Data));
        }

        /// <summary>
        /// Flush all data to the TCPClient NetworkStream.
        /// </summary>
        public void FlushData()
        {
            byte[] AllData = bffr.ToArray ();
            bffr.Clear ();

            WriteVarInt (AllData.Length);
            byte[] Buffer = bffr.ToArray ();

            mStream.Write (Buffer, 0, Buffer.Length);
            mStream.Write (AllData, 0, AllData.Length);
            bffr.Clear ();
        }
    }
}

