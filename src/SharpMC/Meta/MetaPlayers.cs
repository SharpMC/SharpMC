using System.Collections.Generic;

namespace SharpMC.Meta
{
    internal class MetaPlayers
    {
        public int Max { get; set; }
        public int Online { get; set; }
        public List<MetaSample> Sample { get; set; }
    }
}