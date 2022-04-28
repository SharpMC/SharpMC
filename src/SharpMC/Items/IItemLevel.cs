using System.Numerics;
using SharpMC.API.Worlds;
using SharpMC.Blocks;

namespace SharpMC.Items
{
    public interface IItemLevel : ILevel
    {
        IBlock GetBlock(Vector3 coordinates);

        void SetBlock(IBlock block, Vector3 coordinates, bool a, bool b);
    }
}