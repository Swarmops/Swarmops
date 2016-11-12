using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Structure;
using Activizr.Logic.Pirates;
using System.Text;
using Activizr.Basic.Enums;

public partial class Pages_v4_v3_GeographyOfficers : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        divContent.InnerHtml = "";
        string geoString = Request.QueryString["GeographyId"];
        int geoRootId = 0;
        if (geoString != null)
            int.TryParse(geoString.Trim(), out geoRootId);
        Geography geoRoot;

        if (geoRootId == 0)
        {
            Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
            geoRoot = viewingPerson.Geography;
        }
        else
        {
            geoRoot = Geography.FromIdentity(geoRootId);

        }

        Geographies geographies = geoRoot.GetTree();


        Geographies upwardsList = new Geographies();
        while (geoRoot.GeographyId != Geography.SwedenId)
        {
            upwardsList.Insert(0, geoRoot);
            geoRoot = geoRoot.Parent;
        }

        string toPrint = "";
        if (geoRootId == Geography.SwedenId)
        {
            string cacheDataKey = "SwedenContactList";
            toPrint = (string)Cache.Get(cacheDataKey);
            if (toPrint == null)
            {

                toPrint = buildOutput(upwardsList, geographies);
                Cache.Insert(cacheDataKey, toPrint, null, DateTime.UtcNow.AddMinutes(15),
                     System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                divContent.InnerHtml += (Server.HtmlEncode("Notera! Av belastnings-skäl kan denna lista vara upp till 15 minuter gammal.") + "<br />");
            }
        }
        else
        {
            toPrint = buildOutput(upwardsList, geographies);
        }

        divContent.InnerHtml += toPrint;

    }


    string PrintGeography (Geography geo)
    {
        StringBuilder sbl = new StringBuilder();
        RoleType[] rolesToShow = new RoleType[] { RoleType.OrganizationChairman, RoleType.OrganizationVice1, RoleType.OrganizationSecretary, RoleType.LocalLead, RoleType.LocalDeputy };
        Organizations orgs = Organizations.GetOrganizationsAvailableAtGeography(geo.Identity);
        foreach (Organization org in orgs)
        {
            Dictionary<int, bool> listedPersons = new Dictionary<int, bool>();
            if (org.IsOrInherits(Organization.UPSEid))
            {
                if (org.AnchorGeographyId == geo.Identity || org.UptakeGeographies.Contains(geo))
                {
                    RoleLookup officers = RoleLookup.FromOrganization(org);
                    bool foundRole = false;
                    foreach (RoleType rt in rolesToShow)
                    {
                        foundRole |= officers[rt].Count > 0;
                        if (foundRole) break;
                    }

                    if (foundRole)
                    {
                        sbl.Append("<br><br><b>" + HttpUtility.HtmlEncode(org.Name) + ":</b>");
                        foreach (RoleType rt in rolesToShow)
                            foreach (Activizr.Logic.Pirates.PersonRole r in officers[rt])
                            {
                                if (!listedPersons.ContainsKey(r.PersonId))
                                {
                                    sbl.Append(PrintOfficer(r));
                                    listedPersons[r.PersonId] = true;
                                }
                            }

                        sbl.Append("<br>");
                    }
                }
            }
            else
            {
                RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo, org);

                if (officers[RoleType.LocalLead].Count > 0 || officers[RoleType.LocalDeputy].Count > 0)
                {
                    sbl.Append("<br><br><b>" + HttpUtility.HtmlEncode(org.Name) + ", " + HttpUtility.HtmlEncode(geo.Name) + ":</b>");
                    foreach (RoleType rt in rolesToShow)
                        foreach (Activizr.Logic.Pirates.PersonRole r in officers[rt])
                            if (!listedPersons.ContainsKey(r.PersonId))
                            {
                                sbl.Append(PrintOfficer(r));
                                listedPersons[r.PersonId] = true;
                            }

                    sbl.Append("<br>");
                }
            }
        }
        return sbl.ToString();
    }

    string PrintOfficer (Activizr.Logic.Pirates.PersonRole r)
    {
        string result = "";
        result += ("<br>&nbsp;&nbsp;&nbsp;");
        string roleName = "" + GetLocalResourceObject(r.Type.ToString());
        if ("" + roleName == "")
            roleName = r.Type.ToString();
        result += (roleName + ": ");
        if (r.Organization.IsOrInherits(Organization.UPSEid))
        {
            result += ("<a href=\"mailto:" + r.Person.PartyEmail.Replace("piratpartiet.se", "ungpirat.se") + "\">" + HttpUtility.HtmlEncode(r.Person.Name) + "</a>");
        }
        else
        {
            result += ("<a href=\"mailto:" + r.Person.PartyEmail + "\">" + HttpUtility.HtmlEncode(r.Person.Name) + "</a>");
        }
        result += (", telefon: " + r.Person.Phone);
        return result;
    }


    string buildOutput (Geographies upwardsList, Geographies geographies)
    {
        StringBuilder sb = new StringBuilder();
        foreach (Geography geo in upwardsList)
        {
            sb.Append(PrintGeography(geo));

        }

        for (int geoIndex = 1; geoIndex < geographies.Count; geoIndex++)
        {
            sb.Append(PrintGeography(geographies[geoIndex]));
        }
        return sb.ToString();
    }



}
