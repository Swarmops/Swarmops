using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using Activizr.Logic.Structure;
using Activizr.Basic.Types;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;

public partial class Xml_Roles : System.Web.UI.Page
{
    public XmlDocument xDoc = new XmlDocument();
    bool localCall = false;

    protected void Page_Load (object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";

        string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
        string[] remote = (Request.ServerVariables["REMOTE_ADDR"]).Split(new char[] { '.' });
        if (local[0] == remote[0] && local[1] == remote[1])
            localCall = true;

        string personIDString = Request["personid"] != null ? Request["personid"].ToString() : "";

        string geoString = Request.QueryString["GeographyId"];

        int geoRootId = Geography.SwedenId;
        if (geoString != null && geoString.Length > 0)
        {
            geoRootId = Int32.Parse(geoString.Trim());
        }

        Geography geography = Geography.FromIdentity(geoRootId);

        XmlElement root = xDoc.CreateElement("ROOT");
        root.SetAttribute("gen", "-1");
        xDoc.AppendChild(root);
        try
        {
            Person viewingPerson = Person.FromIdentity(int.Parse(personIDString));
            Authority authority = viewingPerson.GetAuthority();

            Organization org = Organization.FromIdentity(1);//PP SE
            Geographies geoList = authority.GetGeographiesForOrganization(org);

            geoList = geoList.RemoveRedundant();
            geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));


            foreach (Geography nodeRoot in geoList)
            {
                XmlElement parentRoot = root;
                Geographies nodeTree = nodeRoot.GetTree();
                XmlElement currentElem = root;
                int prevGen = -1;
                foreach (Geography node in nodeTree)
                {
                    int parentLevel = int.Parse(parentRoot.GetAttribute("gen"));
                    if (node.Generation > prevGen)
                    {
                        parentRoot = currentElem;
                        parentLevel = prevGen;
                    }
                    else if (node.Generation < prevGen)
                    {
                        parentRoot = (XmlElement)currentElem;
                        parentLevel = int.Parse(parentRoot.GetAttribute("gen"));
                        while (parentLevel > -1 && parentLevel >= node.Generation)
                        {
                            parentRoot = (XmlElement)parentRoot.ParentNode;
                            parentLevel = int.Parse(parentRoot.GetAttribute("gen"));
                        }
                    }

                    prevGen = node.Generation;

                    currentElem = xDoc.CreateElement("GEOGRAPHY");
                    parentRoot.AppendChild(currentElem);

                    currentElem.SetAttribute("geographyid", node.Identity.ToString());
                    currentElem.SetAttribute("name", node.Name);
                    currentElem.SetAttribute("gen", node.Generation.ToString());

                }
            }
        }
        catch { }

        Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" + xDoc.OuterXml);
        Response.Write("\r\n");
    }




}
