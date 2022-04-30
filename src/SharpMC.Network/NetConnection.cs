using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zlib;
using Microsoft.Extensions.Logging;
using SharpMC.Network.API;
using SharpMC.Network.Core;
using SharpMC.Network.Events;
using SharpMC.Network.Packets;
using SharpMC.Network.Packets.API;
using SharpMC.Network.Util;

namespace SharpMC.Network
{
    public class NetConnection
    {
		private readonly ILogger<NetConnection> Log;

		private CancellationTokenSource CancellationToken { get; }
        private EventHandler<ConnectionConfirmedArgs>? ConnectionConfirmed { get; }
        private Direction Direction { get; }
        private Socket Socket { get; }

        public NetConnection(ILogger<NetConnection> log, Direction direction, Socket socket,
			EventHandler<ConnectionConfirmedArgs>? confirmedAction = null)
        {
            Log = log;
            Direction = direction;
            Socket = socket;
            RemoteEndPoint = Socket.RemoteEndPoint!;
            ConnectionConfirmed = confirmedAction;
            CancellationToken = new CancellationTokenSource();
			ConnectionState = ConnectionState.Handshake;
	        IsConnected = true;
			PacketWriteQueue = new BlockingCollection<byte[]>();
        }

        public EventHandler<PacketReceivedArgs>? OnPacketReceived;
        public EventHandler<ConnectionClosedArgs>? OnConnectionClosed;

        public EndPoint RemoteEndPoint { get; private set; }
		public ConnectionState ConnectionState { get; protected set; }
		public bool CompressionEnabled { get; protected set; }
		protected int CompressionThreshold = 256;

	    public bool EncryptionInitiated { get; private set; }
		protected byte[] SharedSecret { get; private set; }

		public bool IsConnected { get; private set; }

		private BlockingCollection<byte[]> PacketWriteQueue { get; }

		private Task NetworkProcessing { get; set; }
		private Task NetworkWriting { get; set; }
		
        public void Initialize()
        {
	        Socket.Blocking = true;

            NetworkProcessing = new Task(ProcessNetwork, CancellationToken.Token);
            NetworkProcessing.Start();

			NetworkWriting = new Task(SendQueue, CancellationToken.Token);
			NetworkWriting.Start();
        }

        public void Stop()
        {
            if (CancellationToken.IsCancellationRequested)
            	return;
            CancellationToken.Cancel();
            if (SocketConnected(Socket))
            {
                Disconnected(true);
            }
            else
            {
                Disconnected(false);
            }
        }

        private object _disconnectSync = false;

        private void Disconnected(bool notified)
        {
            lock (_disconnectSync)
            {
                if ((bool) _disconnectSync) 
                	return;
                _disconnectSync = true;
            }
            if (!CancellationToken.IsCancellationRequested)
            {
                CancellationToken.Cancel();
            }
            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();
            OnConnectionClosed?.Invoke(this, new ConnectionClosedArgs(this, notified));
	        IsConnected = false;
        }

	    protected void InitEncryption(byte[] sharedKey)
	    {
		    SharedSecret = sharedKey;
			_readerStream.InitEncryption(SharedSecret, false);
			_sendStream.InitEncryption(SharedSecret, true);
		    EncryptionInitiated = true;
	    }

	    private MinecraftStream _readerStream;
	    private MinecraftStream _sendStream;
	    
		private void ProcessNetwork()
        {
            try
            {
                using (var ns = new NetworkStream(Socket))
                {
                    using (var ms = new MinecraftStream(ns))
                    {
	                    _readerStream = ms;
                        while (!CancellationToken.IsCancellationRequested)
                        {
	                        Packet packet;
	                        int packetId;
							byte[] packetData;
							if (!CompressionEnabled)
	                        {
		                        var length = ms.ReadVarInt();
		                        packetId = ms.ReadVarInt(out var packetIdLength);
		                        if (length - packetIdLength > 0)
		                        {
			                        packetData = ms.Read(length - packetIdLength);
		                        }
		                        else
		                        {
			                        packetData = new byte[0];
		                        }
	                        }
	                        else
							{
								var packetLength = ms.ReadVarInt();
								var dataLength = ms.ReadVarInt(out var br);
								if (dataLength == 0)
								{
									packetId = ms.ReadVarInt(out var readMore);
									packetData = ms.Read(packetLength - (br + readMore));
								}
								else
								{
									var data = ms.Read(packetLength - br);
									DecompressData(data, out var decompressed);
									using (var b = new MemoryStream(decompressed))
									{
										using (var a = new MinecraftStream(b))
										{
											packetId = a.ReadVarInt(out var l);
											packetData = a.Read(dataLength - l);
										}
									}
								}
							}
							packet = MCPacketFactory.GetPacket(ConnectionState, packetId);
							if (packet == null)
							{
								Log.LogWarning($"Unhandled package! 0x{packetId:x2}");
								continue;
							}
                            Log.LogInformation($" << Receiving packet 0x{packet.PacketId:x2} ({packet.GetType().Name})");
                            packet.Decode(new MinecraftStream(new MemoryStream(packetData)));
							HandlePacket(packet);
						}
                    }
                }
            }
            catch (Exception ex)
            {
	            Log.LogWarning(ex, "OH NO");
                if (ex is OperationCanceledException) 
                	return;
                if (ex is EndOfStreamException) 
                	return;
                if (ex is IOException) 
                	return;
                Log.LogError(ex, "An unhandled exception occurred while processing network!"); 
            }
            finally
            {
                Disconnected(false);
            }
        }

