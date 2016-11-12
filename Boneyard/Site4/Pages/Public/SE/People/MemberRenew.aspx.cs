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



public partial class Pages_Public_SE_People_MemberRenew : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        bool success = false;
        string expectedHash1 = "";
        string expectedHash2 = "";
        Person person = null;
        string redirectUrl = string.Empty;

        int personId = 0;

        //New or old variant? MemberId indicates old variant with common date for all orgs.
        if (Int32.TryParse(Request.QueryString["MemberId"], out personId))
        {
            // Ok, at least we have  a valid person id.

            person = Person.FromIdentity(personId);

            DateTime currentExpiry = DateTime.MaxValue;
            DateTime newExpiry = DateTime.MinValue;
            Memberships memberships = person.GetRecentMemberships(Membership.GracePeriod);
            string[] mIds = ("" + Request.QueryString["MID"]).Split(',');
            bool membershipExists = false;
            Membership membershipToExtend = null;
            if (mIds.Length > 0)
            {
                memberships = new Memberships();
                foreach (string mId in mIds)
                {
                    try
                    {
                        Membership membership = Membership.FromIdentity(Convert.ToInt32(mId));
                        memberships.Add(membership);
                        //find earliest expiry in expiring orgs
                        if (membership.Expires < currentExpiry)
                        {
                            currentExpiry = membership.Expires;
                            newExpiry = currentExpiry.AddYears(1);
                            membershipExists = true;
                            membershipToExtend = membership;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // Get the new expiry from org 1 Organization.PPSEid

            bool isPpMember = false;
            bool isUPMember = false;

            foreach (Membership membership in person.GetRecentMemberships(Membership.GracePeriod))
            {
                if (membership.OrganizationId == Organization.PPSEid)
                {
                    isPpMember = true;
                }
                else if (membership.Organization.IsOrInherits(Organization.UPSEid))
                {
                    isUPMember = true;
                }
                if (membership.Expires < currentExpiry
                    && (membership.Organization.IsOrInherits(Organization.UPSEid)
                        || membership.OrganizationId == Organization.PPSEid))
                {
                    currentExpiry = membership.Expires;
                    newExpiry = currentExpiry.AddYears(1);
                    membershipExists = true;
                    membershipToExtend = membership;
                }

            }


            if (membershipExists)
            {
                // The default is to renew all existing memberships. However, a person can also
                // request a transfer or to leave one specific organization.

                // This is built into the security token.

                string token1 = person.Name.Replace(" ", "-") + person.PasswordHash + "-" +
                                currentExpiry.Year.ToString();
                string token2 = person.PasswordHash + "-" + currentExpiry.Year.ToString();
                int leaveOrgId = 0;
                int transferOldOrgId = 0;
                int transferNewOrgId = 0;

                string leaveString = Request.QueryString["Leave"];
                string transferString = Request.QueryString["Transfer"];

                if (!string.IsNullOrEmpty(leaveString))
                {
                    leaveOrgId = Int32.Parse(leaveString);
                    token1 += "-Leave" + leaveOrgId.ToString();
                    token2 += "-Leave" + leaveOrgId.ToString();
                }

                if (!string.IsNullOrEmpty(transferString))
                {
                    string[] tokens = transferString.Split(',');
                    transferOldOrgId = Int32.Parse(tokens[0]);
                    transferNewOrgId = Int32.Parse(tokens[1]);

                    token1 += "-Transfer" + transferOldOrgId.ToString() + "/" + transferNewOrgId.ToString();
                    token2 += "-Transfer" + transferOldOrgId.ToString() + "/" + transferNewOrgId.ToString();
                }

                expectedHash1 = SHA1.Hash(token1).Replace(" ", "").Substring(0, 8);
                expectedHash2 = SHA1.Hash(token2).Replace(" ", "").Substring(0, 8);

                if (Request.QueryString["SecHash"] == expectedHash1 || Request.QueryString["SecHash"] == expectedHash2)
                {
                    success = true;

                    this.LabelExpires.Text = newExpiry.ToString("yyyy-MM-dd");

                    if (leaveOrgId > 0)
                    {
                        Membership membership = person.GetRecentMembership(Membership.GracePeriod, leaveOrgId);

                        // Might have been terminated already
                        if (membership != null && membership.Active)
                        {
                            //Terminate logs and creates appropriate events
                            membership.Terminate(EventSource.SignupPage, person,
                                                 "Membership in " + membership.Organization.NameShort +
                                                 " was terminated while renewing.");
                        }
                        this.PanelLeft.Visible = true;
                        this.LiteralLeftOrganizations.Text = "<b>" +
                                                             Server.HtmlEncode(
                                                                 Organization.FromIdentity(leaveOrgId).Name) + "</b>";
                    }

                    if (transferOldOrgId > 0)
                    {
                        Membership oldMembership = person.GetRecentMembership(Membership.GracePeriod, transferOldOrgId);
                        // Might have been terminated already
                        if (oldMembership != null)
                        {
                            Membership newMembership = Membership.Create(person.Identity, transferNewOrgId,
                                                                         oldMembership.Expires);

                            //Terminate logs and creates appropriate events
                            oldMembership.Terminate(EventSource.SignupPage, person,
                                                    "Membership in " + oldMembership.Organization.NameShort +
                                                    " was transferred to " + newMembership.Organization.NameShort +
                                                    " while renewing.");

                        }
                        this.LabelTransferOldOrganization.Text = Organization.FromIdentity(transferOldOrgId).Name;
                        this.LabelTransferNewOrganization.Text = Organization.FromIdentity(transferNewOrgId).Name;
                        this.PanelTransferred.Visible = true;
                    }

                    Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage,
                                                                 EventType.ReceivedMembershipPayment,
                                                                 person.Identity, membershipToExtend.OrganizationId,
                                                                 person.Geography.Identity, person.Identity, 0,
                                                                 Request.UserHostAddress);

                    string orgList = string.Empty;

                    foreach (Membership membership in person.GetRecentMemberships(Membership.GracePeriod))
                    {
                        if (membership.OrganizationId != leaveOrgId)
                        {
                            if ((isPpMember && membership.OrganizationId == Organization.PPSEid)
                                || (isUPMember && membership.Organization.IsOrInherits(Organization.UPSEid))
                                || (memberships.Contains(membership)))
                            {
                                PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed,
                                            "Membership in " + membership.Organization.NameShort + " renewed.",
                                            "Membership was renewed from IP " + Request.UserHostAddress + ".");
                                orgList += ", <b>" + Server.HtmlEncode(membership.Organization.Name) + "</b>";
                            }
                        }
                    }

                    orgList = orgList.Substring(2);
                    this.LiteralRenewedOrganizations.Text = orgList;
                }
            }
            else
            {
                // There were no existing memberships. Create new ones. This is the PPSE path, so create a new membership for just PPSE.

                // Verify security token.

                string tokenBase = person.Name.Replace(" ", "-") + person.PasswordHash + "-" +
                                   DateTime.Today.Year.ToString();
                string expectedSecurityHash = SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8);

                if (Request.QueryString["SecHash"] == expectedSecurityHash)
                {

                    Membership.Create(person, Organization.PPSE, DateTime.Today.AddYears(1));
                    Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage,
                                                                 EventType.ReceivedMembershipPayment,
                                                                 person.Identity, Organization.PPSEid,
                                                                 person.Geography.Identity, person.Identity, 0,
                                                                 Request.UserHostAddress);
                    PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed,
                                "A renewed membership in " + Organization.PPSE.NameShort + " was signed up.",
                                "Membership was created from IP " + Request.UserHostAddress + ".");
                    success = true;
                }
            }
        }

        string errorType = "";
        try
        {
            // This is the new path - we expect all renewals from Dec 14, 2010 and onward to follow this. Delete the old path (with "MemberId") some time February 2011.

            if (Int32.TryParse(Request.QueryString["PersonId"], out personId))
            {
                // Ok, at least we have  a valid person id.

                person = Person.FromIdentity(personId);

                string transferString = Request.QueryString["Transfer"];
                Membership membership = null;
                DateTime newExpiry = DateTime.MinValue; ;
                int transferOldOrgId = 0;
                int transferNewOrgId = 0;
                try
                {
                    if (Request.QueryString["MembershipId"] != null)
                    {
                        membership = Membership.FromIdentity(Int32.Parse(Request.QueryString["MembershipId"]));
                        newExpiry = membership.Expires;

                        //do not mess with lifetime memberships (100 years)
                        if (newExpiry < DateTime.Today.AddYears(10))
                        {
                            newExpiry = newExpiry.AddYears(1);
                            if (newExpiry > DateTime.Now.AddYears(1))
                            {
                                newExpiry = DateTime.Now.AddYears(1).AddDays(1);
                            }
                        }

                        if (membership.OrganizationId == Organization.PPSEid)
                        {
                            redirectUrl = "http://www.piratpartiet.se/fornyelse"; // HACK, have an org parameter for this later
                        }

                        if (membership.PersonId != personId)
                        {
                            throw new ArgumentException("Mismatching membership and person");
                        }
                    }
                    else if (Request.QueryString["Transfer"] != null)
                    {
                        if (!string.IsNullOrEmpty(transferString))
                        {
                            string[] tokens = transferString.Split(',');
                            transferOldOrgId = Int32.Parse(tokens[0]);
                        }
                        membership = person.GetRecentMembership(Membership.GracePeriod, transferOldOrgId);
                        if (membership == null)
                        {
                            throw new ArgumentException("Can't find membership");
                        }
                        //do not mess with lifetime memberships (100 years)
                        if (membership.Expires < DateTime.Today.AddYears(10))
                        {
                            if (membership.Expires > DateTime.Today.AddYears(1))
                                newExpiry = membership.Expires;
                            else
                                newExpiry = DateTime.Today.AddYears(1);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("No MembershipId given");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("No Membership found: " + ex.Message, ex);
                }

                if (!membership.Active)
                {
                    if (membership.DateTerminated.AddDays(Membership.GracePeriod) < DateTime.Today)
                    {
                        throw new ArgumentException("Membership has expired beyond recovery");
                    }

                    newExpiry = DateTime.Today.AddYears(1);

                    if (membership.OrganizationId == Organization.PPSEid)
                    {
                        redirectUrl = "http://www.piratpartiet.se/fornyelse"; // HACK, have an org parameter for this later
                    }
                }


                string tokenBase = person.PasswordHash + "-" + membership.Identity.ToString() + "-" +
                                   membership.Expires.Year.ToString();


                if (!string.IsNullOrEmpty(transferString))
                {
                    string[] tokens = transferString.Split(',');
                    transferOldOrgId = Int32.Parse(tokens[0]);
                    transferNewOrgId = Int32.Parse(tokens[1]);

                    tokenBase += "-Transfer" + transferOldOrgId.ToString() + "/" + transferNewOrgId.ToString();
                }

                if (transferOldOrgId != 0 && transferOldOrgId != membership.OrganizationId)
                {
                    throw new ArgumentException("Invalid Transfer Order");
                }

                string expectedHash = SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8);

                if (Request.QueryString["SecHash"] == expectedHash)
                {
                    success = true;

                    this.LabelExpires.Text = newExpiry.ToString("yyyy-MM-dd");
                    int orgMembershipToExtend = membership.OrganizationId;


                    if (transferOldOrgId > 0)
                    {
                        Membership oldMembership = person.GetRecentMembership(Membership.GracePeriod, transferOldOrgId);
                        Membership newMembership = membership;
                        // Might have been terminated already
                        if (oldMembership != null)
                        {
                            newMembership = Membership.Create(person.Identity, transferNewOrgId, oldMembership.Expires);

                            //Terminate logs and creates appropriate events
                            oldMembership.Terminate(EventSource.SignupPage, person,
                                                    "Membership in " + oldMembership.Organization.NameShort +
                                                    " was transferred to " + newMembership.Organization.NameShort +
                                                    " while renewing.");

                            membership = newMembership;
                        }
                        this.LabelTransferOldOrganization.Text = Organization.FromIdentity(transferOldOrgId).Name;
                        this.LabelTransferNewOrganization.Text = Organization.FromIdentity(transferNewOrgId).Name;
                        this.PanelTransferred.Visible = true;

                        orgMembershipToExtend = transferNewOrgId;
                    }

                    Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage,
                                                                 EventType.ReceivedMembershipPayment,
                                                                 person.Identity, orgMembershipToExtend,
                                                                 person.Geography.Identity, person.Identity, 0,
                                                                 Request.UserHostAddress);

                    PWLog.Write(person.Identity, PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed,
                                "Membership in " + membership.Organization.NameShort + " renewed.",
                                "Membership was renewed from IP " + Request.UserHostAddress + ".");

                    this.LiteralRenewedOrganizations.Text = Organization.FromIdentity(orgMembershipToExtend).Name;
                }
            }
        }
        catch (ArgumentException e2)
        {
            // Something failed parsing the parameters. Do not renew.
            errorType = e2.Message;
        }


        if (!success)
        {
            // Send a couple mails
            PWLog.Write(person.Identity, PWLogItem.Person, person.Identity, PWLogAction.Failure, "Tech failure on membership renewal: " + Request.RawUrl + " Errormessage:" + errorType, "Renewal attempt from from IP " + Request.UserHostAddress + ".");

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
                person.Name + " (#" + person.Identity.ToString() + ") f\xF6rs\xF6kte f\xF6rnya medlemskapet. Det misslyckades:" + errorType +
                "Ett mail har skickats ut.\r\n", Organization.PPSEid);
        }
        else
        {
            this.PanelSuccess.Visible = success;
            this.PanelFail.Visible = !success;

            if (!String.IsNullOrEmpty(redirectUrl))
            {
                Response.Redirect(redirectUrl);
            }
        }

    }
}

