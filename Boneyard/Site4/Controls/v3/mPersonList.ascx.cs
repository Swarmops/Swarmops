using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Pirates;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;


public partial class m_Controls_PersonList : System.Web.UI.UserControl
{
    protected void Page_Load (object sender, EventArgs e)
    {

    }
    protected void PeopleDataSource_Selecting (object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["people"] = (People)HttpContext.Current.Session["PersonList"];
    }

    protected void Grid_RowCommand (object sender, GridViewCommandEventArgs e)
    {
    }

    public People PersonList
    {
        set
        {
            HttpContext.Current.Session["PersonList"] = value; // Set for PeopleDataSource_Selecting
            Grid.DataBind();
        }
    }

    protected void Grid_RowDataBound (object sender, GridViewRowEventArgs e)
    {
        //adjust phone number to be clickable
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Person p = e.Row.DataItem as Person;

            //<a id="phoneLink" href="tel:000" runat="server">000</a>
            HtmlAnchor phone = e.Row.FindControl("phoneLink") as HtmlAnchor;

            if (phone != null)
            {
                if (Request.UserAgent.ToLower().Contains("iphone"))
                    phone.HRef = "tel:" + p.Phone.ToString().Trim();
                else
                    phone.HRef = "wtai://wp/mc;" + p.Phone.ToString().Trim();
                phone.InnerText = p.Phone.ToString();
            }

            //<a id="contLink" href="wtai://wp/ap;000;NNN">NNN</a>
            HtmlAnchor cont = e.Row.FindControl("contLink") as HtmlAnchor;

            if (cont != null)
            {
                cont.HRef = "wtai://wp/ap;" + p.Phone.ToString().Trim() + ";" + Server.UrlEncode(p.Name.ToString()).Replace("+", " ").Trim() + " pp;";
                cont.InnerText = p.Name.ToString();
            }

            //<a id="smsLink" href="sms:000">SMS</a>
            HtmlAnchor sms = e.Row.FindControl("smsLink") as HtmlAnchor;

            if (sms != null)
            {
                sms.HRef = "sms:" + p.Phone.ToString().Trim();
            }
        }
    }


}
  