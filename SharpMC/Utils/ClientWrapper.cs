using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using SharpMC.Entity;
using SharpMC.Networking.Packages;
using Timer = System.Timers.Timer;

namespace SharpMC.Utils
{
	public class ClientWrapper
	{
		private readonly Queue<byte[]> _commands = new Queue<byte[]>();
		private readonly Timer _kTimer = new Timer();
		private readonly AutoResetEvent _resume = new AutoResetEvent(false);
		private readonly Timer _tickTimer = new Timer();
		public Player Player;
		public bool PlayMode = false;
		public TcpClient TcpClient;
		public MyThreadPool ThreadPool;

		public ClientWrapper(TcpClient client)
		{
			TcpClient = client;
			ThreadPool = new MyThreadPool();
			ThreadPool.LaunchThread(new Thread(ThreadRun));
		}

		public void AddToQuee(byte[] data, bool quee = false)
		{
			if (quee)
			{
				lock (_commands)
				{
					_commands.Enqueue(data);
				}
				_resume.Set();
			}
			else
			{
				SendData(data);
			}
		}

		private void ThreadRun()
		{
			while (_resume.WaitOne())
			{
				byte[] command;
				lock (_commands)
				{
					command = _commands.Dequeue();
				}
				SendData(command);
			}
		}

		public void SendData(byte[] data)
		{
			try
			{
				var a = TcpClient.GetStream();
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
			_kTimer.Elapsed += DisplayTimeEvent;
			_kTimer.Interval = 5000;
			_kTimer.Start();

			_tickTimer.Elapsed += DoTick;
			_tickTimer.Interval = 50;
			//_tickTimer.Start();
		}

		public void StopKeepAliveTimer()
		{
			_kTimer.Stop();
		}

		public void DisplayTimeEvent(object source, ElapsedEventArgs e)
		{
			new KeepAlive(Player.Wrapper).Write();
		}

		public void DoTick(object source, ElapsedEventArgs e)
		{
			//if (Player != null)
			//{
			//	Player.OnTick();
			//}
		}
	}
}