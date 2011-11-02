using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Types;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Pages_Special_SE_UngPirat_Join : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		int personId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

		// Determine appropriate UP organization for this person

		Person person = Person.FromIdentity(personId);
		BasicOrganization org = Organizations.GetMostLocalOrganization(person.GeographyId, Organization.UPSEid );

		Memberships memberships = person.GetMemberships();

		bool alreadyMember = false;

		foreach (Activizr.Basic.Types.BasicMembership membership in memberships)
		{
			if (membership.OrganizationId == org.OrganizationId)
			{
				alreadyMember = true;
			}
		}

		this.LabelOrg.Text = Server.HtmlEncode (org.NameShort); //.Replace ("å", "&aring;").Replace ("ö", "&ouml;");



		this.LabelResult.Text = "som &auml;r den lokalavdelning av Ung Pirat som tar upp medlemmar i ditt l&auml;n";

		if (alreadyMember)
		{
			this.LabelWelcome.Text = "Du &auml;r redan medlem i";
		}
		else
		{
			this.LabelWelcome.Text = "Du &auml;r nu medlem i";

			DateTime paidUntil = DateTime.Now.AddYears(1);

			foreach (Activizr.Logic.Pirates.Membership membership in memberships)
			{
                if (membership.OrganizationId == Organization.PPSEid)
				{
					paidUntil = membership.Expires.AddYears(1);
					membership.Expires = paidUntil;
					this.LabelResult.Text += ", och ett kalender&aring;r har lagts till ditt medlemskap";
				}
			}

			Activizr.Logic.Pirates.Membership.Create(personId, org.OrganizationId, paidUntil);
			Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.AddedMembership, personId, org.OrganizationId, person.GeographyId, person.PersonId, 0, string.Empty);
		}
		
	}
}
