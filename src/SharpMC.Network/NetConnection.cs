using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zlib;
using Microsoft.Extensions.Logging;
using SharpMC.Log;
using SharpMC.Network.Events;
using SharpMC.Network.Packets;
using SharpMC.Network.Util;

namespace SharpMC.Network
{
    public class NetConnection
    {
        private static readonly ILogger Log = LogManager.GetLogger(typeof(NetConnection));
        
        private CancellationTokenSource CancellationToken { get; }
        private ConnectionConfirmed ConnectionConfirmed { get; }
        private Direction Direction { get; }
        private Socket Socket { get; }

        public NetConnection(Direction direction, Socket socket, ConnectionConfirmed confirmdAction = null)
        {
            Direction = direction;
            Socket = socket;
            RemoteEndPoint = Socket.RemoteEndPoint;

            ConnectionConfirmed = confirmdAction;

            CancellationToken = new CancellationTokenSource();

			ConnectionState = ConnectionState.Handshake;
	        IsConnected = true;

			PacketWriteQueue = new BlockingCollection<byte[]>();
        }

        public EventHandler<PacketReceivedEventArgs> OnPacketReceived;
        public EventHandler<ConnectionClosedEventArgs> OnConnectionClosed;

        public EndPoint RemoteEndPoint { get; private set; }
		public ConnectionState ConnectionState { get; protected set; }
		public bool CompressionEnabled { get; protected set; }
		protected int CompressionThreshold = 256;

	    public bool EncryptionInitiated { get; private set; } = false;
		protected byte[] SharedSecret { get; private set; }

		public bool IsConnected { get; private set; }

		private BlockingCollection<byte[]> PacketWriteQueue { get; }

		private Task NetworkProcessing { get; set; }
		private Task NetworkWriting { get; set; }
        internal void Initialize()
        {
	        Socket.Blocking = true;

            NetworkProcessing = new Task(ProcessNetwork, CancellationToken.Token);
            NetworkProcessing.Start();

			NetworkWriting = new Task(SendQueue, CancellationToken.Token);
			NetworkWriting.Start();
        }

        public void Stop()
        {
            if (CancellationToken.IsCancellationRequested) return;
            CancellationToken.Cancel();

            if (SocketConnected(Socket))
            {
                //TODO
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
                if ((bool) _disconnectSync) return;
                _disconnectSync = true;
            }

            if (!CancellationToken.IsCancellationRequested)
            {
                CancellationToken.Cancel();
            }

            Socket.Shutdown(SocketShutdown.Both);
            Socket.Close();

            OnConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(this, notified));

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
	                        Packets.Packet packet = null;
	                        int packetId;
							byte[] packetData;

							if (!CompressionEnabled)
	                        {
		                        var length = ms.ReadVarInt();

		                        int packetIdLength;
		                        packetId = ms.ReadVarInt(out packetIdLength);

		                        if (length - packetIdLength > 0)
		                        {
			                        /*packetData = new byte[length - packetIdLength];
			                        int read = 0;
			                        while (read < packetData.Length)
			                        {
				                        read += ms.Read(packetData, read, packetData.Length - read);

				                        if (CancellationToken.IsCancellationRequested) throw new OperationCanceledException();
			                        }*/
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

								int br;
								var dataLength = ms.ReadVarInt(out br);

								int readMore;
								if (dataLength == 0)
								{
									packetId = ms.ReadVarInt(out readMore);
									packetData = ms.Read(packetLength - (br + readMore));
								}
								else
								{
									var data = ms.Read(packetLength - br);
									byte[] decompressed;
									DecompressData(data, out decompressed);

									using (var b = new MemoryStream(decompressed))
									{
										using (var a = new MinecraftStream(b))
										{
											int l;
											packetId = a.ReadVarInt(out l);
											packetData = a.Read(dataLength - l);
										}
									}
								}
							}

							packet = MCPacketFactory.GetPacket(ConnectionState, packetId);
							if (packet == null)
							{
								Log.LogWarning($"Unhandled package! 0x{packetId.ToString("x2")}");
								continue;
							}
							packet.Decode(new MinecraftStream(new MemoryStream(packetData)));
							HandlePacket(packet);
						}
                    }
                }
            }
            catch(Exception ex)
            {
	            Log.LogWarning("OH NO", ex);
                if (ex is OperationCanceledException) return;
                if (ex is EndOfStreamException) return;
                if (ex is IOException) return;

                Log.LogError("An unhandled exception occurred while processing network!", ex);
            }
            finally
            {
                Disconnected(false);
            }
        }

	    protected virtual void HandlePacket(Packets.Packet packet)
	    {
			var args = new PacketReceivedEventArgs(packet);
		    OnPacketReceived?.BeginInvoke(this, args, PacketReceivedCallback, args);
	    }

        private void PacketReceivedCallback(IAsyncResult ar)
        {
            OnPacketReceived.EndInvoke(ar);
            var args = (PacketReceivedEventArgs)ar.AsyncState;
            if (args.IsInvalid)
            {
                Log.LogWarning("Packet reported as invalid!");
            }
        }

        private void SendDataInternal(byte[] buffer)
        {
	        if (CancellationToken.IsCancellationRequested) return;
            var sendData = new SendData(buffer);
            Socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, sendData);
        }

        private void SendCallback(IAsyncResult ar)
        {
	        try
	        {
		        SocketError result;
		        var sent = Socket.EndSend(ar, out result);

		        var data = (SendData) ar.AsyncState;

		        if (result == SocketError.Success)
		        {
			        if (sent != data.Buffer.Length)
			        {
				        Log.LogWarning("Sent {0} out of {1} bytes!", sent, data.Buffer.Length);
			        }
		        }
		        else
		        {
			        Log.LogWarning("Failed to send data! (Reason: {0})", result);
		        }
			}
			catch { }
        }

	    public void SendPacket(Packet packet)
	    {
			if (packet.PacketId == -1) throw new Exception();

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
							byte[] compressed;
							CompressData(encodedPacket, out compressed);

							mc.WriteVarInt(encodedPacket.Length);
							mc.Write(compressed);
						}
						else //Uncompressed
						{
							mc.WriteVarInt(0);
							mc.Write(encodedPacket);
						}

						encodedPacket = ms.ToArray();
					}
				}
		    }

			PacketWriteQueue.Add(encodedPacket);
		    /*using (MemoryStream ms = new MemoryStream())
		    {
			    using (MinecraftStream mc = new MinecraftStream(ms))
			    {
				    if (EncryptionInitiated)
				    {
					    mc.InitEncryption(SharedSecret);
				    }

					mc.WriteVarInt(encodedPacket.Length);
					mc.Write(encodedPacket);
				}
				SendDataInternal(ms.ToArray());
		    }*/
	    }

	    private MinecraftStream _sendStream;
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

		public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
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
            else
                return true;
        }
    }
}