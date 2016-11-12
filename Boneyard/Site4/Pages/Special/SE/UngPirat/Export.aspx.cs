using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using System.Text;
using Activizr.Database;
using Activizr.Basic.Types;
using Activizr.Logic.Support;

public partial class Pages_Special_UP_Export : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        string fileName = DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
        DateTime specificDate = DateTime.MinValue;
        DateTime yearLimit = DateTime.Today.AddYears(-1);
        if (Request["Date"] != null)
        {
            DateTime.TryParse(Request["Date"].ToString(), out specificDate);
            if (specificDate != DateTime.MinValue)
            {
                fileName = "[" + specificDate.ToString("yyyyMMdd_HHmm") + "]" + fileName;
                specificDate = specificDate.AddDays(1);
                yearLimit = specificDate.AddYears(-1);
                specificDate = specificDate.AddSeconds(-1);

            }

        }

        if (Request.Browser.Browser == "IE")
        {

            fileName = Server.UrlPathEncode(fileName);

            if (fileName != null) fileName = fileName.Replace(@"+", @"%20");

        }
        int org = Organization.UPSEid;
        Organization organization = Organization.FromIdentity(org);
        fileName = organization.NameShort + fileName;


        Response.ContentType = "text/plain";
        Response.ContentEncoding = Encoding.GetEncoding(1252);

        //Response.ContentType = "application/octet-stream";

        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\";");


        if (_authority.HasRoleAtOrganization(organization, new RoleType[] { RoleType.OrganizationChairman, RoleType.OrganizationSecretary }, Authorization.Flag.AnyGeographyExactOrganization)
            || _authority.HasRoleType(RoleType.SystemAdmin))
        {

            People ThePeople = People.GetAll().RemoveUnlisted();

            Memberships MembershipsList = null;
            if (Request["Date"] != null)
                MembershipsList = Memberships.FromArray(PirateDb.GetDatabase().GetMemberships());  // gets ALL memberships
            else
                MembershipsList = Memberships.ForOrganizations(Organization.FromIdentity(org).GetTree());

            Dictionary<int, string> GeoDistriktForGeo = new Dictionary<int, string>();
            Dictionary<int, string> OrgDistriktForOrg = new Dictionary<int, string>();


            Dictionary<int, Memberships> ActiveMemberships = new Dictionary<int, Memberships>();
            foreach (Membership ms in MembershipsList)
            {
                if (ms.OrganizationId == org || ms.Organization.Inherits(org))
                {
                    if (specificDate != DateTime.MinValue)
                    {
                        if ((ms.Active || ms.DateTerminated > specificDate)    // Rick replaced "DateTerminated.HasValue" with "Active=1"
                            && ms.Expires > specificDate
                            && ms.MemberSince <= specificDate)
                        {
                            if (!ActiveMemberships.ContainsKey(ms.PersonId))
                                ActiveMemberships.Add(ms.PersonId, new Memberships());
                            ActiveMemberships[ms.PersonId].Add(ms);
                        }
                    }
                    else if (ms.Active)
                    {
                        if (!ActiveMemberships.ContainsKey(ms.PersonId))
                            ActiveMemberships.Add(ms.PersonId, new Memberships());
                        ActiveMemberships[ms.PersonId].Add(ms);
                    }
                }
            }

            List<int> idlist = new List<int>();
            Dictionary<int, List<BasicPWEvent>> personLogs = new Dictionary<int, List<BasicPWEvent>>();
            foreach (int pId in ActiveMemberships.Keys)
            {
                idlist.Add(pId);
                if (idlist.Count > 500)
                {
                    FetchBatchOfLogs(ref specificDate, ref yearLimit, idlist, personLogs);

                    idlist = new List<int>();
                }
            }

            if (idlist.Count > 0)
            {
                FetchBatchOfLogs(ref specificDate, ref yearLimit, idlist, personLogs);
            }


            StringBuilder sb = new StringBuilder();

            sb.Append("Identity");
            sb.Append("\tName");
            sb.Append("\tEmail");
            sb.Append("\tStreet");
            sb.Append("\tPostalCode");
            sb.Append("\tCity");
            sb.Append("\tCountry.Name");
            sb.Append("\tPhone");
            sb.Append("\tBirthdate");
            sb.Append("\tGender");
            sb.Append("\tGeography.Name");
            sb.Append("\tGeography.District");
            sb.Append("\tOrganization.Name");
            sb.Append("\tOrganization.District");
            sb.Append("\tMembership.start");
            sb.Append("\tMembership.until");
            sb.Append("\tRenewed");
            sb.Append("\tType");
            sb.Append("\tby");
            sb.Append("\tfrom");
            sb.Append("\r\n");


            foreach (Person p in ThePeople)
            {
                if (ActiveMemberships.ContainsKey(p.PersonId))
                {
                    Memberships pMemberships = ActiveMemberships[p.PersonId];
                    foreach (Membership m in pMemberships)
                    {
                        if (!GeoDistriktForGeo.ContainsKey(p.GeographyId))
                        {
                            Geography currentGeoDistr = null;
                            GeoDistriktForGeo[p.GeographyId] = "N/A";
                            if (p.GeographyId != 0)
                            {
                                try
                                {
                                    Geography previous = p.Geography;

                                    if (p.Geography.ParentGeographyId != 0)
                                        currentGeoDistr = p.Geography.Parent;

                                    while (currentGeoDistr != null
                                        && currentGeoDistr.Identity != 0
                                        && currentGeoDistr.Identity != Geography.SwedenId
                                        && currentGeoDistr.Identity != Geography.RootIdentity)
                                    {
                                        previous = currentGeoDistr;
                                        if (currentGeoDistr.ParentGeographyId == 0)
                                            currentGeoDistr = null;
                                        else
                                            currentGeoDistr = currentGeoDistr.Parent;
                                    }
                                    if (previous != null)
                                        GeoDistriktForGeo[p.GeographyId] = previous.Name;
                                }
                                catch (Exception ex)
                                {
                                    GeoDistriktForGeo[p.GeographyId] = "n/a";
                                }
                            }
                        }

                        if (!OrgDistriktForOrg.ContainsKey(m.OrganizationId))
                        {
                            Organization currentOrgDistr = null;
                            OrgDistriktForOrg[m.OrganizationId] = "N/A";

                            Organization previous = m.Organization;
                            currentOrgDistr = m.Organization.Parent; ;

                            while (currentOrgDistr != null
                                && currentOrgDistr.Identity != 0
                                && currentOrgDistr.Identity != org
                                && currentOrgDistr.Identity != Organization.RootIdentity)
                            {
                                previous = currentOrgDistr;
                                if (currentOrgDistr.ParentOrganizationId == 0)
                                    currentOrgDistr = null;
                                else
                                    currentOrgDistr = currentOrgDistr.Parent;
                            }
                            if (previous != null)
                                OrgDistriktForOrg[m.OrganizationId] = previous.NameShort;
                        }

                        sb.Append(p.Identity);
                        sb.Append("\t" + p.Name.Replace("\t", ""));
                        sb.Append("\t" + p.Email.Replace("\t", ""));
                        sb.Append("\t" + p.Street.Replace("\t", ""));
                        sb.Append("\t" + p.PostalCode.Replace("\t", ""));
                        sb.Append("\t" + p.CityName.Replace("\t", ""));
                        sb.Append("\t" + p.Country.Name.Replace("\t", ""));
                        sb.Append("\t" + p.Phone.Replace("\t", ""));
                        sb.Append("\t" + p.Birthdate);
                        sb.Append("\t" + p.Gender);
                        sb.Append("\t" + p.Geography.Name);
                        sb.Append("\t" + GeoDistriktForGeo[p.GeographyId]);
                        sb.Append("\t" + m.Organization.Name);
                        sb.Append("\t" + OrgDistriktForOrg[m.OrganizationId]);
                        sb.Append("\t" + m.MemberSince);
                        if (!m.Active)
                            sb.Append("\t" + m.DateTerminated);
                        else
                            sb.Append("\t" + m.Expires);

                        if (personLogs.ContainsKey(p.Identity))
                        {
                            bool found = false;
                            foreach (BasicPWEvent log in personLogs[p.Identity])
                            {
                                if (log.OrganizationId == m.OrganizationId)
                                {
                                    sb.Append("\t" + log.DateTime);
                                    sb.Append("\t" + log.EventType);
                                    sb.Append("\t" + log.ActingPersonId);
                                    sb.Append("\t" + log.ParameterText);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                sb.Append("\t\t\t\t");
                            }
                        }
                        else
                        {
                            sb.Append("\t\t\t\t");
                        }
                        sb.Append("\r\n");
                    }
                }
            }

            Response.Write(sb.ToString());
        }
    }

    private static void FetchBatchOfLogs (ref DateTime specificDate, ref DateTime yearLimit, List<int> idlist, Dictionary<int, List<BasicPWEvent>> personLogs)
    {
        Dictionary<int, List<BasicPWEvent>> eventRecs = PWEvents.ForPersons(idlist.ToArray(),
                                new EventType[] { EventType.AddedMember, EventType.AddedMembership, EventType.ExtendedMembership });
        List<BasicPWEvent> recs = null;
        foreach (List<BasicPWEvent> eventList in eventRecs.Values)
        {
            recs = new List<BasicPWEvent>();
            foreach (BasicPWEvent evtRec in eventList)
            {
                if (evtRec.DateTime >= yearLimit.AddMonths(-1) && evtRec.DateTime < yearLimit.AddYears(1))
                {
                    int organizationId = 0;
                    string parameterText = "";

                    switch (evtRec.EventType)
                    {
                        case EventType.AddedMember:
                            {
                                string[] split1 = ("" + evtRec.ParameterText).Split(',');
                                if (split1.Length < 2)
                                {
                                    split1 = new string[2];
                                    split1[1] = FindIPForEventInLog(evtRec.DateTime, evtRec.AffectedPersonId, evtRec.ActingPersonId);
                                }
                                if (!string.IsNullOrEmpty(split1[0]))
                                {
                                    string[] split2 = ("" + split1[0]).Split(' ');
                                    foreach (string orgnr in split2)
                                    {
                                        if (int.Parse(orgnr) != evtRec.OrganizationId)
                                        {
                                            organizationId = int.Parse(orgnr);
                                            parameterText = split1[0];
                                            BasicPWEvent addR = new BasicPWEvent(
                                                evtRec.EventId, evtRec.DateTime, evtRec.Open, evtRec.ProcessedDateTime, evtRec.EventType,
                                                evtRec.EventSource, evtRec.ActingPersonId, evtRec.AffectedPersonId, organizationId,
                                                evtRec.GeographyId, evtRec.ParameterInt, parameterText);

                                            recs.Add(addR);

                                        }
                                    }
                                }
                                parameterText = split1[1];
                                BasicPWEvent addR2 = new BasicPWEvent(
                                    evtRec.EventId, evtRec.DateTime, evtRec.Open, evtRec.ProcessedDateTime, evtRec.EventType,
                                    evtRec.EventSource, evtRec.ActingPersonId, evtRec.AffectedPersonId, evtRec.OrganizationId,
                                    evtRec.GeographyId, evtRec.ParameterInt, parameterText);
                                recs.Add(addR2);
                            }
                            break;


                        case EventType.AddedMembership:
                            {
                                parameterText = FindIPForEventInLog(evtRec.DateTime, evtRec.AffectedPersonId, evtRec.ActingPersonId);
                                BasicPWEvent addR2 = new BasicPWEvent(
                                    evtRec.EventId, evtRec.DateTime, evtRec.Open, evtRec.ProcessedDateTime, evtRec.EventType,
                                    evtRec.EventSource, evtRec.ActingPersonId, evtRec.AffectedPersonId, evtRec.OrganizationId,
                                    evtRec.GeographyId, evtRec.ParameterInt, parameterText);
                                recs.Add(addR2);
                            }
                            break;


                        case EventType.ExtendedMembership:
                            {
                                string[] split1 = ("" + evtRec.ParameterText).Split(' ');
                                if (split1.Length < 1 || split1[0].Trim() == "")
                                {
                                    if (split1.Length < 1)
                                        split1 = new string[1];
                                    split1[0] = FindIPForEventInLog(evtRec.DateTime, evtRec.AffectedPersonId, evtRec.ActingPersonId);
                                }
                                if (split1.Length > 1)
                                {
                                    foreach (string orgnr in split1)
                                    {
                                        if (orgnr.Length < 7 && int.Parse(orgnr) != evtRec.OrganizationId)
                                        {
                                            organizationId = int.Parse(orgnr);
                                            parameterText = split1[0];
                                            BasicPWEvent addR = new BasicPWEvent(
                                                evtRec.EventId, evtRec.DateTime, evtRec.Open, evtRec.ProcessedDateTime, evtRec.EventType,
                                                evtRec.EventSource, evtRec.ActingPersonId, evtRec.AffectedPersonId, organizationId,
                                                evtRec.GeographyId, evtRec.ParameterInt, parameterText);

                                            recs.Add(addR);
                                        }
                                    }
                                }
                                parameterText = split1[0];
                                BasicPWEvent addR2 = new BasicPWEvent(
                                    evtRec.EventId, evtRec.DateTime, evtRec.Open, evtRec.ProcessedDateTime, evtRec.EventType,
                                    evtRec.EventSource, evtRec.ActingPersonId, evtRec.AffectedPersonId, evtRec.OrganizationId,
                                    evtRec.GeographyId, evtRec.ParameterInt, parameterText);
                                recs.Add(addR2);
                            }
                            break;
                    }
                }
            }
            if (recs.Count > 0)
                personLogs[recs[0].AffectedPersonId] = recs;
        }
    }
    /// <summary>
    /// Find a log entry in PWLog that corresponds to the event and return the IP address from it
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="affectedPersonId"></param>
    /// <param name="actingPersonId"></param>
    /// <returns></returns>
    private static string FindIPForEventInLog (DateTime dateTime, int affectedPersonId, int actingPersonId)
    {
        BasicPWLog[] logArr = PWLog.GetLatestEvents(PWLogItem.Person, DateTime.Now, new int[] { affectedPersonId }, new PWLogAction[] { PWLogAction.MemberAdd, PWLogAction.MemberJoin, PWLogAction.MembershipAdd, PWLogAction.MembershipRenewed });
        foreach (BasicPWLog log in logArr)
        {
            if (log.AffectedItemId == affectedPersonId
                && log.ActingPersonId == actingPersonId
                && !string.IsNullOrEmpty(log.IpAddress)
                && Math.Abs(log.DateTimeUtc.Subtract(dateTime.ToUniversalTime()).TotalMinutes) < 140)
            {
                return log.IpAddress;
            }
        }
        foreach (BasicPWLog log in logArr)
        {
            if (log.AffectedItemId == affectedPersonId
                && !string.IsNullOrEmpty(log.IpAddress)
                && Math.Abs(log.DateTimeUtc.Subtract(dateTime.ToUniversalTime()).TotalMinutes) < 122)
            {
                return log.IpAddress;
            }
        }
        return "";
    }


}
