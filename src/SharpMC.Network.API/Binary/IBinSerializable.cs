namespace SharpMC.Network.Binary
{
    public interface IBinSerializable<T>
    {
        T ToCompound();

        void ToObject(T tag);
    }
}