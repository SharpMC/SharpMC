using SharpMC.Enums;
using SharpMC.Utils;

namespace SharpMC.Networking.Packages
{
    public class ChangeGameState : Package<ChangeGameState>
    {
	    public GameStateReason Reason = GameStateReason.ChangeGameMode;
	    public float Value = 0;
	    public ChangeGameState(ClientWrapper client) : base(client)
	    {
		    SendId = 0x2B;
	    }

	    public ChangeGameState(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
	    {
			SendId = 0x2B;
	    }

	    public override void Write()
	    {
		    if (Buffer != null)
		    {
			    Buffer.WriteVarInt(SendId);
				Buffer.WriteByte((byte)Reason);
				Buffer.WriteFloat(Value);
				Buffer.FlushData();
		    }
	    }
    }
}
