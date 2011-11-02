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
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Support;

public partial class Pages_Public_Handlers_ConfirmParleyAttendee : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string attendeeIdString = Request.QueryString["AttendeeId"];
        ParleyAttendee attendee = ParleyAttendee.FromIdentity(Int32.Parse(attendeeIdString));
        Parley parley = attendee.Parley;

        string expectedSecurityCode =
            SHA1.Hash(attendee.PersonId.ToString() + parley.Identity.ToString()).Replace(" ", "").Substring(0, 8);

        if (attendee.Active)
        {
            this.LabelResult.Text = "This attendee has already been confirmed.";
        }
        else if (expectedSecurityCode == Request.QueryString["SecurityCode"])
        {
            attendee.Active = true;

            PWEvents.CreateEvent(EventSource.SignupPage, EventType.ParleyAttendeeConfirmed, attendee.PersonId,
                                 parley.OrganizationId, parley.GeographyId, attendee.PersonId, attendee.Identity,
                                 string.Empty);

            this.LabelResult.Text =
                "Attendance has been confirmed. Thank you for registering for the conference, and have fun!";

        }
        else
        {
            this.LabelResult.Text =
                "We were unable to match the attendee identity with the security code. No attendance was confirmed.";
        }

    }
}
