using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.ExtensionMethods;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using NGeoIP;
using NGeoIP.Client;
using Resources;
using Swarmops.Common.Enums;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Cache;

namespace Swarmops.Frontend.Pages.Public
{
    public partial class Signup : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            CommonV5.CulturePreInit (Request);

 	        base.OnPreInit(e);
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            // Was this a quick callback to set the culture?

            if (Request["Culture"] != null)
            {
                Response.SetCookie(new HttpCookie("PreferredCulture", Request["Culture"]));
                Response.Redirect(Request.RawUrl.Substring (0, Request.RawUrl.IndexOf ("&Culture=")), true);
            }

            // Find what organization we're supposed to sign up to

            this.Organization = Organization.FromVanityDomain (Request.Url.Host);

            if (this.Organization == null)
            {
                int organizationId = Convert.ToInt32 (Request["OrganizationId"]); // may throw - not pretty but ok for now. // TODO
                this.Organization = Organization.FromIdentity (organizationId); // may also throw, as above
            }

            if (this.Organization == null)
            {
                throw new ArgumentException("Organization not specified");  // TODO: make a friendly landing page instead
            }


            // Override style widths - (this will cause problems with a future responsive design; come back here to fix that)
            /*
            this.DropCountries.Style[HtmlTextWriterStyle.Width] = "204px";
            this.DropGenders.Style[HtmlTextWriterStyle.Width] = "204px";*/

            /*
            this.BoxTitle.Text = PageTitle = String.Format(Resources.Pages.Swarm.AddPerson_Title,
                Participant.Localized(CurrentOrganization.RegularLabel));
            InfoBoxLiteral = String.Format(Resources.Pages.Swarm.AddPerson_Info, Global.Timespan_OneYear,
                Participant.Localized(CurrentOrganization.RegularLabel, TitleVariant.Ship),
                DateTime.Today.AddYears(1).ToLongDateString());*/

