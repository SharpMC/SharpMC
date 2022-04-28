using SharpMC.API.Entities;
using SharpMC.Players;

namespace SharpMC.Items.Usables
{
    public interface ILevelPlayer : IPlayer
    {
        IInventory Inventory { get; }
    }
}