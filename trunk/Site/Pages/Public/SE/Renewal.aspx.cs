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
using Activizr.Logic.Support;

using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Logic.Security;


public partial class Pages_Public_SE_Renewal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		string result = "Tyvärr kunde vi inte tolka parametrarna riktigt. Kontakta medlemsservice på medlemsservice@piratpartiet.se.";

		int personId = 0;

		if (Int32.TryParse (Request.QueryString ["MemberId"], out personId))
		{
			// Ok, at least we have  a valid person id.

			Person person = Person.FromIdentity (personId);

			DateTime newExpiry = DateTime.MinValue;
            Memberships memberships = person.GetRecentMemberships(Membership.GracePeriod);
			bool ok = false;

			foreach (Membership membership in memberships)
			{
				string expectedHash = SHA1.Hash("Pirate" + person.Identity.ToString() +
						membership.Expires.Date.ToString("yyyyMMdd")).Replace(" ", "").Substring(0, 8);


				if (Request.QueryString ["SecHash"] == expectedHash)
				{
					ok = true;
					newExpiry = membership.Expires.Date.AddYears (1);
					Activizr.Logic.Support.PWEvents.CreateEvent (EventSource.SignupPage, EventType.ReceivedMembershipPayment,
					                                                person.Identity, membership.OrganizationId,
					                                                person.Geography.Identity, person.Identity, 0, Request.UserHostAddress);
                    PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed, "Membership in " + membership.Organization.NameShort + " renewed.", "Membership was renewed from IP " + Request.UserHostAddress + ".");
                    break;
				}
			}

            if (ok)
            {
                result = "Tack för att du förnyar! Ditt medlemskap går nu ut " + newExpiry.ToString("yyyy-MM-dd") +
                         ". Du kommer också att få ett mail som bekräftar din förnyelse.";
            }
            else
            {
                PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.Failure, "Members failed to renew:"+Request.RawUrl, "Membership was renewed from IP " + Request.UserHostAddress + ".");
            }

			this.LabelResults.Text = result;
		}
	}
}
