namespace SharpMC.Config
{
    public sealed class ServerSettings
    {
        public NetSettings? Net { get; set; }

        public PluginSettings? Plugins { get; set; }

        public DebugSettings? Debug { get; set; }

        public GeneralSettings? General { get; set; }

        public LevelSettings? Level { get; set; }

        public AdminSettings? Admin { get; set; }
    }
}