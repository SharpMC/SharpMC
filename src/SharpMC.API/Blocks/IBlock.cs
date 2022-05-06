using System;

namespace SharpMC.API.Blocks
{
    public interface IBlock : IEquatable<IBlock>
    {
        int DefaultState { get; }

        int Id { get; }
    }
}