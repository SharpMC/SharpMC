using SharpNBT;

namespace SharpMC.Network.Binary
{
    public interface INbtSerializable
    {
        CompoundTag ToCompound();

        void ToObject(CompoundTag tag);
    }
}