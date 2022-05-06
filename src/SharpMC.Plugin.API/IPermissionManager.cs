using SharpMC.API.Entities;

namespace SharpMC.Plugin.API
{
    public interface IPermissionManager
    {
        bool HasPermission(IPlayer player, string permissionId);
    }
}