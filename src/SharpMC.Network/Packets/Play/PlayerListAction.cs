namespace SharpMC.Network.Packets.Play
{
    public enum PlayerListAction : int
    {
        AddPlayer = 0,
        UpdateGamemode = 1,
        UpdateLatency = 2,
        UpdateDisplayName = 3,
        RemovePlayer = 4
    }
}