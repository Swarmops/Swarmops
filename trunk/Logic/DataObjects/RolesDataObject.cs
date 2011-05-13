using System;
using System.ComponentModel;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Basic.Types.Security;

namespace Activizr.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject (true)]
#endif
    public class RolesDataObject
    {
#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static BasicPersonRole[] Select (int personId, RoleClass roleClass)
        {
            BasicAuthority authority = Authorization.GetPersonAuthority (personId);

            switch (roleClass)
            {
                case RoleClass.System:
                    return authority.SystemPersonRoles;

                case RoleClass.Organization:
                    return authority.OrganizationPersonRoles;

                case RoleClass.Local:
                    return authority.LocalPersonRoles;

                default:
                    throw new InvalidOperationException ("Undefined RoleClass in RolesDataObject.Select: " + roleClass);
            }
        }

#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static BasicPersonRole[] Select (Person person, RoleClass roleClass)
        {
            return Select (person.Identity, roleClass);
        }
    }
}