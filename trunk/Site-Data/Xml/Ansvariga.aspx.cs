using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Activizr.Logic.Structure;
using Activizr.Basic.Types;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;

public partial class Xml_Ansvariga : System.Web.UI.Page
{
    public XmlDocument xDoc = new XmlDocument();
    bool localCall = false;

    protected void Page_Load (object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";
        try
        {
            string cacheDataKey = "allGeographies";

            string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
            string[] remote = (Request.ServerVariables["REMOTE_ADDR"]).Split(new char[] { '.' });
            if (local[0] == remote[0] && local[1] == remote[1])
                localCall = true;

            Geography geography = null;
            string geoString = Request.QueryString["GeographyId"] != null ? Request.QueryString["GeographyId"].ToString() : "-1";
            string geoName = Request.QueryString["GeographyName"] != null ? Request.QueryString["GeographyName"].ToString() : "-1";
            if (geoName != "")
            {
                Geographies allGeographies = (Geographies)Cache.Get(cacheDataKey);

                if (allGeographies == null || geoName == "reload")
                {
                    allGeographies = Geography.Root.GetTree();
                    Cache.Insert(cacheDataKey, allGeographies, null, DateTime.Today.AddHours(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                foreach (Geography g in allGeographies)
                {
                    if (g.Name.ToLower().Trim().Replace("s ", "").Replace(" ", "") == geoName.ToLower().Trim().Replace("s ", "").Replace(" ", "")
                        || g.Name.ToLower().Trim().Replace(" ", "") == geoName.ToLower().Trim().Replace(" ", ""))
                    {
                        geography = g;
                        break;
                    }
                }
                if (geography == null)
                {
                    Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<EPICFAIL>No geography with name:'" + geoName + "'.</EPICFAIL>\r\n");
                    Session.Abandon();
                    Response.End();
                }

            }
            else
            {
                int geoRootId = Int32.Parse(geoString.Trim());
                if (geoRootId < 0)
                {
                    Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<EPICFAIL>No geographyId given.</EPICFAIL>\r\n");
                    Session.Abandon();
                    Response.End();
                }
                geography = Geography.FromIdentity(geoRootId);
            }


            XmlElement root = xDoc.CreateElement("ROOT");
            xDoc.AppendChild(root);


            int totOfficers = PrintGeography(root, geography, 1);

            root.SetAttribute("totcount", totOfficers.ToString());
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" + xDoc.OuterXml);
            Response.Write("\r\n");
            Session.Abandon();
        }
        catch (Exception e1)
        {
            Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<EPICFAIL>Exception was thrown. '" + Server.HtmlEncode(e1.Message) + "'</EPICFAIL>\r\n");
            Session.Abandon();
            Response.End();
        }

    }


    int PrintGeography (XmlElement currentNode, Geography geo, int level)
    {

        XmlElement thisNode = currentNode.OwnerDocument.CreateElement("GEOGRAPHY");

        currentNode.AppendChild(thisNode);

        thisNode.SetAttribute("name", geo.Name);
        thisNode.SetAttribute("geographyid", geo.Identity.ToString());

        RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, 1);

        int totOfficers = GenerateOfficers(thisNode, officers);

        Geography supGeo = geo.Parent;
        if (supGeo != null && supGeo.GeographyId != Geography.SwedenId)
        {
            totOfficers += PrintGeography(currentNode, supGeo, level + 1);
        }
        return totOfficers;
    }

    private int GenerateOfficers (XmlElement thisNode, RoleLookup officers)
    {
        int tot = 0;
        Dictionary<int, bool> dupCheck = new Dictionary<int, bool>();
        if (officers[RoleType.LocalLead].Count > 0)
        {
            foreach (PersonRole r in officers[RoleType.LocalLead])
            {
                if (!dupCheck.ContainsKey(r.Person.Identity))
                {
                    ++tot;
                    dupCheck[r.Person.Identity] = true;
                    GeneratePerson(thisNode, RoleType.LocalLead, r.Person);
                }
            }
        }

        if (officers[RoleType.LocalDeputy].Count > 0)
        {
            foreach (PersonRole r in officers[RoleType.LocalDeputy])
            {
                if (!dupCheck.ContainsKey(r.Person.Identity))
                {
                    ++tot;
                    dupCheck[r.Person.Identity] = true;
                    GeneratePerson(thisNode, RoleType.LocalDeputy, r.Person);
                }
            }
        }

        if (officers[RoleType.LocalAdmin].Count > 0)
        {
            foreach (PersonRole r in officers[RoleType.LocalAdmin])
            {
                if (!dupCheck.ContainsKey(r.Person.Identity))
                {
                    ++tot;
                    dupCheck[r.Person.Identity] = true;
                    GeneratePerson(thisNode, RoleType.LocalAdmin, r.Person);
                }
            }
        }
        return tot;
    }

    private void GeneratePerson (XmlElement thisNode, RoleType role, Person lead)
    {
        XmlElement officerNode = thisNode.OwnerDocument.CreateElement("OFFICER");
        thisNode.AppendChild(officerNode);

        officerNode.SetAttribute("personid", lead.PersonId.ToString());
        officerNode.SetAttribute("type", role.ToString());
        if (localCall)
        {
            officerNode.SetAttribute("email", lead.PartyEmail);
            officerNode.SetAttribute("phone", lead.Phone);
            officerNode.InnerText = lead.Name;
        }
        else
        {
            officerNode.SetAttribute("email", "some.address@piratpartiet.se");
            officerNode.SetAttribute("phone", "12345678");
            officerNode.InnerText = "Hidden for remote call(" + lead.PersonId.ToString() + ")";
        }
    }

}
