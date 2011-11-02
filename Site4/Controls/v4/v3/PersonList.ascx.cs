using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Activizr.Interface;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;


public partial class Controls_PersonList : System.Web.UI.UserControl
{


    private bool showStatus;
    public bool ShowStatus
    {
        get
        {
            if (ViewState["showStatus"] == null)
            {
                ViewState["showStatus"] = showStatus;
                return showStatus;
            }
            else
            {
                showStatus = (bool)ViewState["showStatus"];
                return (bool)ViewState["showStatus"];
            }
        }
        set
        {
            showStatus = value;
            ViewState["showStatus"] = value;
        }
    }

    private bool showStreet;
    public bool ShowStreet
    {
        get
        {
            if (ViewState["showStreet"] == null)
            {
                ViewState["showStreet"] = showStreet;
                return showStreet;
            }
            else
            {
                showStreet = (bool)ViewState["showStreet"];
                return (bool)ViewState["showStreet"];
            }
        }
        set
        {
            showStreet = value;
            ViewState["showStreet"] = value;
        }
    }
    private bool showExpiry;
    public bool ShowExpiry
    {
        get
        {
            if (ViewState["showExpiry"] == null)
            {
                ViewState["showExpiry"] = showExpiry;
                return showExpiry;
            }
            else
            {
                showExpiry = (bool)ViewState["showExpiry"];
                return (bool)ViewState["showExpiry"];
            }
        }
        set
        {
            showExpiry = value;
            ViewState["showExpiry"] = value;
        }
    }

    private int listedOrg = 0;
    public int ListedOrg
    {
        get
        {
            if (ViewState["listedOrg"] == null)
            {
                ViewState["listedOrg"] = listedOrg;
                return listedOrg;
            }
            else
            {
                listedOrg = (int)ViewState["listedOrg"];
                return (int)ViewState["listedOrg"];
            }
        }
        set
        {
            listedOrg = value;
            ViewState["listedOrg"] = value;
        }
    }

    private string jsEditFunction = "";
    public string JsEditFunction
    {
        get
        {
            if (ViewState["jsEditFunction"] == null)
            {
                ViewState["jsEditFunction"] = jsEditFunction;
                return jsEditFunction;
            }
            else
            {
                jsEditFunction = (string)ViewState["jsEditFunction"];
                return (string)ViewState["jsEditFunction"];
            }
        }
        set
        {
            jsEditFunction = value;
            ViewState["jsEditFunction"] = value;
        }
    }

    public GridView GridView
    {
        get
        {
            return this.Grid;
        }
    }

    Authority _authority = null;
    Person _currentUser = null;
    Organization _listedOrganisation = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        int currentUserId = 0;
        currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        _currentUser = Person.FromIdentity(currentUserId);
        _authority = _currentUser.GetAuthority();
        _listedOrganisation = null;
        if (ListedOrg != 0)
            _listedOrganisation = Organization.FromIdentity(ListedOrg); ;

    }

    protected void PeopleDataSource_Selecting (object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["people"] = (People)HttpContext.Current.Session["PersonList"];
    }

    protected void Grid_RowCommand (object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "ViewEdit":
                if (ViewEditCommand == null)
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    int personId = Convert.ToInt32(this.Grid.DataKeys[index].Value);
                    Person displayedPerson = Person.FromIdentity(personId);
                    HttpContext.Current.Session["DisplayedPerson"] = displayedPerson;
                    Response.Redirect("BasicDetails.aspx?id=" + personId);
                }
                else
                {
                    ViewEditCommand(sender, e);
                }
                break;

        }
    }

    public People PersonList
    {
        set
        {
            HttpContext.Current.Session["PersonList"] = value; // Set for PeopleDataSource_Selecting
            Grid.DataBind();
        }
    }

    private Organizations _orgsToList = null;

    public Organizations OrgsToList
    {
        get
        {
            if (_orgsToList == null)
            {
                _orgsToList = new Organizations();

                if (ListedOrg != 0)
                {
                    if (_listedOrganisation == null)
                        _listedOrganisation = Organization.FromIdentity(ListedOrg); ;

                    _orgsToList = _listedOrganisation.GetTree();
                }
                else
                {
                    _orgsToList = Organizations.FromSingle(Organization.PPSE);
                }
            }
            return _orgsToList;
        }
    }
    protected void Grid_RowDataBound (object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            string expiresDate = "";
            string memberSinceDate = "";
            string statusString = "";

            Person cP = e.Row.DataItem as Person;

            Grid.Columns[3].Visible = ShowStreet;

            Grid.Columns[8].Visible = false; // ShowStatus;

            Grid.Columns[9].Visible = ShowExpiry;

            if (ListedOrg != 0)
            {
                Grid.Columns[10].Visible = true;
            }

            if (ShowExpiry || ListedOrg != 0 || ShowStatus)
            {
                statusString = "Member ";
                Memberships ms = cP.GetRecentMemberships(Activizr.Logic.Pirates.Membership.GracePeriod);
                ms = ms.RemoveToMatchAuthority(cP.Geography, _authority);
                if (ms.Count > 0)
                {
                    foreach (Activizr.Logic.Pirates.Membership m in ms)
                    {
                        if (m.OrganizationId == ListedOrg)
                        {
                            expiresDate = m.Expires.ToString("yyyy-MM-dd");
                            memberSinceDate = m.MemberSince.ToString("yyyy-MM-dd");
                        }
                        statusString += " " + m.Organization.NameShort;
                        if (!m.Active)
                            statusString += "*";
                    }
                    if (cP.IsActivist)
                    {
                        if (ms.Count > 0)
                            statusString += " ";
                        statusString += "Activist";
                    }
                }
            }


            Label lblStatus = e.Row.FindControl("LabelStatus") as Label;
            lblStatus.Text = statusString;

            Label lbl = e.Row.FindControl("LabelExpiry") as Label;
            lbl.Text = expiresDate;

            Label lbl2 = e.Row.FindControl("LabelMemberSince") as Label;
            lbl2.Text = memberSinceDate;

            Image image = e.Row.FindControl("IconPerson") as Image;

            PersonIcon.PersonIconSpec icon = PersonIcon.ForPerson(cP, OrgsToList);

            image.ImageUrl = "/Images/Public/Fugue/icons-shadowless/" + icon.Image;
            image.ToolTip = icon.AltText;


            HyperLink phoneLink = e.Row.FindControl("HyperLinkPhone") as HyperLink;

            string phone = "" + cP.Phone.Trim();
            if (phone.Length > 4 && !phone.StartsWith("+"))
            {
                if (phone.StartsWith("00"))
                {
                    phone = "+" + phone.Substring(2);
                }
                else if (phone.StartsWith("0"))
                {
                    phone = "+46" + phone.Substring(1);
                }
            }

            phoneLink.Text = "" + cP.Phone.Trim();
            phoneLink.NavigateUrl = "callto:" + phone;


            if (JsEditFunction != "")
            {
                LinkButton ViewEditButton = e.Row.FindControl("ViewEditButton") as LinkButton;
                ViewEditButton.OnClientClick = "return " + JsEditFunction + "(" + cP.Identity + ");";
            }

        }
    }

    public event GridViewCommandEventHandler ViewEditCommand;
}
