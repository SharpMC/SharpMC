using System;
using System.ComponentModel;
using System.Collections.Generic;
using SharpMCRewrite.Worlds;
using System.Threading;
using System.IO;

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

        public Inventory PlayerInventory = new Inventory ();

        Vector2 CurrentChunkPosition = new Vector2 (0, 0);
        public bool ForceChunkReload { get; set; }
        private Dictionary<Tuple<int,int>, ChunkColumn> _chunksUsed;

        public Player()
        {
            _chunksUsed = new Dictionary<Tuple<int,int>, ChunkColumn>();
           // Coordinates = Globals.Level.Generator.GetSpawnPoint ();
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

        public void SavePlayer()
        {
            MSGBuffer data = new MSGBuffer (Wrapper);
            data.WriteByte ((byte)Gamemode);
            data.WriteByte (Dimension);
            data.WriteDouble (Coordinates.X);
            data.WriteDouble (Coordinates.Y);
            data.WriteDouble (Coordinates.Z);
            data.WriteFloat (Yaw);
            data.WriteFloat (Pitch);
            data.WriteBool (OnGround);

            data.WriteInt (PlayerInventory.toSlotArray ().Length);
            data.Write (PlayerInventory.toSlotArray ());

            if (!Directory.Exists(Globals.Level.LVLName + "/players"))
                Directory.CreateDirectory(Globals.Level.LVLName + "/players");

            File.WriteAllBytes (Globals.Level.LVLName + "/players/" + UUID + ".pfile" , Globals.Compress(data.ExportWriter));
        }

        public void FromFile()
        {
            if (File.Exists (Globals.Level.LVLName + "/players/" + UUID + ".pfile"))
            {
                byte[] d = Globals.Decompress(File.ReadAllBytes (Globals.Level.LVLName + "/players/" + UUID + ".pfile"));
                MSGBuffer data = new MSGBuffer(d);
                int GM = data.ReadByte (); //Ergh
                switch (GM)
                {
                    case 0:
                        Gamemode = Gamemode.Surival;
                        break;
                    case 1:
                        Gamemode = Gamemode.Creative;
                        break;
                    case 2:
                        Gamemode = Gamemode.Adventure;
                        break;
                }
                Dimension = (byte)data.ReadByte ();
                double X = data.ReadDouble ();
                ConsoleFunctions.WriteDebugLine (X.ToString());
                double Y = data.ReadDouble ();
                ConsoleFunctions.WriteDebugLine (Y.ToString());
                double Z = data.ReadDouble ();
                ConsoleFunctions.WriteDebugLine (Z.ToString());
              //  Coordinates.X = X;
              //  Coordinates.Y = Y;
              //  Coordinates.Z = Z;
                Yaw = data.ReadFloat ();
                Pitch = data.ReadFloat ();
                OnGround = data.ReadBool ();
                int InvLength = data.ReadInt ();
                byte[] Inventory = data.Read (InvLength);
                PlayerInventory.fromSlotArray (Inventory);
                return;
            }
            return;
        }
            
        public void SendChunksForKnownPosition(bool force = false)
        {
			int VD = ViewDistance;
	//		List<ChunkColumn> chunkies = new List<ChunkColumn> ();
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

                foreach (var chunk in Globals.Level.Generator.GenerateChunks(VD, Coordinates.X, Coordinates.Z, force ? new Dictionary<Tuple<int,int>, ChunkColumn>() : _chunksUsed))
                { 
                    if (worker.CancellationPending)
                    {
                        args.Cancel = true;
                        break;
                    }
                        
                    new ChunkData().Write(Wrapper, new MSGBuffer(Wrapper), new object[]{ chunk.GetBytes() });
                    Thread.Yield();

                    if (Counted >= VD && !IsSpawned)
                    {
                        new PlayerPositionAndLook().Write(Wrapper, new MSGBuffer(Wrapper), new object[0]);

                        IsSpawned = true;
                        Globals.Level.AddPlayer(this);
                        Globals.Level.BroadcastPacket(new PlayerListItem(), new object[] { this, 0 });
                        Globals.Level.BroadcastExistingPlayers(Wrapper);
                        Globals.Level.BroadcastNewPlayer(Wrapper);
                        new WindowItems().Write(Wrapper, new MSGBuffer(Wrapper), new object[] { (byte)0, PlayerInventory.toSlotArray(), 44}); 
                    }
                    Counted++;
                }
            };
            _worker.RunWorkerAsync();
        }
    }
}

