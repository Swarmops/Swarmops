using Swarmops.Basic.Enums;

namespace Swarmops.Basic.Types
{
    public class BasicPermission
    {
        public RoleType RoleType;
        public Permission PermissionType;

        public BasicPermission (RoleType pRoleType, Permission pPermissionType)
        {
            RoleType = pRoleType;
            PermissionType = pPermissionType;
        }
    }
}
