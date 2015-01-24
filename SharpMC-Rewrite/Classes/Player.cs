using System;
using System.ComponentModel;
using System.Collections.Generic;
using SharpMCRewrite.Worlds;
using System.Threading;

namespace SharpMCRewrite
{
    public class Player
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public ClientWrapper Wrapper { get; set; }
        public int UniqueServerID { get; set; }
        public Gamemode Gamemode { get; set; }
        public bool IsSpawned { get; set; }

        //Location stuff
        public byte Dimension { get; set; }
        public Vector3 Coordinates { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public bool OnGround { get; set; }

        //Client settings
        public string Locale { get; set; }
        public byte ViewDistance { get; set; }
        public byte ChatFlags { get; set; }
        public bool ChatColours { get; set; }
        public byte SkinParts { get; set; }

        Vector2 CurrentChunkPosition = new Vector2 (0, 0);
        public bool ForceChunkReload { get; set; }
        private Dictionary<Tuple<int,int>, ChunkColumn> _chunksUsed;

        public Player()
        {
            _chunksUsed = new Dictionary<Tuple<int,int>, ChunkColumn>();
        }

        public void AddToList()
        {
            Globals.Level.AddPlayer (this);
        }

        public void BroadcastMovement()
        {

        }

        public static Player GetPlayer(ClientWrapper wrapper)
        {
            foreach (Player  i in Globals.Level.OnlinePlayers)
            {
                if (i.Wrapper == wrapper)
                {
                    return i;
                }
            }
            throw new ArgumentOutOfRangeException ("The specified player could not be found ;(");
        }

        public void SendChat(string Message)
        {
            new ChatMessage ().Write (Wrapper, new MSGBuffer (Wrapper), new object[1] { Message });
        }

        public void SendChunksFromPosition()
        {
            if (Coordinates == null)
            {
                Coordinates = Globals.Level.Generator.GetSpawnPoint();
                ViewDistance = 9;
            }
            SendChunksForKnownPosition (false);
        }

        public void SendChunksForKnownPosition(bool force = false)
        {
            int centerX = (int) Coordinates.X/16;
            int centerZ = (int) Coordinates.Z/16;

            if (!force && IsSpawned && CurrentChunkPosition == new Vector2(centerX, centerZ)) return;

            CurrentChunkPosition.X = centerX;
            CurrentChunkPosition.Z = centerZ;

            var _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += delegate(object sender, DoWorkEventArgs args)
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                int Counted = 0;
                foreach (var chunk in Globals.Level.Generator.GenerateChunks(ViewDistance, Coordinates.X, Coordinates.Z, force ? new Dictionary<Tuple<int,int>, ChunkColumn>() : _chunksUsed))
                { 
                    if (worker.CancellationPending)
                    {
                        args.Cancel = true;
                        break;
                    }
                        
                    new ChunkData().Write(Wrapper, new MSGBuffer(Wrapper), new object[]{ chunk.GetBytes() });
                    Thread.Yield();

                    if (Counted >= ViewDistance && !IsSpawned)
                    {
                        new PlayerPositionAndLook().Write(Wrapper, new MSGBuffer(Wrapper), new object[0]);

                        IsSpawned = true;
                        Globals.Level.AddPlayer(this);
                    }
                    Counted++;
                }
            };
            _worker.RunWorkerAsync();
        }
    }
}

