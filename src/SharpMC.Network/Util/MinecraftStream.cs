﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using SharpMC.Network.API;
using SharpMC.Network.Binary;
using SharpMC.Network.Binary.Special;
using SharpMC.Network.Packets.API;
using SharpNBT;

namespace SharpMC.Network.Util
{
	public class MinecraftStream : Stream, IMinecraftStream
	{
		private BufferedBlockCipher EncryptCipher { get; set; }
		private BufferedBlockCipher DecryptCipher { get; set; }

		private CancellationTokenSource CancelationToken { get; }
		public Stream BaseStream { get; private set; }

		public MinecraftStream(Stream baseStream)
		{
			BaseStream = baseStream;
			CancelationToken = new CancellationTokenSource();
		}

		public MinecraftStream() : this(new MemoryStream())
		{
        }

		public void InitEncryption(byte[] key, bool write)
		{
			EncryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
			EncryptCipher.Init(true, new ParametersWithIV(
				new KeyParameter(key), key, 0, 16));

			DecryptCipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 8));
			DecryptCipher.Init(false, new ParametersWithIV(
				new KeyParameter(key), key, 0, 16));

			BaseStream = new CipherStream(BaseStream, DecryptCipher, EncryptCipher);
		}

		public override bool CanRead => BaseStream.CanRead;
		public override bool CanSeek => BaseStream.CanRead;
		public override bool CanWrite => BaseStream.CanRead;
		public override long Length => BaseStream.Length;

		public override long Position
		{
			get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

		public override long Seek(long offset, SeekOrigin origin)
		{
			return BaseStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			BaseStream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return BaseStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			BaseStream.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			BaseStream.Flush();
		}

		#region Reader

		public override int ReadByte()
		{
			return BaseStream.ReadByte();
		}

        public T ReadNbt<T>() where T : INbtSerializable, new()
        {
            var tag = this.ToCompound()!;
            var obj = tag.ToObject<T>();
            return obj;
        }

        public T ReadBitField<T>() where T : IPacket, new()
        {
            var bits = new T();
			bits.Decode(this);
            return bits;
        }

        public byte[] Read(int length)
		{
            var s = new SpinWait();
			var read = 0;

			var buffer = new byte[length];
            while (read < buffer.Length && !CancelationToken.IsCancellationRequested &&
                   s.Count < 25) 
                // Give the network some time to catch up on sending data,
                // but really 25 cycles should be enough.
			{
				var oldRead = read;

				var r = Read(buffer, read, length - read);
				if (r < 0) //No data read?
				{
					break;
				}

				read += r;

				if (read == oldRead)
				{
					s.SpinOnce();
				}
				if (CancelationToken.IsCancellationRequested) 
                    throw new ObjectDisposedException("");
			}

			return buffer;
		}

		public int ReadInt()
		{
			var dat = Read(4);
			var value = BitConverter.ToInt32(dat, 0);
			return IPAddress.NetworkToHostOrder(value);
		}

        public ISlotData ReadSlot()
        {
            var slot = new SlotData();
            slot.Decode(this);
            return slot;
        }

		public float ReadFloat()
		{
			var almost = Read(4);
			var f = BitConverter.ToSingle(almost, 0);
			return NetworkToHostOrder(f);
		}

		public bool ReadBool()
		{
			var answer = ReadByte();
			if (answer == 1)
				return true;
			return false;
		}

        byte IMinecraftReader.ReadByte()
        {
            return (byte) ReadByte();
        }

		public double ReadDouble()
		{
			var almostValue = Read(8);
			return NetworkToHostOrder(almostValue);
		}

        public byte[] ReadBuffer()
        {
            var length = ReadVarInt(out _);
            var array = new byte[length + 1];
            array[0] = (byte)length;
            _ = Read(array, 1, length);
            return array;
        }

        public string[] ReadStringArray()
        {
            var length = ReadVarInt(out _);
            var result = new string[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadString();
            }
            return result;
        }

        public T[] ReadBitFieldArray<T>() where T : IPacket, new()
        {
            var length = ReadVarInt(out _);
            var result = new T[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadBitField<T>();
            }
            return result;
        }

        public long[] ReadLongArray()
        {
            var length = ReadVarInt(out _);
            var result = new long[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadLong();
            }
            return result;
        }

        public byte[] ReadByteArray()
        {
            var length = ReadVarInt(out _);
            var result = new byte[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = (byte) ReadByte();
            }
            return result;
        }

        public byte[][] ReadByteArrays()
        {
            var length = ReadVarInt(out _);
            var result = new byte[length][];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadByteArray();
            }
            return result;
        }

        public sbyte ReadSByte()
        {
			return (sbyte) ReadByte();
        }

		public int ReadVarInt()
		{
			var read = 0;
			return ReadVarInt(out read);
		}

		public int ReadVarInt(out int bytesRead)
		{
			var numRead = 0;
			var result = 0;
			byte read;
			do
			{
				read = (byte)ReadByte();
				var value = read & 0x7f;
				result |= value << (7 * numRead);
				numRead++;
				if (numRead > 5)
				{
					throw new Exception("VarInt is too big");
				}
			} while ((read & 0x80) != 0);
			bytesRead = numRead;
			return result;
		}

		public long ReadVarLong()
		{
			var numRead = 0;
			long result = 0;
			byte read;
			do
			{
				read = (byte)ReadByte();
				var value = read & 0x7f;
				result |= (uint) (value << (7 * numRead));
				numRead++;
				if (numRead > 10)
				{
					throw new Exception("VarLong is too big");
				}
			} while ((read & 0x80) != 0);

			return result;
		}

		public short ReadShort()
		{
			var da = Read(2);
			var d = BitConverter.ToInt16(da, 0);
			return IPAddress.NetworkToHostOrder(d);
		}

        public uint ReadUInt()
        {
            var da = Read(4);
            return NetworkToHostOrder(BitConverter.ToUInt32(da, 0));
        }

		public ushort ReadUShort()
		{
			var da = Read(2);
			return NetworkToHostOrder(BitConverter.ToUInt16(da, 0));
		}

		public ushort[] ReadUShort(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return NetworkToHostOrder(us);
		}

		public ushort[] ReadUShortLocal(int count)
		{
			var us = new ushort[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToUInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public short[] ReadShortLocal(int count)
		{
			var us = new short[count];
			for (var i = 0; i < us.Length; i++)
			{
				var da = Read(2);
				var d = BitConverter.ToInt16(da, 0);
				us[i] = d;
			}
			return us;
		}

		public string ReadString()
		{
			var length = ReadVarInt();
			var stringValue = Read(length);

			return Encoding.UTF8.GetString(stringValue);
		}

		public long ReadLong()
		{
			var l = Read(8);
			return IPAddress.NetworkToHostOrder(BitConverter.ToInt64(l, 0));
		}

        public byte[] ReadMetadata()
        {
            throw new NotImplementedException();
        }

        public object ReadOptNbt()
        {
            var tag = this.ToCompound();
            return tag;
        }

        public ulong ReadULong()
        {
            var l = Read(8);
            return NetworkToHostOrder(BitConverter.ToUInt64(l, 0));
        }

        public Vector3 ReadPosition()
		{
			var val = ReadLong();
			var x = Convert.ToSingle(val >> 38);
			var y = Convert.ToSingle(val & 0xFFF);
			var z = Convert.ToSingle(val << 38 >> 38 >> 12);
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
			var bytes = BitConverter.GetBytes(network);

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
			var net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt16(net, 0);
		}

        private uint NetworkToHostOrder(uint network)
        {
            var net = BitConverter.GetBytes(network);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(net);
            return BitConverter.ToUInt32(net, 0);
        }

        private ulong NetworkToHostOrder(ulong network)
		{
			var net = BitConverter.GetBytes(network);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(net);
			return BitConverter.ToUInt64(net, 0);
		}

		#endregion

		#region Writer
		public void WriteMetadata(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void WriteBitField(IPacket value)
        {
            value.Encode(this);
        }

        public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}

		public void WritePosition(Vector3 position)
		{
			var x = Convert.ToInt64(position.X);
			var y = Convert.ToInt64(position.Y);
			var z = Convert.ToInt64(position.Z);
			var toSend = ((x & 0x3FFFFFF) << 38) | ((z & 0x3FFFFFF) << 12) | (y & 0xFFF);
			WriteLong(toSend);
		}

        public void WriteStringArray(string[] texts)
        {
            if (texts == null)
            {
                WriteVarInt(0);
				return;
            }
            WriteVarInt(texts.Length);
            foreach (var text in texts)
                WriteString(text);
        }

        public void WriteBitFieldArray<T>(T[] values) where T : IPacket, new()
        {
            if (values == null)
            {
                WriteVarInt(0);
                return;
            }
            WriteVarInt(values.Length);
            foreach (var value in values)
                WriteBitField(value);
        }

        public void WriteLongArray(long[] values)
        {
            if (values == null)
            {
                WriteVarInt(0);
                return;
            }
            WriteVarInt(values.Length);
            foreach (var value in values)
                WriteLong(value);
        }

		public void WriteByteArray(byte[] values)
        {
            if (values == null)
            {
                WriteVarInt(0);
                return;
            }
            WriteVarInt(values.Length);
            foreach (var value in values)
                WriteByte(value);
        }

        public void WriteByteArrays(byte[][] values)
        {
            if (values == null)
            {
                WriteVarInt(0);
                return;
            }
            WriteVarInt(values.Length);
            foreach (var value in values)
                WriteByteArray(value);
        }

        public void WriteSByte(sbyte value)
        {
	        WriteByte((byte) value);
        }

        void IMinecraftWriter.WriteVarInt(int value)
        {
            WriteVarInt(value);
        }

        public int WriteVarInt(int value)
		{
            var write = 0;
            do
            {
                var temp = (byte)(value & 127);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 128;
                }
                WriteByte(temp);
                write++;
            } while (value != 0);
            return write;
		}

		public int WriteVarLong(long value)
		{
			var write = 0;
			do
			{
				var temp = (byte)(value & 127);
				value >>= 7;
				if (value != 0)
				{
					temp |= 128;
				}
				WriteByte(temp);
				write++;
			} while (value != 0);
			return write;
		}

		public void WriteInt(int data)
		{
			var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(buffer);
		}

        public void WriteBuffer(byte[] data)
        {
            Write(data);
        }

        public void WriteString(string data)
        {
            var txt = data ?? string.Empty;
			var stringData = Encoding.UTF8.GetBytes(txt);
			WriteVarInt(stringData.Length);
			Write(stringData);
		}

		public void WriteShort(short data)
		{
			var shortData = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data));
			Write(shortData);
		}

        public void WriteSlot(ISlotData value)
        {
            value.Encode(this);
        }

		public void WriteUShort(ushort data)
		{
			var uShortData = BitConverter.GetBytes(data);
			Write(uShortData);
		}

		public void WriteBool(bool data)
		{
			Write(BitConverter.GetBytes(data));
		}

		public void WriteDouble(double data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteFloat(float data)
		{
			Write(HostToNetworkOrder(data));
		}

		public void WriteLong(long data)
		{
			Write(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data)));
		}

        public void WriteOptNbt(object data)
        {
            if (data == null)
            {
                WriteByte(0);
                return;
            }
            byte[] bytes;
            if (data is INbtSerializable s)
            {
                bytes = s.ToBytes();
            }
            else
            {
                bytes = ((CompoundTag) data).ToBytes();
            }
            Write(bytes);
        }

        public void WriteNbt(INbtSerializable data)
        {
            var bytes = data.ToBytes();
			Write(bytes);
        }

		public void WriteULong(ulong data)
		{
			Write(HostToNetworkOrderLong(data));
		}

        public void WriteUuid(Guid uuid)
		{
			var guid = uuid.ToByteArray();
			var long1 = new byte[8];
			var long2 = new byte[8];
			Array.Copy(guid, 0, long1, 0, 8);
			Array.Copy(guid, 8, long2, 0, 8);
			Write(long1);
			Write(long2);
		}

		public Guid ReadUuid()
		{
			var long1 = Read(8);
			var long2 = Read(8);
            return new Guid(long1.Concat(long2).ToArray());
		}

        private byte[] HostToNetworkOrder(double d)
		{
			var data = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian)
				Array.Reverse(data);
            return data;
		}

		private byte[] HostToNetworkOrder(float host)
		{
			var bytes = BitConverter.GetBytes(host);
            if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
            return bytes;
		}

		private byte[] HostToNetworkOrderLong(ulong host)
		{
			var bytes = BitConverter.GetBytes(host);
            if (BitConverter.IsLittleEndian)
				Array.Reverse(bytes);
            return bytes;
		}

        #endregion

        private object _disposeLock = new();
		private bool _disposed;

		protected override void Dispose(bool disposing)
		{
			if (!Monitor.IsEntered(_disposeLock))
				return;
            try
			{
				if (disposing && !_disposed)
				{
					_disposed = true;
                    if (!CancelationToken.IsCancellationRequested)
						CancelationToken.Cancel();
                }
				base.Dispose(disposing);
			}
			finally
			{
				Monitor.Exit(_disposeLock);
			}
		}
    }
}