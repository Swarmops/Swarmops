using System;
using System.Collections;
using System.Collections.Generic;
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
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;
using Membership = Activizr.Logic.Pirates.Membership;

public partial class Pages_v4_Communications_SmsInboxSweden : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PopulateGrid();
    }


    protected void PopulateGrid()
    {
        SupportCase[] cases = SupportDatabase.GetOpenCases();
        List<InboxItem> data = new List<InboxItem>();

        foreach (SupportCase supportCase in cases)
        {
            if (supportCase.Title.StartsWith("SMS ") && supportCase.Title.Contains("'+"))
            {
                string phoneNumber = ExtractPhoneFromTitle(supportCase.Title);

                if (String.IsNullOrEmpty(phoneNumber))
                {
                    continue;
                }

                People matchingPeople = GetPeopleFromPhone(phoneNumber);
                
                string rawMail = SupportDatabase.GetFirstEventText(supportCase.Identity);
                string personCanonical = "No match";
                string message = "Not found";

                if (matchingPeople.Count > 1)
                {
                    personCanonical = "Several";
                }
                else if (matchingPeople.Count == 1)
                {
                    personCanonical = matchingPeople[0].Canonical;
                }

                string decodedMail = DecodeMail(rawMail);

                string[] mailLines = decodedMail.Split("\n".ToCharArray());

                foreach (string mailLine in mailLines)
                {
                    if (mailLine.StartsWith("Meddelande: "))
                    {
                        message = mailLine.Substring(12).Trim();
                    }
                }

                data.Add(new InboxItem(supportCase.Identity, phoneNumber, message, personCanonical));
            }
        }

        this.GridInbox.DataSource = data;
    }


    static string ExtractPhoneFromTitle (string title)
    {
        int indexStart = title.IndexOf("'+");
        int indexEnd = title.LastIndexOf('\'');

        if (indexStart >= indexEnd)
        {
            return string.Empty;
        }

        return title.Substring(indexStart + 1, (indexEnd - indexStart - 1));
    }

    static People GetPeopleFromPhone (string phone)
    {
        People resultFirst = People.FromPhoneNumber("SE", phone);

        return People.LogicalOr(resultFirst,
                                          People.FromPhoneNumber("SE", phone.Replace("+46", "0")));
    }


    static string DecodeMail (string rawMail)  // cheat and assume utf-8 base64
    {
        string[] rawMailLines = rawMail.Split('\n');

        // First empty line is split between headers and mail

        List<string> headers = new List<string>();
        string body = string.Empty;
        bool discoveredSplitter = false;

        foreach (string rawMailLine in rawMailLines)
        {
            if (rawMailLine.Trim().Length == 0)
            {
                discoveredSplitter = true;
            }
            else if (discoveredSplitter)
            {
                body += rawMailLine + "\n";
            }
            else
            {
                headers.Add(rawMailLine.TrimEnd());
            }
        }

        // Convert from Base64 (assumption here)

        byte[] charsetUndecoded = Convert.FromBase64String(body);
        string decoded = System.Text.Encoding.UTF8.GetString(charsetUndecoded);

        return decoded;
    }


    protected void GridInbox_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            InboxItem item = e.Item.DataItem as InboxItem;

            if (item == null)
            {
                return;
            }

            DropDownList listActions = (DropDownList)e.Item.FindControl("DropActions");

            if (item.PhoneNumber.EndsWith("708303600"))
            {
                listActions.SelectedIndex = 0; // Debug purposes
            }
            else if (item.Message.ToLowerInvariant().Trim() == "pp igen")
            {
                listActions.SelectedIndex = 1;
            }
            else
            {
                listActions.SelectedIndex = 0;//On request from member service
            }
        }
    }


    public class InboxItem
    {
        public InboxItem (int caseId, string phoneNumber, string message, string personCanonical)
        {
            this.caseId = caseId;
            this.phoneNumber = phoneNumber;
            this.message = message;
            this.personCanonical = personCanonical;
        }

        public int CaseId { get { return this.caseId; }}
        public string PhoneNumber { get { return this.phoneNumber; }}
        public string PersonCanonical { get { return this.personCanonical; } }
        public string Message { get { return this.message; }}

        private int caseId;
        private string phoneNumber;
        private string message;
        private string personCanonical;
    }



    protected void ButtonPerformActions_Click(object sender, EventArgs e)
    {
        // Iterate through the grid, find the datakey and the selected action, and perform them.
    
        foreach (GridDataItem item in this.GridInbox.Items)
        {
            DropDownList listActions = (DropDownList)item.FindControl("DropActions");

            if (listActions == null)
            {
                continue;
            }

            int caseId = (int) item.GetDataKeyValue("CaseId");

            SupportCase openCase = SupportDatabase.GetCase(caseId);

            string phone = ExtractPhoneFromTitle(openCase.Title);
            People people = GetPeopleFromPhone(phone);
            bool closeCase = true;

            foreach (Person person in people)
            {
                string selectedValue = listActions.SelectedValue;

                switch (selectedValue)
                {
                    case "TerminateAll":
                        // terminate everything: all activistships, all memberships

                        if (person.IsActivist)
                        {
                            person.TerminateActivist();
                            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.ActivistLost, "Activist Lost",
                                        "SMS message from phone# " + phone + " declined all further contact");
                        }

                        Memberships terminateMemberships = person.GetMemberships();

                        foreach (Membership membership in terminateMemberships)
                        {
                            membership.Terminate(EventSource.SMS, _currentUser, "Membership in " + membership.Organization.Name + " terminated after person declined further contact over SMS.");
                        }

                        break;
                    case "RenewAll":
                        Memberships renewedMemberships = person.GetRecentMemberships(Membership.GracePeriod);

                        // Get PPSE new expiry date, use as master

                        DateTime masterNewExpiry = DateTime.Today.AddYears(1);

                        foreach (Membership membership in renewedMemberships)
                        {
                            if (membership.OrganizationId == Organization.PPSEid)
                            {
                                if (membership.Expires > DateTime.Now)
                                {
                                    // Set to one year from today or one year from previous expiry, whichever is greater

                                    masterNewExpiry = membership.Expires.AddYears(1);
                                }
                            }
                        }

                        foreach (Membership renewedMembership in renewedMemberships)
                        {
                            renewedMembership.Expires = masterNewExpiry;

                            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SMS, EventType.ReceivedMembershipPayment,
                                                                            person.Identity, renewedMembership.OrganizationId,
                                                                            person.Geography.Identity, person.Identity, 0, Request.UserHostAddress);

                            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MembershipRenewed,
                                        "Membership in " + renewedMembership.Organization.NameShort + " renewed.",
                                        "Membership was renewed over SMS from phone# " + phone + " and will expire " + masterNewExpiry.ToString("yyyy-MM-dd") + ".");
                        }

                        // Renew all memberships. Easist done by logging an event of repayment
                        break;
                    default:
                        closeCase = false;
                        break;
                }

                if (closeCase)
                {
                    openCase.CloseWithComment("Handled from PirateWeb interface, using option " + selectedValue + ".");
                }
            }
        }

        PopulateGrid();
        this.GridInbox.Rebind();
    }
}



