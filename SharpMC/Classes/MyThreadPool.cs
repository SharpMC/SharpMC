using System.Collections.Generic;
using System.Threading;

namespace SharpMC.Classes
{
	public class MyThreadPool
	{
		private const int MaxThreads = 500;
		private readonly IList<Thread> _threads;

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

		public void KillThread(int index)
		{
			var id = string.Format("MyThread{0}", index);
			foreach (var thread in _threads)
			{
				if (thread.Name == id)
					thread.Abort();
			}
		}
	}
}