using SharpMC.API.Entities;
using SharpMC.Plugin.API;

namespace SharpMC.Plugins
{
    internal sealed class PermissionManager : IPermissionManager
    {
        public bool HasPermission(IPlayer player, string permissionId)
        {
            // TODO Fix that!
            return true;
        }
    }
}