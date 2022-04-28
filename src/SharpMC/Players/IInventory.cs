using SharpMC.Items;

namespace SharpMC.Players
{
    public interface IInventory
    {
        IItem GetItemInHand(int number);

        int CurrentSlot { get; }

        void SetSlot(int slot, int a, int b, int c);
    }
}