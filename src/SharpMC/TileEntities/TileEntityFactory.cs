using System;

namespace SharpMC.TileEntities
{
    public static class TileEntityFactory
    {
        public static TileEntity GetTileEntityByLabel(string entityLabel)
        {
            switch (entityLabel)
            {
                case SignTileEntity.NameId:
                    return new SignTileEntity();
                default:
                    throw new InvalidOperationException(entityLabel);
            }
        }
    }
}