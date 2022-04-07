using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SharpMC.Core.Utils
{
	public class MyThreadPool
	{
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

		public void LaunchThread(Action action)
		{
			var t = new Thread(obj => action());
			LaunchThread(t);
		}

		public void KillAllThreads()
		{
			foreach (var thread in _threads.Where(thread => thread.IsAlive))
			{
				thread.Abort();
			}
		}

		public void KillThread(int index)
		{
			var id = string.Concat("Thread", index.ToString());
			foreach (var thread in _threads.Where(thread => thread.Name == id))
			{
				thread.Abort();
			}
		}
	}
}