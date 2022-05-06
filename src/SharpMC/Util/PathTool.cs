using System.IO;
using SharpMC.API;

namespace SharpMC.Util
{
    internal static class PathTool
    {
        public static string Ensure(IHostEnv host, string subFolder)
        {
            var path = Path.Combine(host.ContentRoot, subFolder);
            path = Path.GetFullPath(path);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}