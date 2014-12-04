using System;
using System.ComponentModel;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Security;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject(true)]
#endif
    public class RolesDataObject
    {
#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static BasicPersonRole[] Select(int personId, RoleClass roleClass)
        {
            BasicAuthority authority = Authorization.GetPersonAuthority(personId);

            switch (roleClass)
            {
                case RoleClass.System:
                    return authority.SystemPersonRoles;

                case RoleClass.Organization:
                    return authority.OrganizationPersonRoles;

                case RoleClass.Local:
                    return authority.LocalPersonRoles;

                default:
                    throw new InvalidOperationException("Undefined RoleClass in RolesDataObject.Select: " + roleClass);
            }
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static BasicPersonRole[] Select(Person person, RoleClass roleClass)
        {
            return Select(person.Identity, roleClass);
        }
    }
}