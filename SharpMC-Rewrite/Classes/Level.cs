using System.Collections.Generic;
using MiNET.Worlds;
using System.Threading;
using System.Timers;

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

    public class ILevel
    {
        public string LVLName { get; set; }
        public int Difficulty { get; set; }
        public Gamemode DefaultGamemode { get; set; }
        public LVLType LevelType { get; set; }

        public List<Player> OnlinePlayers { get; set; }

        public int Tick { get; set; }
        public int Day { get; set; }

        public IWorldProvider Generator { get; set; }

        public ILevel()
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
                new ChatMessage ().Write (i.Wrapper, new MSGBuffer (i.Wrapper), new object[] { Message });
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

        public void BroadcastPacket(IPacket packet, object[] Arguments)
        {
            foreach (Player i in OnlinePlayers)
            {
                packet.Write (i.Wrapper, new MSGBuffer (i.Wrapper), Arguments);
            }
        }

        public void SaveChunks()
        {
            Generator.SaveChunks (LVLName);
        }
        #region TickTimer
        private Thread TimerThread;

        public void StartTimeOfDayTimer()
        {
            TimerThread = new Thread (() => StartTimeTimer());
            TimerThread.Start ();
        }

        public void StopTimeOfDayTimer()
        {
            TimerThread.Abort ();
            TimerThread = new Thread (() => StartTimeTimer());
        }

        static System.Timers.Timer kTimer = new System.Timers.Timer();

        private void StartTimeTimer()
        {
            kTimer.Elapsed += new ElapsedEventHandler(RunTick);
            kTimer.Interval = 1000;
            kTimer.Start();
        }

        private void StopTimeTimer()
        {
            kTimer.Stop ();
        }

        private void RunTick(object source, ElapsedEventArgs e)
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
        #endregion
    }
}

