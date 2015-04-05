using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using SharpMCRewrite.Networking.Packages;
using Timer = System.Timers.Timer;

namespace SharpMCRewrite.Classes
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
			//new Thread(ThreadRun).Start();
		}

		public void AddToQuee(byte[] data, bool quee = false)
		{
			//ConsoleFunctions.WriteDebugLine("Data length: " + data.Length);
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

		public void SendData(byte[] data, int length)
		{
			try
			{
				var a = TcpClient.GetStream();
				a.Write(data, 0, length);
				a.Flush();
			}
			catch
			{
				ConsoleFunctions.WriteErrorLine("Failed to send a packet!");
			}
		}

		public void SendData(byte[] data, int offset, int length)
		{
			try
			{
				var a = TcpClient.GetStream();
				a.Write(data, offset, length);
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
			_tickTimer.Start();
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
			if (Player != null)
			{
				Player.Tick();
			}
		}
	}

	public class MyThreadPool
	{
		private IList<Thread> _threads;
		private const int MaxThreads = 500;

		public MyThreadPool()
		{
			_threads = new List<Thread>();
		}

		public void LaunchThread(Thread thread)
		{
			thread.IsBackground = true;
			thread.Name = "Thread" + _threads.Count + 1;
			_threads.Add(thread);
			thread.Start();
		}

		public void KillAllThreads()
		{
			foreach (var thread in _threads)
			{
				if (thread.IsAlive)
				{
					thread.Abort();
				}
			}
		}

		public void LaunchThreads()
		{
			for (int i = 0; i < MaxThreads; i++)
			{
				Thread thread = new Thread(ThreadEntry);
				thread.IsBackground = true;
				thread.Name = string.Format("MyThread{0}", i);

				_threads.Add(thread);
				thread.Start();
			}
		}

		public void KillThread(int index)
		{
			string id = string.Format("MyThread{0}", index);
			foreach (Thread thread in _threads)
			{
				if (thread.Name == id)
					thread.Abort();
			}
		}

		void ThreadEntry()
		{

		}
	}
}