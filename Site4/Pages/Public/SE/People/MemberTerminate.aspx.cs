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
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

using Membership = Activizr.Logic.Pirates.Membership;



public partial class Pages_Public_SE_People_MemberTerminate : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        bool success = false;
        string expectedHash1 = "";
        string membershipsIds = "" + Request.QueryString["MID"];

        int personId = 0;

        if (Int32.TryParse(Request.QueryString["MemberId"], out personId))
        {
            // Ok, at least we have  a valid person id.

            Person person = Person.FromIdentity(personId);

            DateTime currentExpiry = DateTime.MinValue;
            DateTime newExpiry = DateTime.MinValue;
            Memberships memberships = person.GetRecentMemberships(Membership.GracePeriod);

            {
                // The default is to renew all existing memberships. However, a person can also
                // request a transfer or to leave one specific organization.

                // This is built into the security token.

                string token1 = person.PasswordHash + "-" + membershipsIds;

                expectedHash1 = SHA1.Hash(token1).Replace(" ", "").Substring(0, 8);



                if (Request.QueryString["SecHash"] == expectedHash1)
                {
                    success = true;


                    foreach (Membership membership in memberships)
                    {
                        if (membership != null && membership.Active && (membership.OrganizationId == Organization.PPSEid || membership.Organization.Inherits(Organization.PPSEid)))
                        {
                            membership.Terminate(EventSource.SignupPage, person, "Membership in " + membership.Organization.NameShort + " was terminated while renewing.");
                        }
                    }
                }
            }


            if (!success)
            {
                // Send a couple mails
                PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.Failure, "Members failed to terminate:" + Request.RawUrl, "");

                person.SendNotice("Vill du f\xF6rnya?",
                    "Alldeles nyss f\xF6rs\xF6kte n\xE5gon, troligtvis du, f\xF6rnya ditt medlemskap i Piratpartiet " +
                    "och/eller Ung Pirat. Det misslyckades p\xE5 grund av en felaktig s\xE4kerhetskod.\r\n\r\n" +
                    "Det kan bero p\xE5 ett antal anledningar, men f\xF6r att vara s\xE4ker p\xE5 att det inte \xE4r " +
                    "on\xF6" + "diga tekniska fel som hindrar dig fr\xE5n att forts\xE4tta vara medlem, s\xE5 kan vi ocks\xE5 f\xF6rnya " +
                    "ditt medlemskap manuellt.\r\n\r\nAllt som kr\xE4vs \xE4r att du svarar JA p\xE5 det h\xE4r brevet och " +
                    "skickar tillbaka det till Medlemsservice (avs\xE4ndaren).\r\n\r\n" +
                    "Vill du f\xF6rnya ditt eller dina befintliga medlemskap i Piratpartiet och/eller Ung Pirat " +
                    "f\xF6r ett \xE5r till?\r\n\r\n", Organization.PPSEid);
                Person.FromIdentity(1).SendNotice("Misslyckad f\xF6rnyelse",
                    person.Name + " (#" + person.Identity.ToString() + ") f\xF6rs\xF6kte f\xF6rnya medlemskapet. Det misslyckades. " +
                    "Ett mail har skickats ut.\r\n", Organization.PPSEid);
            }
            else
            {
                Response.Redirect("http://www.piratpartiet.se/fornyelse");
            }


            this.PanelSuccess.Visible = success;
            this.PanelFail.Visible = !success;
        }
    }
}

