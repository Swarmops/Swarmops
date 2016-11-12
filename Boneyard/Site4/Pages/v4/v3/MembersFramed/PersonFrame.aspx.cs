using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Pirates;

public partial class Pages_v4_v3_Members_PersonFrame : PageV4Base
{
    protected string currentId = "";
    protected string urlArray = "";
    protected void Page_Load (object sender, EventArgs e)
    {
        currentId = "" + Request["id"];
        Dictionary<string, string> urls = new Dictionary<string, string>();
        urls["BasicDetails.aspx"] = this.GetLocalResourceObject("BasicDetails.aspx").ToString();
        urls["MembershipSettings.aspx"] = this.GetLocalResourceObject("MembershipSettings.aspx").ToString();
        urls["Memberships.aspx"] = this.GetLocalResourceObject("Memberships.aspx").ToString();
        urls["RolesResponsibilities.aspx"] = this.GetLocalResourceObject("RolesResponsibilities.aspx").ToString();
        urls["SubscriptionSettings.aspx"] = this.GetLocalResourceObject("SubscriptionSettings.aspx").ToString();

        string id = "" + Request["id"];

        if (id == "") id = "0";

        string html1 = "";
        string html2 = "";

        int selectedTab = 0;
        if (Request.Cookies["pwPersonFrame_selectedTab"] != null)
        {
            int.TryParse(Request.Cookies["pwPersonFrame_selectedTab"].Value.ToString(), out selectedTab);
        }

        int i = 0;
        foreach (string url in urls.Keys)
        {
            urlArray += ",'" + url + "'";

            html1 += "<li><a href='#tabs-" + i + "'>" + urls[url] + "</a></li>\r\n";
            html2 += "<div id='tabs-" + i + "' class='tabFrame'><iframe name='frame" + i + "' id='Iframe" + i + "' src='";
            if (selectedTab ==  i)
                html2 += url + "?id=" + id;
            else
                html2 += "empty.htm";
            html2 += "' frameborder='0'></iframe></div>\r\n";
            ++i;
        }
        Response.Cookies.Add(new HttpCookie("pwPersonFrame_selectedTab"));
        Response.Cookies["pwPersonFrame_selectedTab"].Value = selectedTab.ToString();

        urlArray = urlArray.Substring(1);
        tabs.InnerHtml = "<div id='tabs'><ul>\r\n" + html1 + "</ul>\r\n" + html2 + "</div>";
        LabelSelectedMember.Text = "";
        if (id != "0")
        {
            int personID = Convert.ToInt32(id);
            Person selected = Person.FromIdentity(personID);
            LabelSelectedMember.Text = "#" + personID + " " + selected.Name;
        }
    }
}
