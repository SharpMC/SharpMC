using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SharpMC.Core.Networking.Packages;
using SharpMC.Core.Utils;

namespace SharpMC.Core.Networking
{
	internal class ClientManager
	{
		private int CurrentIdentifier { get; set; }
		private Timer Ticks { get; set; }
		private Dictionary<int, ClientWrapper> Clients { get; set; } 
		internal ClientManager()
		{
			CurrentIdentifier = 0;
			Clients = new Dictionary<int, ClientWrapper>();
			Ticks = new Timer();
			Ticks.Elapsed += DoServerTick;
			Ticks.Interval = 5000;
			Ticks.Start();
		}

		internal void AddClient(ClientWrapper client)
		{
			if (client.ClientIdentifier == -1)
			{
				CurrentIdentifier++;
				client.ClientIdentifier = CurrentIdentifier;
				Clients.Add(CurrentIdentifier, client);
			}
		}

		internal void RemoveClient(ClientWrapper client)
		{
			if (Clients.ContainsKey(client.ClientIdentifier))
			{
				Clients.Remove(client.ClientIdentifier);
				GC.Collect();
			}
		}

		private void DoServerTick(object obj, ElapsedEventArgs eventargs)
		{
			foreach (var c in Clients.Values.ToArray())
			{
				if (c != null)
				{
					new KeepAlive(c).Write();
				}
			}
		}
	}
}