            if (!Page.IsPostBack)
            {
                Localize();
                Populate();

                this.TextName.Focus();
            }

        }

        private void Populate()
        {
            Countries allCountries = Countries.All;
            this.DropCountries.Items.Clear();

            foreach (Country country in allCountries)
            {
                string countryLocalName = country.Localized;
                string countryDisplay = country.Code + " " + countryLocalName;
                this.DropCountries.Items.Add (new ListItem (countryDisplay, country.Code));
            }

            if (this.Organization.DefaultCountry != null)
            {
                this.DropCountries.SelectedValue = this.Organization.DefaultCountry.Code;
            }

            this.DropGenders.Items.Clear();
            this.DropGenders.Items.Add (new ListItem (Global.Global_UnknownUndisclosed, "Unknown"));
            this.DropGenders.Items.Add (new ListItem (Global.Global_Female, "Female"));
            this.DropGenders.Items.Add (new ListItem (Global.Global_Male, "Male"));
        }

        private void Localize()
        {
            this.LabelSidebarActionsHeader.Text = 

            this.LiteralErrorCity.Text = Resources.Pages.Swarm.AddPerson_ErrorCity;
            this.LiteralErrorMail.Text = Resources.Pages.Swarm.AddPerson_ErrorMail;
            this.LiteralErrorName.Text = Resources.Pages.Swarm.AddPerson_ErrorName;
            this.LiteralErrorStreet.Text = Resources.Pages.Swarm.AddPerson_ErrorStreet;
            this.LiteralErrorDate.Text = Resources.Pages.Swarm.AddPerson_ErrorDate;

            this.LiteralErrorNeedPassword.Text = Resources.Pages.Public.Signup_Error_NeedPassword;
            this.LiteralErrorPasswordMismatch.Text = Resources.Pages.Public.Signup_Error_PasswordMismatch;
            this.LiteralErrorSelectActivationLevel.Text = Resources.Pages.Public.Signup_Error_SelectActivationLevel;
            this.LiteralErrorSelectVolunteerPosition.Text = Resources.Pages.Public.Signup_Error_SelectPosition;
            this.LiteralErrorMailExists.Text = Resources.Pages.Public.Signup_Error_MailExists;

            this.LabelWelcomeHeader.Text = String.Format (Resources.Pages.Public.Signup_Welcome, Organization.Name);
            this.LabelHeader.Text = String.Format(Resources.Pages.Public.Signup_SigningUp, Organization.Name).ToUpperInvariant();
            this.LabelYourLogon.Text = Resources.Pages.Public.Signup_YourLogon;
            this.LabelYourLogonText.Text = Resources.Pages.Public.Signup_YourLogonText;

            this.LabelStep1Header.Text = Resources.Pages.Public.Signup_Step1Header;
            this.LabelStep1Text.Text = Resources.Pages.Public.Signup_Step1Text;
            this.LabelStep2Header.Text = Resources.Pages.Public.Signup_Step2Header;
            this.LabelStep2Text.Text = Resources.Pages.Public.Signup_Step2Text;
            this.LabelStep3Header.Text = Resources.Pages.Public.Signup_Step3Header;
            this.LabelStep3Text.Text = Resources.Pages.Public.Signup_Step3Text;
            this.LabelStep4Header.Text = Resources.Pages.Public.Signup_Step4Header;
            this.LabelStep4Text.Text = Resources.Pages.Public.Signup_Step4Text;
            this.LabelStep5Header.Text = Resources.Pages.Public.Signup_Step5Header;
            this.LabelStep5Text.Text = Resources.Pages.Public.Signup_Step5Text;
            this.LabelStep6Header.Text = Resources.Pages.Public.Signup_Step6Header;
            this.LabelStep6Text.Text = Resources.Pages.Public.Signup_Step6Text;

            this.LabelName.Text = Resources.Pages.Public.Signup_YourName;
            this.LabelCountry.Text = Resources.Global.Global_Country;
            this.LabelMail.Text = Resources.Global.Global_Mail;
            this.LabelPhone.Text = Resources.Global.Global_Phone;
            this.LabelStreet1.Text = Resources.Pages.Swarm.AddPerson_Street1PO;
            this.LabelStreet2.Text = Resources.Pages.Swarm.AddPerson_Street2;
            this.LabelPostalCode.Text = Resources.Global.Global_PostalCode;
            this.LabelCity.Text = Resources.Global.Global_City;
            this.LabelGeographyDetected.Text = Resources.Pages.Swarm.AddPerson_GeographyDetected;
            this.LabelDateOfBirth.Text = Resources.Global.Global_DateOfBirth;
            this.LabelLegalGender.Text = Resources.Pages.Swarm.AddPerson_LegalGender;

            this.LabelLoginKey.Text = Resources.Pages.Public.Signup_MailLoginKey;
            this.LabelPassword1.Text = Resources.Pages.Public.Signup_Password1;
            this.LabelPassword2.Text = Resources.Pages.Public.Signup_Password2;

            this.TextDateOfBirth.Attributes["placeholder"] = Global.Global_DateFormatShortReadable;
            this.TextName.Attributes["placeholder"] = "Joe Smith";
            this.TextMail.Attributes["placeholder"] = "joe@example.com";
            this.TextPhone.Attributes["placeholder"] = "+1 263 151 1341";
            this.TextStreet1.Attributes["placeholder"] = "78 West Avenue";
            this.TextPostal.Attributes["placeholder"] = "12345";

            this.LabelActivationLevelHeader.Text = Resources.Pages.Public.Signup_ActivationLevel;
            this.LabelActivationLevelIntro.Text = String.Format(Resources.Pages.Public.Signup_ActivationLevelText, this.Organization.Name);
            this.RadioActivationPassive.Text = String.Format(Resources.Pages.Public.Signup_ActivationPassiveHeader, Participant.Localized (this.Organization.RegularLabel));
            this.LabelActivationPassiveText.Text = String.Format(Resources.Pages.Public.Signup_ActivationPassive, Participant.Localized(this.Organization.RegularLabel));
            this.RadioActivationActive.Text = String.Format(Resources.Pages.Public.Signup_ActivationActiveHeader, Participant.Localized (this.Organization.ActivistLabel));
            this.LabelActivationActiveText.Text = Resources.Pages.Public.Signup_ActivationActive;
            this.RadioActivationVolunteer.Text = Resources.Pages.Public.Signup_ActivationVolunteerHeader;
            this.LabelActivationVolunteerText.Text = Resources.Pages.Public.Signup_ActivationVolunteer;

            this.LabelVolunteerHeaderHighestGeography.Text = Resources.Pages.Public.Signup_VolunteerTableHeaderGeography;
            this.LabelVolunteerHeaderPositionTitle.Text = Resources.Pages.Public.Signup_VolunteerTableHeaderPosition;
            this.LabelVolunteerPositionHeader.Text = Resources.Pages.Public.Signup_VolunteerHeader;
            this.LabelVolunteerPositionText.Text = Resources.Pages.Public.Signup_VolunteerText;

            this.LabelFinalizeSignupHeader.Text = Resources.Pages.Public.Signup_FinalizeSignup;

            // Enable support for RTL languages

            if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            {
                this.LiteralBodyAttributes.Text = @"dir='rtl' class='rtl'";
            }

            this.LiteralWizardNextButton.Text = Resources.Global.Global_WizardNext;
            this.LiteralWizardFinishButton.Text = Resources.Global.Global_WizardFinish;

            this.LabelSidebarActionsHeader.Text = Resources.Pages.Public.Signup_FeeHeader;
            // if (this.Organization.MembershipFee == 0 && this.Organization.MembershipRenewalFee == 0)
            this.LabelSidebarActionsContent.Text = String.Format (Resources.Pages.Public.Signup_NoFeeText,
                Participant.Localized (this.Organization.RegularLabel, TitleVariant.Ship), DateTime.Today.AddYears (1).ToShortDateString(), this.Organization.Name);

            this.LabelSidebarInfoHeader.Text = Resources.Global.Sidebar_Information;
            this.LabelSidebarTodoHeader.Text = Resources.Global.Sidebar_Todo;

            this.LabelSidebarTodo.Text = Resources.Pages.Public.Signup_Todo_Complete;
            this.LabelSidebarInfoContent.Text =
                @"This is the organization's Short About text. It is set in Admin / Org Settings.";
        }


        protected Organization Organization;

        [WebMethod]
        public static AjaxCallResult SignupParticipant (string name, int organizationId, string mail, string password, string phone,
            string street1, string street2, string postalCode, string city, string countryCode, string dateOfBirth,
            int geographyId, bool activist, PersonGender gender, int[] positionIdsVolunteer)
        {
            CommonV5.CulturePreInit (HttpContext.Current.Request); // Set culture, for date parsing

            if (geographyId == 0)
            {
                geographyId = Geography.RootIdentity; // if geo was undetermined, set it to "Global"
            }

            Organization organization = Organization.FromIdentity (organizationId);
            DateTime parsedDateOfBirth = new DateTime (1800, 1, 1); // Default if unspecified

            if (dateOfBirth.Length > 0)
            {
                parsedDateOfBirth = DateTime.Parse (dateOfBirth);
            }

            Person newPerson = Person.Create (name, mail, password, phone, street1 + "\n" + street2.Trim(), postalCode,
                city, countryCode, parsedDateOfBirth, gender);
            Participation participation = newPerson.AddParticipation (organization, DateTime.UtcNow.AddYears (1));  // TODO: set duration from organization settings of Participantship

            // TODO: SEND NOTIFICATIONS

            // Log the signup

            SwarmopsLog.CreateEntry (newPerson, new PersonAddedLogEntry (participation, newPerson));

            // Create notification

            OutboundComm.CreateParticipantNotification (newPerson, newPerson, organization,
                NotificationResource.Participant_Signup);

            // Add the bells and whistles

            if (activist)
            {
                newPerson.CreateActivist (false, false);
            }

            if (positionIdsVolunteer.Length > 0)
            {
                Volunteer volunteer = newPerson.CreateVolunteer();
                foreach (int positionId in positionIdsVolunteer)
                {
                    Position position = Position.FromIdentity (positionId);
                    volunteer.AddPosition (position);
                    SwarmopsLog.CreateEntry (newPerson, new VolunteerForPositionLogEntry (newPerson, position));
                }
            }

            newPerson.LastLogonOrganizationId = organizationId;

            // Create a welcome message to the Dashboard

            HttpContext.Current.Response.AppendCookie (new HttpCookie ("DashboardMessage", CommonV5.JavascriptEscape(String.Format(Resources.Pages.Public.Signup_DashboardMessage, organization.Name))));

            // Set authentication cookie, which will log the new person in using the credentials just given

            FormsAuthentication.SetAuthCookie (Authority.FromLogin (newPerson).ToEncryptedXml(), true);

            AjaxCallResult result = new AjaxCallResult {Success = true};
            return result;
        }

        [WebMethod]
        public static AjaxCallResult CheckMailFree (string mail)
        {
            People people = People.FromMail (mail);
            // This should be zero

            if (people.Count == 0)
            {
                return new AjaxCallResult {Success = true};
            }

            return new AjaxCallResult {Success = false};
        }

        [WebMethod]
        public static AjaxCallResult GuessCountry()
        {
            // IMPORTANT: If you're implementing a sensitive organization, this should use YOUR OWN geoip server and not freegeoip.com, which
            // may potentially be eavesdroppable. Look at the freegeoip.com for how to download their database.

            if (HttpContext.Current.Request.UserHostAddress == "::1" && !Debugger.IsAttached)
            {
                // yeah, we're running from localhost, no big point trying to geoip this
                return new AjaxCallResult {Success = false};
            }

            try
            {
                string ipAddress = Logic.Support.SupportFunctions.GetMostLikelyRemoteIPAddress();

                NGeoIP.RawData rawData = (NGeoIP.RawData) GuidCache.Get(ipAddress);

                Persistence.Key["DebugData"] = HttpContext.Current.Request.ToRaw();

                if (rawData == null)
                {
                    NGeoIP.Request request = new Request()
                    {
                        Format = Format.Json,
                        IP = ipAddress
                    };
                    NGeoClient client = new NGeoClient(request);
                    rawData = client.Execute();
                }

                if (!string.IsNullOrEmpty (rawData.CountryCode))
                {
                    // Successful lookup

                    GuidCache.Set(ipAddress, rawData);  // store lookup results in cache for later

                    return new AjaxCallResult {Success = true, DisplayMessage = rawData.CountryCode};
                }
            }
            catch (Exception)
            {
                // We failed, return failure in the statement following this block
            }

            return new AjaxCallResult { Success = false };
        }
    }
}