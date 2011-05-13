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
using Activizr.Logic.Financial;

public partial class Xml_Budgets : System.Web.UI.Page
{
    public XmlDocument xDoc = new XmlDocument();
    bool localCall = false;
    string[] allowedIP = { 
                             "66.7.199.204" //shopen
                             ,"194.14.56.34" //shopen 
                             ,"194.14.56.35" //shopen
                         };

    protected void Page_Load (object sender, EventArgs e)
    {
        Response.ContentType = "text/xml";
        try
        {
            string callingAddr = ("" + Request.ServerVariables["REMOTE_ADDR"]);
            string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
            string[] remote = (Request.ServerVariables["REMOTE_ADDR"]).Split(new char[] { '.' });
            if (local[0] == remote[0] && local[1] == remote[1])
                localCall = true;

            foreach (string addr in allowedIP)
                if (addr == callingAddr)
                    localCall = true;

            XmlElement root = xDoc.CreateElement("ROOT");
            xDoc.AppendChild(root);

            Populate(root);

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

    private void Populate (XmlElement root)
    {

        int year = DateTime.Now.Year; ;

        FinancialAccounts allAccounts = FinancialAccounts.ForOrganization(Organization.PPSE);

        foreach (FinancialAccount account in allAccounts)
        {
            if (
                ((account.AccountType == FinancialAccountType.Cost 
                    || account.AccountType == FinancialAccountType.Income) 
                && Request["type"] == null)
            ||
                (account.AccountType == FinancialAccountType.Cost && Request["type"] == "Cost") 
            ||
                (account.AccountType == FinancialAccountType.Income) && Request["type"] == "Income")
            {
                string budget = Math.Round(account.GetBudget(year)).ToString();
                string actual = Math.Round(-account.GetDelta(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1))).ToString();
                XmlElement XAccount = xDoc.CreateElement("account");
                root.AppendChild(XAccount);
                XAccount.SetAttribute("id", account.FinancialAccountId.ToString());
                XAccount.SetAttribute("name", account.Name);
                XAccount.SetAttribute("parentid", account.ParentFinancialAccountId.ToString());
                XAccount.SetAttribute("ownerid", account.OwnerPersonId.ToString());
                if (localCall)
                {
                    if (account.Owner != null)
                    {
                        XAccount.SetAttribute("ownername", account.Owner.Name);
                        XAccount.SetAttribute("ownerphone", account.Owner.Phone);
                        XAccount.SetAttribute("ownermail", account.Owner.PartyEmail);
                    }
                    XAccount.SetAttribute("actual", actual);
                }
                XAccount.SetAttribute("budget", budget);
            }
        }
    }
}
