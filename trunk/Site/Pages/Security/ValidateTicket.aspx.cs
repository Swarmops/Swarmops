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
using PirateWeb.Logic.Structure;
using PirateWeb.Basic.Types;
using PirateWeb.Basic.Enums;
using PirateWeb.Logic.Pirates;
using PirateWeb.Logic.Security;

public partial class Xml_Roles : System.Web.UI.Page
{
    public XmlDocument xDoc = new XmlDocument();
    bool authorizedCall = false;
    static Dictionary<string, string> allowedExternalIPs = initAllowedTable();

    private static Dictionary<string, string> initAllowedTable ()
    {
        Dictionary<string, string> retval = new Dictionary<string, string>();
        //retval.Add("81.27.1.77", "Gert Löwenhamn, Stockholm, röstningsapplikation för DL-val 2009");
        retval.Add("85.24.181.147", "För SSO mot Pinax, utveckling (D Nyström)");
        retval.Add("79.136.47.72", "För SSO mot Pinax, utveckling  (D Nyström)");
        retval.Add("88.87.60.45", "För test av Referendum  (Egil Möller)");
        return retval;
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";
        Response.Expires = -1;
        bool validationOK = false;

        int personId = 0;

        string ticket = Request["ticket"] != null ? Request["ticket"].ToString() : "";

        //Check that call is originating from an address that is allowed

        string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
        string[] remote = (Request.ServerVariables["REMOTE_ADDR"]).Split(new char[] { '.' });

        if (local[0] == remote[0] && local[1] == remote[1])
            authorizedCall = true;

        if (allowedExternalIPs.ContainsKey(Request.ServerVariables["REMOTE_ADDR"].ToString()))
            authorizedCall = true;



        //Check that the ticket is among the issued ones.

        Dictionary<string, InternalLoginTicket> openTickets = new Dictionary<string, InternalLoginTicket>();

        if (Application["SubSystem_LoginTickets"] != null)
        {
            Application.Lock();

            try
            {
                openTickets = (Dictionary<string, InternalLoginTicket>)Application["SubSystem_LoginTickets"];
            }
            catch (Exception)
            {
            }


            //Remove expired ones

            List<string> toremove = new List<string>();
            foreach (string key in openTickets.Keys)
            {
                if (openTickets[key].created.AddMinutes(2) < DateTime.Now)
                    toremove.Add(key);
            }

            foreach (string key in toremove)
            {
                openTickets.Remove(key);
            }

            //Check the current ticket
            if (openTickets.ContainsKey(ticket))
            {
                validationOK = true;
                personId = openTickets[ticket].validatedUserID;
                openTickets.Remove(ticket);
            }
            Application["SubSystem_LoginTickets"] = openTickets;
            Application.UnLock();

        }


        if (!validationOK)
        {
            XmlElement fail = xDoc.CreateElement("EPICFAIL");
            xDoc.AppendChild(fail);
            fail.InnerText = "Ticket not found";
        }
        else
        {

            XmlElement root = xDoc.CreateElement("ROOT");
            xDoc.AppendChild(root);

            XmlElement xPerson = xDoc.CreateElement("USER");
            root.AppendChild(xPerson);
            try
            {
                Person viewingPerson = Person.FromIdentity(personId);

                XmlElement xElem = xDoc.CreateElement("ID");
                xPerson.AppendChild(xElem);
                xElem.InnerText = viewingPerson.Identity.ToString();

                xElem = xDoc.CreateElement("NAME");
                xPerson.AppendChild(xElem);
                xElem.InnerText = authorizedCall ? viewingPerson.Name : "Call from external IP";

                xElem = xDoc.CreateElement("EMAIL");
                xPerson.AppendChild(xElem);

                if (string.IsNullOrEmpty(viewingPerson.PartyEmail))
                {
                    xElem.InnerText = authorizedCall ? viewingPerson.Email : "some info hidden";
                }
                else
                {
                    xElem.InnerText = authorizedCall ? viewingPerson.PartyEmail : "some info hidden";
                }

                xElem = xDoc.CreateElement("HANDLE");
                xPerson.AppendChild(xElem);
                if (string.IsNullOrEmpty(viewingPerson.Handle))
                {
                    xElem.InnerText = "";
                }
                else
                {
                    xElem.InnerText = authorizedCall ? viewingPerson.Handle : "some info hidden";
                }

                xElem = xDoc.CreateElement("PHONE");
                xPerson.AppendChild(xElem);
                if (string.IsNullOrEmpty(viewingPerson.Phone))
                {
                    xElem.InnerText = "";
                }
                else
                {
                    xElem.InnerText = authorizedCall ? viewingPerson.Phone : "for security reasons";
                }

                xElem = xDoc.CreateElement("GEOGRAPHIESFORPERSON");
                xPerson.AppendChild(xElem);
                Geographies line = viewingPerson.Geography.GetLine();
                Geography[] linearr = line.ToArray();
                Array.Reverse(linearr);

                foreach (Geography g in linearr)
                {
                    XmlElement currentElem = xDoc.CreateElement("GEOGRAPHY");
                    xElem.AppendChild(currentElem);
                    xElem = currentElem;

                    currentElem.SetAttribute("geographyid", g.Identity.ToString());
                    currentElem.SetAttribute("name", g.Name);
                }

                xElem = xDoc.CreateElement("MEMBERSHIPS");
                xPerson.AppendChild(xElem);
                if (authorizedCall)
                {
                    Memberships memberships = viewingPerson.GetMemberships();
                    foreach (PirateWeb.Logic.Pirates.Membership ms in memberships)
                    {
                        XmlElement currentElem = xDoc.CreateElement("MEMBERSHIP");
                        xElem.AppendChild(currentElem);

                        currentElem.SetAttribute("orgid", ms.OrganizationId.ToString());
                        currentElem.SetAttribute("name", ms.Organization.Name);
                        currentElem.SetAttribute("until", ms.Expires.ToString("yyyy-MM-dd"));
                        currentElem.SetAttribute("paymentstatus", ms.PaymentStatus.ToString());
                    }
                }
                else
                {
                    XmlElement currentElem = xDoc.CreateElement("MEMBERSHIP");
                    xElem.AppendChild(currentElem);

                    currentElem.SetAttribute("orgid", "1");
                    currentElem.SetAttribute("name", "Dummy org PPSE, external call always member");
                    currentElem.SetAttribute("until", DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd"));
                    currentElem.SetAttribute("paymentstatus", "PaymentRecieved");
                }

                try
                {

                    Authority authority = viewingPerson.GetAuthority();

                    XmlElement geoRoot = xDoc.CreateElement("GEOGRAPHIESFORORGANIZATION");
                    root.AppendChild(geoRoot);
                    geoRoot.SetAttribute("orgid", Organization.PPSEid.ToString());
                    geoRoot.SetAttribute("gen", "-1");

                    Organization org = Organization.FromIdentity(Organization.PPSEid);//PP SE
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
                catch (Exception)
                {
                    //Failed in trying to list roles
                }
            }
            catch (Exception ex)
            {
                XmlElement fail = xDoc.CreateElement("EPICFAIL");
                xDoc.AppendChild(fail);
                fail.InnerText = "Exception thrown:" + ex.Message;

            }
        }
        Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n" + xDoc.OuterXml);
        Response.Write("\r\n");
    }


}
