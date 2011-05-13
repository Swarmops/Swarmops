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

public partial class Xml_GeographyAccess : System.Web.UI.Page
{
    public XmlDocument xDoc = new XmlDocument();
    private bool localCall = false;

    protected void Page_Load (object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";

        string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
        string[] remote = (Request.ServerVariables["REMOTE_ADDR"]).Split(new char[] { '.' });
        if (local[0] == remote[0] && local[1] == remote[1])
            localCall = true;

        int personId = 0;
        Person viewingPerson = null;

        string personStr = Request["personid"] != null ? Request["personid"].ToString() : "";
        int.TryParse(personStr, out personId);
        if (personId > 0)
        {
            viewingPerson = Person.FromIdentity(personId);
        }

        if (personId == 0 || viewingPerson == null)
        {
            XmlElement fail = xDoc.CreateElement("EPICFAIL");
            xDoc.AppendChild(fail);
            fail.InnerText = "No such Person";
        }
        else
        {

            XmlElement root = xDoc.CreateElement("ROOT");
            xDoc.AppendChild(root);

            try
            {

                Authority authority = viewingPerson.GetAuthority();

                XmlElement geoRoot = xDoc.CreateElement("GEOGRAPHIESFORORGANIZATION");
                root.AppendChild(geoRoot);
                geoRoot.SetAttribute("orgid", "1");
                geoRoot.SetAttribute("gen", "-1");
                geoRoot.SetAttribute("person", personId.ToString());

                Organization org = Organization.FromIdentity(1);//PP SE
                Geographies geoList = authority.GetGeographiesForOrganization(org);

                geoList = geoList.RemoveRedundant();
                geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));


                foreach (Geography nodeRoot in geoList)
                {
                    XmlElement parentRoot = geoRoot;
                    Geographies nodeTree = nodeRoot.GetTree();
                    XmlElement currentElem = geoRoot;
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
        }
        Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" + xDoc.OuterXml);
        Response.Write("\r\n");
    }


}
