using System;
using System.Collections.Generic;
using System.Reflection;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Logic.Swarm;

/// <summary>
/// Summary description for ListPersonDataSource
/// </summary>
public class ListPerson : Person
{
    //additional fields for the list
    private DateTime joinedDate;
    public DateTime JoinedDate
    {
        get { return joinedDate; }
        set { joinedDate = value; }
    }

    private DateTime expiresDate;
    public DateTime ExpiresDate
    {
        get { return expiresDate; }
        set { expiresDate = value; }
    }

    private MembershipPaymentStatus paymentStatus;
    public MembershipPaymentStatus PaymentStatus
    {
        get { return paymentStatus; }
        set { paymentStatus = value; }
    }

    int membershipId;

    public int MembershipId
    {
        get { return membershipId; }
        set { membershipId = value; }
    }

    public ListPerson (BasicPerson b)
        : base(b)
    {
    }

    public ListPerson (Person p)
        : base(p)
    {
    }

    public ListPerson ()
        : base(null)
    {
    }


}

public class ListPersonDataSource
{
    int[] listedPersons;
    public int selectedOrgId = 1;

    public ListPersonDataSource ()
    {
    }

    public ListPersonDataSource (int[] listedPersons)
    {
        this.listedPersons = listedPersons;
    }

    public List<ListPerson> GetData (string sortBy)
    {

        List<ListPerson> retval = new List<ListPerson>();
        if (listedPersons != null)
        {
            People ppl = People.FromIdentities(listedPersons);
            Dictionary<int, List<BasicMembership>> membershipTable = Memberships.GetMembershipsForPeople(listedPersons, Membership.GracePeriod);
            Memberships membershipsToLoad = new Memberships();
            foreach (Person p in ppl)
            {
                if (membershipTable.ContainsKey(p.Identity))
                {
                    foreach (BasicMembership bm in membershipTable[p.Identity])
                    {
                        if (bm.OrganizationId == selectedOrgId)
                        {
                            Membership ms = Membership.FromBasic(bm);
                            membershipsToLoad.Add(ms);
                        }
                    }
                }
            }
            Membership.LoadPaymentStatuses(membershipsToLoad);

            Dictionary<int, Membership> memberships = new Dictionary<int, Membership>();
            foreach (Membership ms in membershipsToLoad)
            {
                memberships[ms.Identity] = ms;
            }

            foreach (Person p in ppl)
            {
                ListPerson lp = new ListPerson(p);
                if (membershipTable.ContainsKey(p.Identity))
                {
                    foreach (BasicMembership bm in membershipTable[p.Identity])
                    {
                        if (bm.OrganizationId == selectedOrgId)
                        {
                            Membership ms = memberships[bm.MembershipId];
                            lp.JoinedDate = ms.MemberSince;
                            lp.ExpiresDate = ms.Expires;
                            lp.MembershipId = ms.Identity;
                            retval.Add(lp);
                        }
                    }
                }
            }
        }

        PropertyInfo pi = typeof(ListPerson).GetProperty(sortBy);
        if (pi != null)
        {

            MemberInfo [] miA = pi.PropertyType.GetMember("CompareTo", MemberTypes.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (miA.Length > 0)
            {

                MethodInfo mi =(MethodInfo) miA[0];
                retval.Sort(delegate(ListPerson p1, ListPerson p2)
                {
                    return Convert.ToInt32(mi.Invoke(pi.GetValue(p1, null), new object[] { pi.GetValue(p2, null) }));
                });
            }
        }
        return retval;
    }
}