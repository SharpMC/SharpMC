using System.Collections.Generic;

namespace SharpMC.Generator.Prismarine
{
    public class OneUnit
    {
        public string Namespace { get; set; }

        public string Class { get; set; }

        public string Id { get; set; }

        public string Direction { get; set; }
        
        public List<OneField> Fields { get; set; }

        public override string ToString() => $"[{Id}] {Class}";
    }
}