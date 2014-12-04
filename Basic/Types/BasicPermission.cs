using Swarmops.Basic.Enums;

namespace Swarmops.Basic.Types
{
    public class BasicPermission
    {
        public Permission PermissionType;
        public RoleType RoleType;

        public BasicPermission(RoleType pRoleType, Permission pPermissionType)
        {
            this.RoleType = pRoleType;
            this.PermissionType = pPermissionType;
        }
    }
}