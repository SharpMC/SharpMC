using System;
using SharpMCRewrite.Worlds;

namespace SharpMCRewrite
{
    public class FlatLandLevel : ILevel
    {
        public FlatLandLevel ()
        {
            LVLName = "Flatland";
            Difficulty = 0;
            Generator = new FlatLandGenerator();
            LevelType = LVLType.flat;
        }

    }
}

