namespace SharpMC.Admin
{
    /// <summary>
    /// Server settings
    /// </summary>
    public class ServerSettings
    {
        internal static string Seed = "default";
        internal static bool UseCompression = false;
        internal static int CompressionThreshold = 999999999;
        internal static bool OnlineMode = false;
        internal static bool EncryptionEnabled = true;
        internal static int MaxPlayers = 10;

        public static bool DisplayPacketErrors = false;
        public static bool Debug = false;
        public static string Motd = "";

        public static bool ReportExceptionsToClient = true;
    }
}