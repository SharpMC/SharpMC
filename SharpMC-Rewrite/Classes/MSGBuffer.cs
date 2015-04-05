using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SharpMCRewrite.Classes
{
	public class MSGBuffer
	{
		private ClientWrapper _client;
		public int Size = 0;
		private int LastByte = 0;
		public byte[] BufferedData = new byte[4096];

		public MSGBuffer(ClientWrapper client)
		{
			_client = client;
			if (client.TCPClient.Connected) mStream = client.TCPClient.GetStream();
		}

		public MSGBuffer(NetworkStream stream)
		{
			mStream = stream;
		}

		public MSGBuffer(byte[] data)
		{
			BufferedData = data;
		}

		#region Reader
		public int ReadByte()
		{
			byte returnData = BufferedData[LastByte];
			LastByte++;
			return returnData;
		}

		public byte[] Read(int Length)
		{
			byte[] Buffered = new byte[Length];
			Array.Copy(BufferedData, LastByte, Buffered, 0, Length);
			LastByte += Length;
			return Buffered;
		}


		public int ReadInt()
		{
			byte[] Dat = Read(4);
			int Value = BitConverter.ToInt32(Dat, 0);
			return IPAddress.NetworkToHostOrder(Value);
		}

		public float ReadFloat()
		{
			byte[] Almost = Read(4);
			float f = BitConverter.ToSingle(Almost, 0);
			return NetworkToHostOrder(f);
		}

		public bool ReadBool()
		{
			int Answer = ReadByte();
			if (Answer == 1)
				return true;
			else
				return false;
		}

		public double ReadDouble()
		{
			byte[] AlmostValue = Read(8);
			return NetworkToHostOrder(AlmostValue);
		}

		public int ReadVarInt()
		{
			int value = 0;
			int size = 0;
			int b;
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++ * 7);
				if (size > 5)
				{
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
			while (((b = ReadByte()) & 0x80) == 0x80)
			{
				value |= (b & 0x7F) << (size++ * 7);
				if (size > 10)
				{
					throw new IOException("VarLong too long. That's what she said.");
				}
			}
			return value | ((b & 0x7F) << (size * 7));
		}

		public short ReadShort()
		{
			byte[] Da = Read(2);
			short D = BitConverter.ToInt16(Da, 0);
			return IPAddress.NetworkToHostOrder(D);
		}

		public ushort ReadUShort()
		{
			byte[] Da = Read(2);
			return NetworkToHostOrder(BitConverter.ToUInt16(Da, 0));
		}

		public ushort[] ReadUShort(int count)
		{
			ushort[] us = new ushort[count];
			for (int i = 0; i < us.Length; i++)
			{
				byte[] Da = Read(2);
				ushort D = BitConverter.ToUInt16(Da, 0);
				us[i] = D;
			}
			return NetworkToHostOrder(us);
			//return IPAddress.NetworkToHostOrder (D);
		}

		public ushort[] ReadUShortLocal(int count)
		{
			ushort[] us = new ushort[count];
			for (int i = 0; i < us.Length; i++)
			{
				byte[] Da = Read(2);
				ushort D = BitConverter.ToUInt16(Da, 0);
				us[i] = D;
			}
			return us;
			//return IPAddress.NetworkToHostOrder (D);
		}

		public string ReadString()
		{
			int Length = ReadVarInt();
			byte[] StringValue = Read(Length);
			return Encoding.UTF8.GetString(StringValue);
		}

		public long ReadLong()
		{
			byte[] l = Read(8);
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(l, 0));
		}

		public Vector3 ReadPosition()
		{
			long val = ReadLong();
			double x = Convert.ToDouble(val >> 38);
			double y = Convert.ToDouble((val >> 26) & 0xFFF);
			double z = Convert.ToDouble(val << 38 >> 38);
			return new Vector3(x, y, z);
		}

		private double NetworkToHostOrder(byte[] data)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
			return BitConverter.ToDouble(data, 0);
		}

		private float NetworkToHostOrder(float network)
		{
			byte[] bytes = BitConverter.GetBytes(network);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return BitConverter.ToSingle(bytes, 0);
		}

		private ushort[] NetworkToHostOrder(ushort[] network)
		{
			if (BitConverter.IsLittleEndian)
				Array.Reverse(network);
			return network;
		}

		private ushort NetworkToHostOrder(ushort network)
		{
			byte[] net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt16(net, 0);
		}
		#endregion

		#region Writer
		public byte[] ExportWriter
		{
			get
			{
				return bffr.ToArray();
			}
		}
		private List<byte> bffr = new List<byte>();
		private NetworkStream mStream;

		public void Write(byte[] Data, int Offset, int Length)
		{
			for (int i = 0; i < Length; i++)
			{
				bffr.Add(Data[i + Offset]);
			}
		}

		public void Write(byte[] Data)
		{
			foreach (byte i in Data)
			{
				bffr.Add(i);
			}
		}

		public void WritePosition(Vector3 position)
		{
			long x = Convert.ToInt64(position.X);
			long y = Convert.ToInt64(position.Y);
			long z = Convert.ToInt64(position.Z);
			var toSend = ((x & 0x3FFFFFF) << 38) | ((y & 0xFFF) << 26) | (z & 0x3FFFFFF);
			WriteLong(toSend);
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
			while ((Fuck & ~0x7F) != 0)
			{
				bffr.Add((byte)((Fuck & 0x7F) | 0x80));
				Fuck >>= 7;
			}
			bffr.Add((byte)Fuck);
		}

		public void WriteInt(int Data)
		{
			byte[] Buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data));
			Write(Buffer);
		}

		public void WriteString(string Data)
		{
			byte[] StringData = Encoding.UTF8.GetBytes(Data);
			WriteVarInt(StringData.Length);
			Write(StringData);
		}

		public void WriteShort(short Data)
		{
			byte[] ShortData = BitConverter.GetBytes(Data);
			Write(ShortData);
		}

		public void WriteUShort(ushort Data)
		{
			byte[] UShortData = BitConverter.GetBytes(Data);
			Write(UShortData);
		}

		public void WriteByte(byte Data)
		{
			bffr.Add(Data);
		}

		public void WriteBool(bool Data)
		{
			Write(BitConverter.GetBytes(Data));
		}

		public void WriteDouble(double Data)
		{
			Write(HostToNetworkOrder(Data));
		}

		public void WriteFloat(float Data)
		{
			Write(HostToNetworkOrder(Data));
		}

		public void WriteLong(long Data)
		{
			Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data)));
		}

		public void WriteUUID(Guid uuid)
		{
			byte[] guid = uuid.ToByteArray();
			byte[] Long1 = new byte[8];
			byte[] Long2 = new byte[8];
			Array.Copy(guid, 0, Long1, 0, 8);
			Array.Copy(guid, 8, Long2, 0, 8);
			Write(Long1);
			Write(Long2);
		}

		/// <summary>
		/// Flush all data to the TCPClient NetworkStream.
		/// </summary>
		public void FlushData(bool quee = false)
		{
			try
			{
				byte[] AllData = bffr.ToArray();
				bffr.Clear();

				WriteVarInt(AllData.Length);
				byte[] Buffer = bffr.ToArray();

				// ConsoleFunctions.WriteDebugLine ("Specified Data length: " + AllData.Length);
				//  ConsoleFunctions.WriteDebugLine ("Full packet length: " + (AllData.Length + Buffer.Length));
				//                mStream.Write (Buffer, 0, Buffer.Length);
				//              mStream.Write (AllData, 0, AllData.Length);
				List<byte> data = new List<byte>();
				foreach (byte i in Buffer)
				{
					data.Add(i);
				}
				foreach (byte i in AllData)
				{
					data.Add(i);
				}
				_client.AddToQuee(data.ToArray(), quee);
				bffr.Clear();
			}
			catch (Exception ex)
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!\n" + ex.ToString());
			}
		}

		public void FlushData(int packetId)
		{
			try
			{
				byte[] AllData = bffr.ToArray();
				bffr.Clear();

				WriteVarInt(packetId);
				WriteVarInt(AllData.Length);
				byte[] Buffer = bffr.ToArray();

				// ConsoleFunctions.WriteDebugLine ("Specified Data length: " + AllData.Length);
				//  ConsoleFunctions.WriteDebugLine ("Full packet length: " + (AllData.Length + Buffer.Length));
				mStream.Write(Buffer, 0, Buffer.Length);
				mStream.Write(AllData, 0, AllData.Length);
				bffr.Clear();
			}
			catch (Exception ex)
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!\n" + ex.ToString());
			}
		}

		private byte[] HostToNetworkOrder(double d)
		{
			byte[] data = BitConverter.GetBytes(d);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data);
			}
			return data;
		}

		private byte[] HostToNetworkOrder(float host)
		{
			byte[] bytes = BitConverter.GetBytes(host);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);

			return bytes;
		}

		#endregion
	}
}