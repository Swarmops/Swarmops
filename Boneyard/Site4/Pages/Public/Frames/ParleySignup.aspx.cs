using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Basic;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;

namespace Activizr.Site.Pages.Public.Frames
{
    public partial class ParleySignup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string parleyIdString = Request.QueryString["ParleyId"];
            int parleyId = Int32.Parse(parleyIdString);
            _parley = Parley.FromIdentity(parleyId);

            this.LabelConference.Text = _parley.Name;

            string bgColor = Request.QueryString["BackgroundColor"];
            if (!String.IsNullOrEmpty(bgColor))
            {
                this.BodyTag.Style["background-color"] = "#" + bgColor.Substring(0, 6); // safety net against code injection - cut to six chars
            }

            this.TextNameFirst.Style[HtmlTextWriterStyle.Width] = "100px";
            this.TextNameLast.Style[HtmlTextWriterStyle.Width] = "150px";
            this.TextEmail.Style[HtmlTextWriterStyle.Width] = "260px";
            this.TextEmail2.Style[HtmlTextWriterStyle.Width] = "260px";

            PopulateOptions();


        }


        private Parley _parley;

        private void PopulateOptions()
        {
            ParleyOptions options = ParleyOptions.ForParley(_parley);

            string spacer = string.Empty;

            for (int loop = 0; loop < options.Count; loop++)
            {
                spacer += "&nbsp;<br/>";
            }

            this.LiteralOptionsSpacer.Text = spacer;

            string currencyCode = _parley.Organization.DefaultCountry.Currency.Code;

            CheckBox boxAttendance = new CheckBox();
            boxAttendance.Text = "Attendance, " + currencyCode + " " + (_parley.AttendanceFeeCents/100).ToString();
            boxAttendance.Checked = true;
            boxAttendance.Enabled = false;
            boxAttendance.ID = "CheckOptionAttendance";

            this.PlaceholderOptions.Controls.Add(boxAttendance);

            this.PlaceholderOptions.Controls.Add(GetLiteral("&nbsp;<br/>"));

            foreach (ParleyOption option in options)
            {
                CheckBox boxOption = new CheckBox();
                boxOption.Text = option.Description + ", " + currencyCode +  " " + (option.AmountCents/100).ToString();
                boxOption.ID = "CheckOption" + option.Identity.ToString();
                this.PlaceholderOptions.Controls.Add(boxOption);
                this.PlaceholderOptions.Controls.Add(GetLiteral("&nbsp;<br/>"));
            }
        }

        private Literal GetLiteral(string text)
        {
            Literal literal = new Literal();
            literal.Text = text;
            return literal;
        }

        protected void ButtonSignup_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return; // no action
            }

            // Sign up this attendee, and create event to send confirmation mail

            ParleyAttendee newAttendee = _parley.CreateAttendee(this.TextNameFirst.Text, this.TextNameLast.Text, this.TextEmail.Text, false);

            // Add options

            ParleyOptions options = _parley.Options;

            foreach (ParleyOption option in options)
            {
                CheckBox checkOption = (CheckBox) this.PlaceholderOptions.FindControl("CheckOption" + option.Identity.ToString());
                if (checkOption.Checked)
                {
                    newAttendee.AddOption(option);
                }
            }

            PWEvents.CreateEvent(EventSource.SignupPage, EventType.ParleyAttendeeCreated, newAttendee.PersonId,
                                 _parley.OrganizationId, _parley.GeographyId, newAttendee.PersonId, newAttendee.Identity,
                                 string.Empty);

            if (Request.UserLanguages.Length > 0)
            {
                newAttendee.Person.PreferredCulture = Request.UserLanguages[0];
            }

            this.PanelSignupCompleted.Visible = true;
            this.PanelSignup.Visible = false;
        }
    }
}