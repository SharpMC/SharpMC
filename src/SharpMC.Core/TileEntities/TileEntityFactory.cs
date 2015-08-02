namespace SharpMC.Core.TileEntities
{
	public static class TileEntityFactory
	{
		public static TileEntity GetBlockEntityById(string blockEntityId)
		{
			TileEntity tileEntity = null;

			if (blockEntityId == "Sign") tileEntity = new SignTileEntity();

			return tileEntity;
		}
	}
}
