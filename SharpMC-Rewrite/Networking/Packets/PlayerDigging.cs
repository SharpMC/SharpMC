namespace SharpMCRewrite
{
	public class PlayerDigging : IPacket
	{
		public int PacketID
		{
			get { return 0x07; }
		}

		public bool IsPlayePacket
		{
			get { return true; }
		}

		public void Read(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
			var status = buffer.ReadByte();

			if (status == 2 || state.Player.Gamemode == Gamemode.Creative)
			{
				var Position = buffer.ReadPosition();
				var Face = buffer.ReadByte();
				var intVector = new IntVector3((int) Position.X, (int) Position.Y, (int) Position.Z);

				var block = Globals.Level.GetBlock(intVector);
				block.BreakBlock(Globals.Level);
				//Globals.Level.SetBlock(new BlockAir() {Coordinates = intVector});
				state.Player.Digging = false;
			}
			else if (status == 0)
			{
				state.Player.Digging = true;
			}
		}

		public void Write(ClientWrapper state, MSGBuffer buffer, object[] Arguments)
		{
		}
	}
}