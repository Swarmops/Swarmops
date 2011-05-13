using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Types;
using Activizr.Logic.Governance;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_Organization_CreateInternalPoll : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Reset();
        }
    }

    private void Reset()
    {
        this.DateFirst.SelectedDate = DateTime.Today.AddDays(3);
        this.DateLast.SelectedDate = DateTime.Today.AddDays(9);
        this.TextPollName.Text = string.Empty;
    }

    protected void ButtonCreate_Click(object sender, EventArgs e)
    {
        // TODO: Add Validation

        Organization org = Organization.PPSE; // HACK -- can't make OrganizationDropDownTree and GeographyDropDownTree cooperate
        Geography geo = this.DropGeographies.SelectedGeography;
        DateTime dateOpen = (DateTime) this.DateFirst.SelectedDate;
        DateTime dateClose = ((DateTime) this.DateLast.SelectedDate).AddDays(1).AddMinutes(-1);
        string pollName = this.TextPollName.Text;

        MeetingElection poll = MeetingElection.Create(_currentUser, org, geo, pollName, InternalPollResultsType.Schulze, 0,
                                                new DateTime(2006, 1, 1), new DateTime(2006, 1, 8), dateOpen, dateClose);

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alldone",
                                    "alert ('Internal poll #" + poll.Identity + " has been created.');",
                                    true);
        Reset();
    }
}
