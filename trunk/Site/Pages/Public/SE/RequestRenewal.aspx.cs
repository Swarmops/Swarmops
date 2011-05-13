using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Structure;
using Activizr.Logic.Pirates;
using Activizr.Logic.Communications;
using Activizr.Basic.Enums;
using Activizr.Logic.Security;
using Activizr.Logic.Support;
using System.Net.Mail;
using System.Text;
using System.Drawing;

public partial class Pages_Public_SE_RequestRenewal : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        LabelReply.Text = "";
        LabelReply.Font.Name = "Arial";
        LabelReply.Font.Bold = true;
        LabelReply.ForeColor = Color.Black;
        LabelReply.Font.Size = FontUnit.Medium;

    }

    protected void Button1_Click (object sender, EventArgs e)
    {

        if (!Formatting.ValidateEmailFormat(TextBoxMail.Text))
        {
            LabelReply.Text = "Fel format p� e-Mail adress";
            LabelReply.Font.Bold = true;
            LabelReply.ForeColor = Color.Red;
            LabelReply.Font.Size = FontUnit.Medium;
            return;
        }

        if (CheckBoxRenew.Checked == false)
        {
            LabelReply.Text = "Kryssa f�r att du vill vara medlem!";
            LabelReply.Font.Bold = true;
            LabelReply.ForeColor = Color.Red;
            LabelReply.Font.Size = FontUnit.Medium;
            return;
        }

        if (TextBoxName.Text.Trim() == "")
        {
            LabelReply.Text = "Ange namn!";
            LabelReply.Font.Bold = true;
            LabelReply.ForeColor = Color.Red;
            LabelReply.Font.Size = FontUnit.Medium;
            return;
        }





        People members = People.FromEmail(TextBoxMail.Text);
        Person person = null;
        DateTime newExpiry = DateTime.MinValue;
        bool sentMail = true;

        try
        {
            Memberships membershipsToRenew = new Memberships();
            string foundMemberIds = "";
            try
            {
                DateTime currentExpiry = DateTime.MinValue;
                if (members.Count > 0)
                {
                    person = members[0];
                    DateTime latest = DateTime.MinValue;

                    Memberships personMS = person.GetRecentMemberships(Membership.GracePeriod);
                    if (personMS != null && personMS.Count > 0)
                    {
                        membershipsToRenew = personMS;
                        latest = personMS[0].Expires;
                    }

                    foreach (Person p in members)
                    {
                        foundMemberIds += "," + p.Identity;
                        personMS = p.GetRecentMemberships(Membership.GracePeriod);

                        if (personMS != null
                            && personMS.Count > 0
                            && personMS[0].Expires > latest)
                        {
                            membershipsToRenew = personMS;
                            person = p;
                            latest = personMS[0].Expires;
                        }
                    }
                }
            }
            finally
            {
                PWLog.Write(PWLogItem.None, 0, PWLogAction.MembershipRenewalRequest, "MembershipRenewalRequest recieved |" + TextBoxMail.Text + "|" + TextBoxName.Text, "Found " + members.Count + "Persons" + foundMemberIds);
            }
            if (membershipsToRenew.Count > 0)
            {
                //Don't need to extend these, it will be handled in the renewalpage

                //newExpiry = DateTime.Today.AddDays(6); //6 days to avoid renewal mail (just in case)
                //foreach (Membership ms in membershipsToRenew)
                //{
                //    if (ms.Expires < DateTime.Now)
                //    {
                //        PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed, "Membership in " + ms.Organization.NameShort + " extended temporarily on delayed renewal", "Membership was renewed from IP " + Request.UserHostAddress + ".");
                //        ms.Expires = newExpiry;
                //    }
                //}
                SendReminderMail(person, membershipsToRenew);

            }
            else
            {
                //No memberships

                sentMail = SendMail(
                    FunctionalMail.Address[MailAuthorType.MemberService].Name,
                    FunctionalMail.Address[MailAuthorType.MemberService].Email,
                    TextBoxName.Text,
                    TextBoxMail.Text,
                    "Vill du f�rnya?",
                    @"
Alldeles nyss f�rs�kte n�gon (troligtvis du) f�rnya ditt medlemskap i Piratpartiet och/eller Ung Pirat.

Tyv�rr kunde vi inte hitta din mailadress i v�rt medlemsregister, s� antingen var det fel adress, eller s�
har vi redan hunnit rensa bort dina medlemsuppgifter.

Om du misst�nker att det kan ha varit fel mailadress, att den adress vi har registrerad �r en annan, 
pr�va g�rna med en annan p� https://pirateweb.net/Pages/Public/SE/RequestRenewal.aspx

Annars, om det har g�tt en tid sedan ditt medlemskap l�pte ut, registrera dig p� nytt
p� http://blipirat.nu/

Du �r v�lkommen, du beh�vs!

mvh

Piratpartiet");
            }
        }
        catch
        {
            sentMail = false;
        }
        if (sentMail)
        {
            Panel1.Visible = false;
            LabelReply.Text = "Vi har skickat ett mail med en f�rnyelsel�nk.<br><br>Kolla din mailbox.";
        }
        else
            LabelReply.Text = "PROGRAMFEL: Lyckades inte skicka mail. Kontakta medlemsservice@piratpartiet.se ist�llet och be att de hj�lper dig att f�rnya ditt medlemskap.";

    }


    public static void SendReminderMail (Person person, Memberships memberships)
    {
        // First, determine the organization template to use. Prioritize a long ancestry.

        // This is a hack for the Swedish structure.

        ReminderMail remindermail = new ReminderMail();


        int longestAncestry = 0; // "ancestry" as a length means distance to organization tree root
        int shortestAncestry = 999;
        Organization topOrg = null;
        Organization lowOrg = null;
        DateTime currentExpiry = DateTime.MinValue;

        foreach (Membership membership in memberships)
        {
            if (membership.Organization.AutoAssignNewMembers)
            {
                Organizations ancestry = membership.Organization.GetLine();

                if (ancestry.Count > longestAncestry)
                {
                    longestAncestry = ancestry.Count;
                    lowOrg = membership.Organization;
                    remindermail.pOrgName = lowOrg.MailPrefixInherited;
                }

                if (ancestry.Count < shortestAncestry)
                {
                    shortestAncestry = ancestry.Count;
                    topOrg = membership.Organization;
                }
            }
            if (membership.OrganizationId == Organization.PPSEid)
            {
                topOrg = membership.Organization;
                remindermail.pOrgName = membership.Organization.MailPrefixInherited;
                currentExpiry = membership.Expires;
            }
        }

        DateTime newExpiry = currentExpiry;
        if (newExpiry < DateTime.Today.AddYears(10))
        {
            //do not mess with lifetime memberships
            newExpiry = newExpiry.AddYears(1);
            if (newExpiry > DateTime.Today.AddYears(1))
                newExpiry = DateTime.Today.AddYears(1).AddDays(1);
        }


        //Person sender = Person.FromIdentity(1); //Rick

        remindermail.pExpirationDate = currentExpiry;
        remindermail.pNextDate = newExpiry;
        remindermail.pPreamble = "<p> Vi �r glada att du vill <strong>f�rnya ditt medlemskap</strong> i Piratpartiet och/eller Ung Pirat.<br /><br />Anv�nd en av l�nkarna nedan s� genomf�rs f�rnyelsen.<br />";

        string tokenBase = person.PasswordHash + "-" + currentExpiry.Year.ToString();

        Organization expectedLowOrg = Organizations.GetMostLocalOrganization(person.GeographyId, Organization.UPSEid);

        int ageThisYear = DateTime.Now.Year - person.Birthdate.Year;

        //Hardcoded: age = 26
        if (ageThisYear >= 26 && lowOrg.Inherits(Organization.UPSEid))
        {
            // If this person is older than 26, suggest that they leave UP.

            remindermail.pCurrentAge = ageThisYear.ToString();
            remindermail.pCurrentOrg = lowOrg.Name;
            remindermail.pOtherOrg = topOrg.Name;
            string link = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?MemberId=" +
                          person.Identity.ToString() + "&Leave=" + lowOrg.Identity.ToString() +
                          "&SecHash=" + SHA1.Hash(tokenBase + "-Leave" + lowOrg.Identity.ToString()).
                                            Replace(" ", "").Substring(0, 8);
            remindermail.pOtherRenewLink = link;
            remindermail.pWrongOrgSpan = " "; //clear the other span
        }

        else if (lowOrg.Inherits(Organization.UPSEid) && lowOrg.Identity != expectedLowOrg.Identity)
        {
            // Is this person in the wrong locale?

            remindermail.pCurrentOrg = lowOrg.Name;
            remindermail.pOtherOrg = expectedLowOrg.Name;
            remindermail.pGeographyName = person.Geography.Name;
            //mailBody += "Du �r medlem i " + lowOrg.Name + ", men n�r du bor i [b]" + person.Geography.Name +
            //             "[/b] s� rekommenderar " +
            //             "vi att du byter till din lokala organisation, [b]" + expectedLowOrg.Name +
            //             "[/b]. Klicka h�r f�r att g�ra det:\r\n\r\n";

            string link = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?MemberId=" +
                          person.Identity.ToString() + "&Transfer=" + lowOrg.Identity.ToString() + "," +
                          expectedLowOrg.Identity.ToString() +
                          "&SecHash=" + SHA1.Hash(tokenBase + "-Transfer" + lowOrg.Identity.ToString() + "/" +
                                                  expectedLowOrg.Identity.ToString()).Replace(" ", "").Substring(0, 8);
            remindermail.pOtherRenewLink = link;
            remindermail.pTooOldForYouthOrgSpan = " "; //clear the other span

            //mailBody += "[a href=\"" + link + "\"]" + link + "[/a]\r\n\r\n" +
            //            "Det �r naturligtvis inget krav, utan du kan forts�tta precis som f�rut om du vill. " +
            //            "F�r att forts�tta i dina befintliga f�reningar, klicka h�r:\r\n\r\n";
        }

        else
        {
            remindermail.pTooOldForYouthOrgSpan = " "; //clear the other span
            remindermail.pWrongOrgSpan = " "; //clear the other span
        }

        string stdLink = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?MemberId=" +
                         person.Identity.ToString() +
                         "&SecHash=" + SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8);

        remindermail.pStdRenewLink = stdLink;

        OutboundMail mail = remindermail.CreateFunctionalOutboundMail(MailAuthorType.MemberService, OutboundMail.PriorityNormal, topOrg, Geography.Root);
        if (mail.Body.Trim() == "")
        {
            throw new Exception("Failed to create a mailBody");
        }
        else
        {
            mail.AddRecipient(person.Identity, false);
            mail.SetRecipientCount(1);
            mail.SetResolved();
            mail.SetReadyForPickup();

            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewReminder, "Mail was sent to " + person.Email + " on request to renew membership.", string.Empty);

        }
    }



    private bool SendMail (
                        string fromName,
                        string fromAddress,
                        string toName,
                        string toAddress,
                        string subject,
                        string bodyPlain)
    {
        QuotedPrintable qpUTF8 = new QuotedPrintable(Encoding.UTF8);
        QuotedPrintable qp8859 = new QuotedPrintable(Encoding.GetEncoding("ISO-8859-1"));
        MailMessage message = null;


        message = new MailMessage(new MailAddress(fromAddress, qpUTF8.EncodeMailHeaderString(fromName), Encoding.UTF8),
                                  new MailAddress(toAddress, qpUTF8.EncodeMailHeaderString(toName), Encoding.UTF8));

        message.Subject = subject;
        message.Body = bodyPlain;

        message.SubjectEncoding = Encoding.UTF8;
        message.BodyEncoding = Encoding.UTF8;


        try
        {
            SmtpClient client = new SmtpClient("mail.piratpartiet.se", 587);
            client.Credentials = null;
            client.Send(message);
            return true;
        }

        catch (Exception)
        {
            return false;
        }

    }
}

