using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class Controls_v4_PageTitle : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.LabelPageDescription.Text = description + ".";
        this.LabelPageTitle.Text = title.ToUpper();

        if (File.Exists(Server.MapPath("~/Images/Public/Silk/" + icon)))
        {
            this.IconPage.ImageUrl = "~/Images/Public/Silk/" + icon;
        }
        else if (File.Exists(Server.MapPath("~/Images/Public/Fugue/icons-shadowless/" + icon)))
        {
            this.IconPage.ImageUrl = "~/Images/Public/Fugue/icons-shadowless/" + icon;
        }
        else
        {
            this.IconPage.ImageUrl = "~/Images/Public/Fugue/icons-shadowless/pwcustom/" + icon;
        }
    }


    public string Icon
    {
        get { return this.icon; }
        set { this.icon = value; }
    }

    public string Title
    {
        get { return this.title; }
        set { this.title = value; }
    }

    public string Description
    {
        get { return this.description; }
        set { this.description = value; }
    }


    private string icon;
    private string title;
    private string description;
}

