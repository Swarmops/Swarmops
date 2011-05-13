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

public partial class Xml_ValbokData : System.Web.UI.Page
{
    public XmlDocument xDoc = new XmlDocument();
    bool localCall = false;
    public Dictionary<string, string> valkretsbyte = new Dictionary<string, string>();

    protected void Page_Load (object sender, EventArgs e)
    {
        string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
        string[] remote = (Request.ServerVariables["REMOTE_ADDR"]).Split(new char[] { '.' });
        if (local[0] == remote[0] && local[1] == remote[1])
            localCall = true;
        SetupValkretsCodes();

        string geoString = Request.QueryString["GeographyId"];

        int geoRootId = Geography.SwedenId;
        if (geoString != null && geoString.Length > 0)
        {
            geoRootId = Int32.Parse(geoString.Trim());
        }

        Geography geography = Geography.FromIdentity(geoRootId);

        XmlElement root = xDoc.CreateElement("ROOT");
        xDoc.AppendChild(root);

        Response.ContentType = "text/xml";

        PrintGeography(root, geography,1);
        Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" + xDoc.OuterXml);
        Response.Write("\r\n");

    }

    private void SetupValkretsCodes ()
    {
        valkretsbyte.Add("1", "1");
        valkretsbyte.Add("2N", "1");
        valkretsbyte.Add("2S", "1");
        valkretsbyte.Add("3", "3");
        valkretsbyte.Add("4", "4");
        valkretsbyte.Add("5", "5");
        valkretsbyte.Add("6", "6");
        valkretsbyte.Add("7", "7");
        valkretsbyte.Add("8", "8");
        valkretsbyte.Add("9", "9");
        valkretsbyte.Add("10", "10");
        valkretsbyte.Add("11", "12");
        valkretsbyte.Add("12", "12");
        valkretsbyte.Add("13", "12");
        valkretsbyte.Add("14", "12");
        valkretsbyte.Add("15", "13");
        valkretsbyte.Add("16", "14");
        valkretsbyte.Add("17", "14");
        valkretsbyte.Add("18", "14");
        valkretsbyte.Add("19", "14");
        valkretsbyte.Add("20", "14");
        valkretsbyte.Add("21", "17");
        valkretsbyte.Add("22", "18");
        valkretsbyte.Add("23", "19");
        valkretsbyte.Add("24", "20");
        valkretsbyte.Add("25", "21");
        valkretsbyte.Add("26", "22");
        valkretsbyte.Add("27", "23");
        valkretsbyte.Add("28", "24");
        valkretsbyte.Add("29", "25");
    }

    void PrintGeography (XmlElement currentNode, Geography geo, int level)
    {

        BasicGeographyDesignation[] officialdesignations = geo.GetGeographyDesignations();
        XmlElement thisNode = currentNode;
        if (officialdesignations.Length == 0)
        {
            thisNode = currentNode.OwnerDocument.CreateElement("GEOGRAPHY");
        }
        else if (officialdesignations[0].GeographyLevel == GeographyLevel.Municipality)
        {
            thisNode = currentNode.OwnerDocument.CreateElement("GEOGRAPHY");
        }
        else if (officialdesignations[0].GeographyLevel == GeographyLevel.ElectoralCircuit)
        {
            thisNode = currentNode.OwnerDocument.CreateElement("GEOGRAPHY");
        }

        if (thisNode != currentNode)
        {
            currentNode.AppendChild(thisNode);

            RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, 1);
            thisNode.SetAttribute("name", geo.Name);
            thisNode.SetAttribute("hierlevel", level.ToString());
            thisNode.SetAttribute("geographyid", geo.Identity.ToString());
            if (officialdesignations.Length > 0)
            {
                thisNode.SetAttribute("level", ((int)officialdesignations[0].GeographyLevel).ToString());
                if (valkretsbyte.ContainsKey(officialdesignations[0].Designation))
                    thisNode.SetAttribute("key", valkretsbyte[officialdesignations[0].Designation]);
            }


            if (officers[RoleType.LocalLead].Count > 0)
            {
                foreach (PersonRole r in officers[RoleType.LocalLead])
                {
                    GeneratePerson(thisNode, RoleType.LocalLead, r.Person);
                }

            }
            if (officers[RoleType.LocalDeputy].Count > 0)
            {
                foreach (PersonRole r in officers[RoleType.LocalDeputy])
                {
                    GeneratePerson(thisNode, RoleType.LocalDeputy, r.Person);
                }
            }
        }

        foreach (Geography subGeo in geo.Children)
        {
            PrintGeography(thisNode, subGeo, level+1);
        }

    }

    private void GeneratePerson (XmlElement thisNode, RoleType role, Person lead)
    {
        XmlElement officerNode = thisNode.OwnerDocument.CreateElement(role == RoleType.LocalLead ? "OFFICER1" : "OFFICER2");
        thisNode.AppendChild(officerNode);

        officerNode.SetAttribute("personid", lead.PersonId.ToString());
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
