using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Support;

using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Logic.Structure;


public partial class Pages_Public_SE_UP_Renew : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		string result = "Tyvärr kunde vi inte tolka parametrarna riktigt. Kontakta medlemsservice på info@ungpirat.se.";

		int personId = 0;

		if (Int32.TryParse (Request.QueryString ["MemberId"], out personId))
		{
			// Ok, at least we have  a valid person id.

			Person person = Person.FromIdentity (personId);

			DateTime newExpiry = DateTime.MinValue;
			Memberships memberships = person.GetMemberships();
			bool ok = false;

			foreach (Membership membership in memberships)
			{
				if (membership.Organization.Inherits (Organization.UPSEid))
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
                        PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed, "Membership in Piratpartiet SE renewed.", "Membership was renewed from IP " + Request.UserHostAddress + ".");
                        break;
					}
				}
			}

			if (ok)
			{
				result = "Tack för att du förnyar! Ditt medlemskap går nu ut " + newExpiry.ToString ("yyyy-MM-dd") +
				         ", och gäller för föreningarna ";

				result += memberships[0].Organization.Name;

				for (int index = 0; index < memberships.Count; index++)
				{
					string divider = ", ";

					if (index == memberships.Count - 1)
					{
						divider = " och ";
					}

					result += divider + memberships[index].Organization.Name;
				}

				result += ". Du kommer också att få ett mail som bekräftar din förnyelse.";
			}

			this.LabelResults.Text = result;
		}
	}
}
