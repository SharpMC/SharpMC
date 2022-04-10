using System;

namespace SharpMC.Generator.Prismarine
{
    public class OneField
    {
        public string TypeName { get; set; }
        
        public Type NativeType { get; set; }
        
        public string Name { get; set; }

        public override string ToString() => $"{Name} ({NativeType})";
    }
}