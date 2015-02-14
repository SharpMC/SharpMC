using System;
using System.Collections.Generic;
using System.Net;
using MiNET.Worlds;
using System.Threading;
using System.Timers;
using SharpMCRewrite.Blocks;
using SharpMCRewrite.NET;
using SharpMCRewrite.Worlds;

namespace SharpMCRewrite
{
    public enum LVLType
    {
        Default,
        flat,
        largeBiomes,
        amplified,
        default_1_1
    }

    public class Level
    {
        public string LVLName { get; set; }
        public int Difficulty { get; set; }
        public Gamemode DefaultGamemode { get; set; }
        public LVLType LevelType { get; set; }

        public List<Player> OnlinePlayers { get; set; }

        public int Tick { get; set; }
        public int Day { get; set; }

        public IWorldProvider Generator { get; set; }

        public Level()
        {
            Tick = 1200;
            Day = 0;
            OnlinePlayers = new List<Player> ();
            DefaultGamemode = Gamemode.Creative;
        }

        public void RemovePlayer(Player player)
        {
            OnlinePlayers.Remove (player);
        }

        public void AddPlayer(Player player)
        {
            OnlinePlayers.Add (player);
        }

        public void BroadcastChat(string Message)
        {
            foreach(Player i in OnlinePlayers)
            {
                //new ChatMessage ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[] { Message });
				new Networking.Packages.ChatMessage(i.Wrapper){Message = @Message}.Write();
			}
            ConsoleFunctions.WriteInfoLine ("Chat: " + Message);
        }

        public void BroadcastData(byte[] Data)
        {
            Thread broadcaster = new Thread(() => Broadcaster(Data));
            broadcaster.IsBackground = true;
            broadcaster.Start ();
        }

        private void Broadcaster(byte[] Data)
        {
            foreach (Player i in OnlinePlayers)
            {
                i.Wrapper.SendData (Data);
            }
        }

        public void BroadcastPacket(IPacket packet, object[] Arguments, bool self = true, Player source = null)
        {
            foreach (Player i in OnlinePlayers)
            {
	            if (!self && i == source)
	            {
		           continue;
	            }
                packet.Write (i.Wrapper, new MSGBuffer (i.Wrapper), Arguments);
            }
        }

        public void BroadcastPlayers(ClientWrapper Target)
        {
            foreach (Player i in OnlinePlayers)
            {
                new SpawnPlayer ().Write (Target, new MSGBuffer (Target), new object[] { i });
            }
        }

        public void BroadcastNewPlayer(ClientWrapper newPlayer)
        {
            foreach (Player i in OnlinePlayers)
            {
                if (i.Wrapper != newPlayer)
                {
                    new SpawnPlayer ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[] { newPlayer.Player });
                    new SpawnPlayer ().Write (newPlayer, new MSGBuffer (newPlayer), new object[] { i });
                }
            }
        }

        public void BroadcastExistingPlayers(ClientWrapper Caller)
        {
            foreach (Player i in OnlinePlayers)
            {
                if (i.Wrapper != Caller)
                {
                    new PlayerListItem ().Write (Caller, new MSGBuffer (Caller), new object[] { i, 0 });
                }
            }
        }

        public void BroadcastPlayerRemoval(ClientWrapper player)
        {
            BroadcastPacket (new PlayerListItem (), new object[] { player.Player, 4});
        }

        public void SaveChunks()
        {
            Generator.SaveChunks (LVLName);
        }

		public Block GetBlock(IntVector3 blockCoordinates)
		{
			//ChunkColumn chunk = Generator.GenerateChunkColumn(new Vector2(blockCoordinates.X / 16, blockCoordinates.Z / 16));
			
			ChunkColumn chunk = Generator.GetChunk(blockCoordinates.X / 16, blockCoordinates.Z / 16);	
			
			ushort bid = chunk.GetBlock(blockCoordinates.X & 0x0f, blockCoordinates.Y & 0x7f, blockCoordinates.Z & 0x0f);
			
			byte metadata = chunk.GetMetadata(blockCoordinates.X & 0x0f, blockCoordinates.Y & 0x7f, blockCoordinates.Z & 0x0f);
			
			bid = (ushort) (bid >> 4);
			
			Block block = BlockFactory.GetBlockById(bid);
			block.Coordinates = blockCoordinates;
			block.Metadata = metadata;

			return block;
		}

	    public void SetBlock(Block block, bool broadcast = true)
	    {
		    Generator.SetBlock(block, this, broadcast);
	    }

	    #region TickTimer
        private Thread TimerThread;
        private Thread GameTickThread;

        public void StartTimeOfDayTimer()
        {
            TimerThread = new Thread (() => StartTimeTimer());
            TimerThread.Start ();

            GameTickThread = new Thread (() => StartTickTimer());
            GameTickThread.Start ();
        }

        public void StopTimeOfDayTimer()
        {
            TimerThread.Abort ();
            TimerThread = new Thread (() => StartTimeTimer());
        }

        static System.Timers.Timer kTimer = new System.Timers.Timer();
        static System.Timers.Timer kTTimer = new System.Timers.Timer();

        private void StartTimeTimer()
        {
            kTimer.Elapsed += new ElapsedEventHandler(RunDayTick);
            kTimer.Interval = 1000;
            kTimer.Start();
        }
            
        private void StartTickTimer()
        {
            kTTimer.Elapsed += new ElapsedEventHandler(GameTick);
            kTTimer.Interval = 50;
            kTTimer.Start();
        }

        private void StopTimeTimer()
        {
            kTimer.Stop ();
        }

        private void RunDayTick(object source, ElapsedEventArgs e)
        {
            if (Tick < 24000)
            {
                Tick += 20;
            }
            else
            {
                Tick = 0;
                Day++;
            }

            foreach (Player i in OnlinePlayers)
            {
                new TimeUpdate ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[0]);
            }
        }

        private void GameTick(object source, ElapsedEventArgs e)
        {
           
        }
        #endregion
    }
}

