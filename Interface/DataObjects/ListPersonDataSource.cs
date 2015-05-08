using System;
using System.Collections.Generic;
using System.Reflection;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Logic.Swarm;

/// <summary>
///     Summary description for ListPersonDataSource
/// </summary>
public class ListPerson : Person
{
    //additional fields for the list
    private DateTime expiresDate;
    private DateTime joinedDate;

    public ListPerson(BasicPerson b)
        : base(b)
    {
    }

    public ListPerson(Person p)
        : base(p)
    {
    }

    public ListPerson()
        : base(null)
    {
    }

    public DateTime JoinedDate
    {
        get { return this.joinedDate; }
        set { this.joinedDate = value; }
    }

    public DateTime ExpiresDate
    {
        get { return this.expiresDate; }
        set { this.expiresDate = value; }
    }

    public MembershipPaymentStatus PaymentStatus { get; set; }

    public int MembershipId { get; set; }
}

public class ListPersonDataSource
{
    private readonly int[] listedPersons;
    public int selectedOrgId = 1;

    public ListPersonDataSource()
    {
    }

    public ListPersonDataSource(int[] listedPersons)
    {
        this.listedPersons = listedPersons;
    }

    public List<ListPerson> GetData(string sortBy)
    {
        List<ListPerson> retval = new List<ListPerson>();
        if (this.listedPersons != null)
        {
            People ppl = People.FromIdentities(this.listedPersons);
            Dictionary<int, List<BasicParticipation>> membershipTable =
                Participations.GetParticipationsForPeople(this.listedPersons, Participation.GracePeriod);
            Participations participationsToLoad = new Participations();
            foreach (Person p in ppl)
            {
                if (membershipTable.ContainsKey(p.Identity))
                {
                    foreach (BasicParticipation bm in membershipTable[p.Identity])
                    {
                        if (bm.OrganizationId == this.selectedOrgId)
                        {
                            Participation ms = Participation.FromBasic(bm);
                            participationsToLoad.Add(ms);
                        }
                    }
                }
            }
            Participation.LoadPaymentStatuses(participationsToLoad);

            Dictionary<int, Participation> memberships = new Dictionary<int, Participation>();
            foreach (Participation ms in participationsToLoad)
            {
                memberships[ms.Identity] = ms;
            }

            foreach (Person p in ppl)
            {
                ListPerson lp = new ListPerson(p);
                if (membershipTable.ContainsKey(p.Identity))
                {
                    foreach (BasicParticipation bm in membershipTable[p.Identity])
                    {
                        if (bm.OrganizationId == this.selectedOrgId)
                        {
                            Participation ms = memberships[bm.MembershipId];
                            lp.JoinedDate = ms.MemberSince;
                            lp.ExpiresDate = ms.Expires;
                            lp.MembershipId = ms.Identity;
                            retval.Add(lp);
                        }
                    }
                }
            }
        }

        PropertyInfo pi = typeof (ListPerson).GetProperty(sortBy);
        if (pi != null)
        {
            MemberInfo[] miA = pi.PropertyType.GetMember("CompareTo", MemberTypes.Method,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (miA.Length > 0)
            {
                MethodInfo mi = (MethodInfo) miA[0];
                retval.Sort(
                    delegate(ListPerson p1, ListPerson p2)
                    {
                        return Convert.ToInt32(mi.Invoke(pi.GetValue(p1, null), new[] {pi.GetValue(p2, null)}));
                    });
            }
        }
        return retval;
    }
}