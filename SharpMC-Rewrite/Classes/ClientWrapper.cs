using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using SharpMCRewrite.Networking.Packages;
using Timer = System.Timers.Timer;

namespace SharpMCRewrite
{
	public class ClientWrapper
	{
		private readonly Queue<byte[]> Commands = new Queue<byte[]>();
		private readonly Timer kTimer = new Timer();
		private readonly AutoResetEvent Resume = new AutoResetEvent(false);
		private readonly Timer tickTimer = new Timer();
		public Player Player;
		public bool PlayMode = false;
		public TcpClient TCPClient;

		public ClientWrapper(TcpClient client)
		{
			TCPClient = client;
			new Thread(() => ThreadRun()).Start();
		}

		public void AddToQuee(byte[] data, bool quee = false)
		{
			//ConsoleFunctions.WriteDebugLine("Data length: " + data.Length);
			if (quee || data.Length >= 2048)
			{
				lock (Commands)
				{
					Commands.Enqueue(data);
				}
				Resume.Set();
			}
			else
			{
				SendData(data);
			}
		}

		private void ThreadRun()
		{
			while (true)
			{
				Resume.WaitOne();
				byte[] command;
				lock (Commands)
				{
					command = Commands.Dequeue();
				}
				SendData(command);
			}
		}

		public void SendData(byte[] Data, int Length)
		{
			try
			{
				var a = TCPClient.GetStream();
				a.Write(Data, 0, Length);
				a.Flush();
			}
			catch
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!");
			}
		}

		public void SendData(byte[] Data, int Offset, int Length)
		{
			try
			{
				var a = TCPClient.GetStream();
				a.Write(Data, Offset, Length);
				a.Flush();
			}
			catch
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!");
			}
		}

		public void SendData(byte[] data)
		{
			try
			{
				var a = TCPClient.GetStream();
				a.Write(data, 0, data.Length);
				a.Flush();
			}
			catch
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!");
			}
		}

		public void StartKeepAliveTimer()
		{
			kTimer.Elapsed += DisplayTimeEvent;
			kTimer.Interval = 5000;
			kTimer.Start();

			tickTimer.Elapsed += DoTick;
			tickTimer.Interval = 50;
			tickTimer.Start();
		}

		public void StopKeepAliveTimer()
		{
			kTimer.Stop();
		}

		public void DisplayTimeEvent(object source, ElapsedEventArgs e)
		{
			new KeepAlive(Player.Wrapper).Write();
		}

		public void DoTick(object source, ElapsedEventArgs e)
		{
			if (Player != null)
			{
				Player.Tick();
			}
		}
	}
}