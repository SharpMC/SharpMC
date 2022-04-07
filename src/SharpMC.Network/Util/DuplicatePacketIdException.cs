using System;

namespace SharpMC.Network.Util
{
    public class DuplicatePacketIdException<TType> : Exception
        where TType : IComparable<TType>
    {
        internal DuplicatePacketIdException(TType id) 
            : base($"A packet with the id \"{id}\" already exists!")
        {
        }
    }
}