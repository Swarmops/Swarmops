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
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using System.Collections.Generic;

public partial class Controls_PersonDetailsPagesMenu : System.Web.UI.UserControl
{
    private int personID;

    public int PersonID
    {
        get { return personID; }
        set { personID = value; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        string currentDir = Request.RawUrl.Substring(0, Request.RawUrl.LastIndexOf("/")) + "/";

        if (!IsPostBack)
        {
            ViewState[LinkBasicDetails.ID] = currentDir + LinkBasicDetails.NavigateUrl + "?id=";
            ViewState[LinkMemberships.ID] = currentDir + LinkMemberships.NavigateUrl + "?id=";
            ViewState[LinkRolesResponsibilities.ID] = currentDir + LinkRolesResponsibilities.NavigateUrl + "?id=";
            ViewState[LinkAccountSettings.ID] = currentDir + LinkAccountSettings.NavigateUrl + "?id=";
            ViewState[LinkSubscriptions.ID] = currentDir + LinkSubscriptions.NavigateUrl + "?id=";
        }

        LabelSelectedMember.Text = "";
        if (personID == 0 && Request["id"] != null)
        {
            personID = Convert.ToInt32(Request["id"]);
            Person selected = Person.FromIdentity(personID);
            LabelSelectedMember.Text = "#" + personID + " " + selected.Name;
        }

        LinkBasicDetails.NavigateUrl = ""+ ViewState[LinkBasicDetails.ID] + personID;
        LinkMemberships.NavigateUrl = "" + ViewState[LinkMemberships.ID] + personID;
        LinkRolesResponsibilities.NavigateUrl = "" + ViewState[LinkRolesResponsibilities.ID] + personID;
        LinkAccountSettings.NavigateUrl = "" + ViewState[LinkAccountSettings.ID] + personID;
        LinkSubscriptions.NavigateUrl = "" + ViewState[LinkSubscriptions.ID] + personID;

        switch (currentPage)
        {
            case 0:
                this.LinkBasicDetails.NavigateUrl = string.Empty;
                // this.LinkBasicDetails.Font.Bold = true;
                break;
            case 1:
                this.LinkMemberships.NavigateUrl = string.Empty;
                // this.LinkMemberships.Font.Bold = true;
                break;
            case 2:
                this.LinkRolesResponsibilities.NavigateUrl = string.Empty;
                // this.LinkRolesResponsibilities.Font.Bold = true;
                break;
            case 3:
                this.LinkAccountSettings.NavigateUrl = string.Empty;
                break;
            case 4:
                this.LinkSubscriptions.NavigateUrl = string.Empty;
                break;
        }
    }

    public int CurrentPage
    {
        set
        {
            this.currentPage = value;
        }
    }

    private int currentPage;
}
