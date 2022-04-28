using System.Numerics;
using SharpMC.API.Worlds;
using SharpMC.Blocks;

namespace SharpMC.Items
{
    public interface IItemLevel : ILevel
    {
        IBlock GetBlock(Vector3 coordinates);
    }
}