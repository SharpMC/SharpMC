// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

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