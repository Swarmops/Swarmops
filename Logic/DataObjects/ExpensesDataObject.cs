using System.ComponentModel;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject(true)]
#endif
    public class ExpensesDataObject
    {
#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectByClaimer(int personId)
        {
            return ExpenseClaims.FromClaimingPerson(Person.FromIdentity(personId)).ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectOpenByClaimer(int personId)
        {
            return ExpenseClaims.FromClaimingPerson(Person.FromIdentity(personId)).WhereOpen.ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectByClaimer(Person person)
        {
            return SelectByClaimer(person.Identity);
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectOpenByClaimer(Person person)
        {
            return SelectOpenByClaimer(person.Identity);
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectByOrganization(int organizationId)
        {
            return ExpenseClaims.FromOrganization(Organization.FromIdentity(organizationId)).ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectUnapprovedByOrganization(int organizationId)
        {
            return ExpenseClaims.FromOrganization(Organization.FromIdentity(organizationId)).WhereUnapproved.ToArray();
        }


#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static ExpenseClaim[] SelectStatic(ExpenseClaims expenseClaims)
        {
            return expenseClaims.ToArray();
        }
    }
}