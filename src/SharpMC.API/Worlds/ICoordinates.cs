using System;

namespace SharpMC.API.Worlds
{
    public interface ICoordinates : IEquatable<ICoordinates>
    {
        int X { get; }

        int Z { get; }
    }
}