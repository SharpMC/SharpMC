using System.Collections.Generic;
using System.Numerics;
using SharpMC.API.Worlds;
using SharpMC.Items;
using SharpNBT;

namespace SharpMC.TileEntities
{
    public class TileEntity
    {
        public string Id { get; }

        public TileEntity(string id)
        {
            Id = id;
        }

        public Vector3 Coordinates { get; set; }
        public bool UpdatesOnTick { get; set; }

        public virtual CompoundTag GetCompound(string name = null)
        {
            return new CompoundTag(name);
        }

        public virtual void SetCompound(CompoundTag compound)
        {
        }

        public virtual void OnTick(ILevel level)
        {
        }

        public virtual List<LootItem> GetDrops()
        {
            return new List<LootItem>();
        }

        protected string GetTextValue(CompoundTag compound, string key)
        {
            var text = compound[key] as StringTag;
            return text != null ? text.Value ?? string.Empty : string.Empty;
        }
    }
}