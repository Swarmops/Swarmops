<%@ Page Language="C#" %>

<%@ Import Namespace="Activizr.Logic.Structure" %>
<%@ Import Namespace="Activizr.Logic.Pirates" %>
<%@ Import Namespace="Activizr.Basic.Enums" %>
<%@ Import Namespace="System.Collections.Generic" %>

<script runat="server" language="C#">

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

</script>

<%

    string geoString = Request.QueryString["GeographyId"];

    int geoRootId = 30;
    if (geoString != null && geoString.Length > 0)
    {
        geoRootId = Int32.Parse(geoString.Trim());
    }

    Geography geoRoot = Geography.FromIdentity(geoRootId);
    Geographies geographies = geoRoot.GetTree();

    Geographies upwardsList = new Geographies();
    while (geoRoot.GeographyId != 30)
    {
        upwardsList.Insert(0, geoRoot);
        geoRoot = geoRoot.Parent;
    }
%>
<div id="content">
    <%
        foreach (Geography geo in upwardsList)
        {
            Response.Write(PrintGeography(geo));

        }

        for (int geoIndex = 1; geoIndex < geographies.Count; geoIndex++)
        {
            Response.Write(PrintGeography(geographies[geoIndex]));
        }
    %>
</div>