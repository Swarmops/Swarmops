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
using Activizr.Logic.Support;

using Membership=Activizr.Logic.Pirates.Membership;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_Public_SE_People_MemberNewActivist : System.Web.UI.Page
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
			bool ok = false;

            string expectedHash = person.PasswordHash.Replace(" ", "").Substring(0, 8);

            if (person.IsActivist == true)
            {
                result = "Du är redan pirataktivist.";
            }
            else if (Request.QueryString ["SecHash"] == expectedHash)
			{
				ok = true;
				Activizr.Logic.Support.PWEvents.CreateEvent (EventSource.SignupPage, EventType.NewActivist,
				                                                person.Identity, Organization.PPSEid,
				                                                person.Geography.Identity, person.Identity, 0, string.Empty);
			}

			if (ok)
			{
				result = "Tack för att du väljer att bli aktivist! Du får strax ett email som förklarar mer. ";

                person.CreateActivist(false, true);
                PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.ActivistJoin, "Joined as activist", string.Empty);
			}

			this.LabelResults.Text = result;
		}
	}
}
