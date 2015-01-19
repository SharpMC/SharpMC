using System;
using System.Collections.Generic;
using System.Text;
using SharpMCRewrite.Networking;
using System.Net.Sockets;
using System.IO;

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
            foreach (byte i in Buffer)
            {
                bffr.Add (i);
            }
        }

        public void WriteString(string Data)
        {
            byte[] StringData = Encoding.UTF8.GetBytes (Data);
            WriteVarInt (StringData.Length);
            foreach (byte i in StringData)
            {
                bffr.Add (i);
            }
        }

        public void WriteShort(short Data)
        {
            byte[] ShortData = BitConverter.GetBytes (Data);
            foreach (byte i in ShortData)
            {
                bffr.Add (i);
            }
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

