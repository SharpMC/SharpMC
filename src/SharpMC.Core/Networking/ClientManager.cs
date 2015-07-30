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
		private Dictionary<int, int> PacketErrors { get; set; } 
		internal ClientManager()
		{
			CurrentIdentifier = 0;
			Clients = new Dictionary<int, ClientWrapper>();
			PacketErrors = new Dictionary<int, int>();
			Ticks = new Timer();
			Ticks.Elapsed += DoServerTick;
			Ticks.Interval = 5000;
			Ticks.Start();
		}

		internal void AddClient(ref ClientWrapper client)
		{
			if (client.ClientIdentifier == -1)
			{
				CurrentIdentifier++;
				client.ClientIdentifier = CurrentIdentifier;
				Clients.Add(CurrentIdentifier, client);
				PacketErrors.Add(CurrentIdentifier, 0);
			}
		}

		internal void RemoveClient(ClientWrapper client)
		{
			if (Clients.ContainsKey(client.ClientIdentifier))
			{
				Clients.Remove(client.ClientIdentifier);
				PacketErrors.Remove(client.ClientIdentifier);
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

		public void PacketError(ClientWrapper client)
		{
			if (PacketErrors.ContainsKey(client.ClientIdentifier))
			{
				int errors = PacketErrors[client.ClientIdentifier];
				PacketErrors[client.ClientIdentifier] = errors + 1;

				if (ServerSettings.DisplayPacketErrors)
				{
					ConsoleFunctions.WriteWarningLine("Packet error for player: \"" + client.Player.Username + "\" Packet errors: " +
					                                  PacketErrors[client.ClientIdentifier]);
				}

				if (PacketErrors[client.ClientIdentifier] > 3)
				{
					//RemoveClient(client);
					client.Kicked = true;
				}
			}
			else
			{
				//Something really wrong
			}
		}

		public void CleanErrors(ClientWrapper client)
		{
			if (PacketErrors.ContainsKey(client.ClientIdentifier))
			{
				PacketErrors[client.ClientIdentifier] = 0;
			}
			else
			{
				//Something really wrong
			}
		}
	}
}
