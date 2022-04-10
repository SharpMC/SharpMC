using SharpMC.API.Entities;

namespace SharpMC.API.Admin
{
    public interface IPermissionManager
    {
        bool HasPermission(IPlayer player, string permissionName);
    }
}