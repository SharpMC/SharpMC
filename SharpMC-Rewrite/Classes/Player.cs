using System;

namespace SharpMCRewrite
{
    public class Player
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public ClientWrapper Wrapper { get; set; }
        public int UniqueServerID { get; set; }
        public Gamemode Gamemode { get; set; }

        //Location stuff below
        public byte Dimension { get; set; }
        public Vector3 Coordinates { get; set; }
    }
}

