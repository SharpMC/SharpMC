namespace SharpMC.Config
{
    public sealed class GeneralSettings
    {
        public string? Motd { get; set; }
        public int MaxPlayers { get; set; }
        public bool OnlineMode { get; }
    }
}