	    protected virtual void HandlePacket(Packet packet)
	    {
			var args = new PacketReceivedArgs(packet);
		    OnPacketReceived?.BeginInvoke(this, args, PacketReceivedCallback, args);
	    }

        private void PacketReceivedCallback(IAsyncResult ar)
        {
            OnPacketReceived.EndInvoke(ar);
            var args = (PacketReceivedArgs) ar.AsyncState;
            if (args.IsInvalid)
            {
                Log.LogWarning("Packet reported as invalid!");
            }
        }

        private void SendDataInternal(byte[] buffer)
        {
	        if (CancellationToken.IsCancellationRequested) 
	        	return;
            var sendData = new SendData(buffer);
            Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, sendData);
        }

        private void SendCallback(IAsyncResult ar)
        {
	        try
	        {
		        var sent = Socket.EndSend(ar, out var result);
		        var data = (SendData) ar.AsyncState;
		        if (result == SocketError.Success)
		        {
			        if (sent != data.Buffer.Length)
			        {
				        Log.LogWarning($"Sent {sent} out of {data.Buffer.Length} bytes!");
			        }
		        }
		        else
		        {
					Log.LogWarning($"Failed to send data! (Reason: {result})");
		        }
			}
			catch
			{
			}
        }

	    public void SendPacket(Packet packet)
	    {
			if (packet.PacketId == -1 && packet is IToClient toClient)
				packet.PacketId = toClient.ClientId;
			if (packet.PacketId == -1)
				throw new Exception();
            Log.LogInformation($" >> Sending packet 0x{packet.PacketId:x2} ({packet.GetType().Name})");
		    byte[] encodedPacket;
			using (var ms = new MemoryStream())
		    {
			    using (var mc = new MinecraftStream(ms))
			    {
					mc.WriteVarInt(packet.PacketId);
					packet.Encode(mc);
					encodedPacket = ms.ToArray();
				    mc.Position = 0;
					mc.SetLength(0);
					if (CompressionEnabled)
					{
						if (encodedPacket.Length >= CompressionThreshold)
						{
							CompressData(encodedPacket, out var compressed);
							mc.WriteVarInt(encodedPacket.Length);
							mc.Write(compressed);
						}
						else 
						{
							// Uncompressed
							mc.WriteVarInt(0);
							mc.Write(encodedPacket);
						}
						encodedPacket = ms.ToArray();
					}
				}
		    }
			PacketWriteQueue.Add(encodedPacket);
	    }

	    private void SendQueue()
	    {
		    using (var ms = new NetworkStream(Socket))
		    {
			    using (var mc = new MinecraftStream(ms))
			    {
				    _sendStream = mc;
				    while (!CancellationToken.IsCancellationRequested)
				    {
					    try
					    {
						    var data = PacketWriteQueue.Take(CancellationToken.Token);
							mc.WriteVarInt(data.Length);
							mc.Write(data);
						}
					    catch (OperationCanceledException)
					    {
						    break;
					    }
				    }
			    }
		    }
	    }

	    public static void CompressData(byte[] inData, out byte[] outData)
	    {
		    using (var outMemoryStream = new MemoryStream())
		    {
			    using (var outZStream = new ZlibStream(outMemoryStream, CompressionMode.Compress, CompressionLevel.Default, true))
			    {
				    outZStream.Write(inData, 0, inData.Length);
			    }
			    outData = outMemoryStream.ToArray();
		    }
	    }

	    public static void DecompressData(byte[] inData, out byte[] outData)
	    {
		    using (var outMemoryStream = new MemoryStream())
		    {
			    using (var outZStream = new ZlibStream(outMemoryStream, CompressionMode.Decompress, CompressionLevel.Default, true))
			    {
				    outZStream.Write(inData, 0, inData.Length);
			    }
			    outData = outMemoryStream.ToArray();
		    }
	    }

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[2000];
			int len;
			while ((len = input.Read(buffer, 0, 2000)) > 0)
			{
				output.Write(buffer, 0, len);
			}
			output.Flush();
		}

		private bool SocketConnected(Socket s)
        {
            var part1 = s.Poll(1000, SelectMode.SelectRead);
            var part2 = s.Available == 0;
            if (part1 && part2)
                return false;
            return true;
        }
    }
}