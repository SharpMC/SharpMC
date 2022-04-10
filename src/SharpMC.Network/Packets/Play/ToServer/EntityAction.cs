using SharpMC.Network.Util;

namespace SharpMC.Network.Packets.Play.ToServer
{
    public class EntityAction : Packet<EntityAction>, IToServer
    {
        public byte ServerId => 0x1b;

        public int EntityId { get; set; }
        public int ActionId { get; set; }
        public int JumpBoost { get; set; }

        public override void Decode(IMinecraftStream stream)
        {
            EntityId = stream.ReadVarInt();
            ActionId = stream.ReadVarInt();
            JumpBoost = stream.ReadVarInt();
        }

        public override void Encode(IMinecraftStream stream)
        {
            stream.WriteVarInt(EntityId);
            stream.WriteVarInt(ActionId);
            stream.WriteVarInt(JumpBoost);
        }
    }
}
