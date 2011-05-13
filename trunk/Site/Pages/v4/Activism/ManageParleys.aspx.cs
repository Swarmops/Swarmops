using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

namespace Activizr.Site.Pages.v4.Activism
{
    public partial class ManageParleys : PageV4Base
    {
        protected void Page_Init (object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ViewState[this.ClientID + "_ParleyId"] != null)
            {
                int parleyId = Int32.Parse((string)ViewState[this.ClientID + "_ParleyId"]);
                _parley = Parley.FromIdentity(parleyId);

                CreateDynamicControls();
            }
        }

        private Parley _parley;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PopulateControls();
            }
        }


        private void PopulateControls()
        {
            Parleys parleys = Parleys.ForOwner(_currentUser);

            this.DropParleys.Items.Clear();

            foreach (Parley parley in parleys)
            {
                this.DropParleys.Items.Add(new ListItem(parley.Name + " / " + parley.Organization.Name,
                                                        parley.Identity.ToString()));
            }

            bool selectedParley = false;
            string accountParameterString = Request.QueryString["ParleyId"];

            if (!String.IsNullOrEmpty(accountParameterString))
            {
                int parleyId = Int32.Parse(accountParameterString);

                if (FinancialAccount.FromIdentity(parleyId).OwnerPersonId == _currentUser.Identity)
                {
                    this.DropParleys.SelectedValue = parleyId.ToString();
                }
            }

            if (this.DropParleys.SelectedIndex >= 0)
            {
                ViewState[this.ClientID + "_ParleyId"] = this.DropParleys.SelectedValue;
                PopulateParleyData();
            }
        }


        private void PopulateParleyData()
        {
            int parleyId = Int32.Parse(this.DropParleys.SelectedValue);
            _parley = Parley.FromIdentity(parleyId);

            Ledger.Accounts = FinancialAccounts.FromSingle(_parley.Budget);
            int year = DateTime.Today.Year;

            Ledger.DateStart = _parley.CreatedDateTime.Date;
            Ledger.DateEnd = new DateTime(year, 12, 31);
            Ledger.MaxAmount = 1.0e12m;

            Ledger.Populate();

            CreateDynamicControls();
        }

        private void CreateDynamicControls()
        {
            this.PlaceHolderSummary.Controls.Clear();
            this.PlaceHolderGuests.Controls.Clear();

            string tableStart =
                "<table border=\"0\" cellpadding=\"0\" cellspacing=\"1\" width=\"100%\" style=\"line-height:120%\">";

            this.PlaceHolderSummary.Controls.Add(GetLiteral(tableStart));
            this.PlaceHolderGuests.Controls.Add(GetLiteral(tableStart));

            ParleyAttendees attendees = _parley.Attendees;
            ParleyOptions parleyOptions = _parley.Options;

            int activeAttendeeCount = 0;

            Dictionary<int, int> optionLookup = new Dictionary<int, int>();

            foreach (ParleyAttendee attendee in attendees)
            {
                if (attendee.Active)
                {
                    activeAttendeeCount++;

                    bool paid = false;

                    if (attendee.Invoiced && attendee.Invoice.Open == false)
                    {
                        paid = true;
                    }

                    this.PlaceHolderGuests.Controls.Add(
                        GetTableRow(new string[] {attendee.Person.Name, paid ? "ok" : "--"}));

                    ParleyOptions attendeeOptions = attendee.Options;
                    foreach (ParleyOption attendeeOption in attendeeOptions)
                    {
                        if (!optionLookup.ContainsKey(attendeeOption.Identity))
                        {
                            optionLookup[attendeeOption.Identity] = 0;
                        }

                        optionLookup[attendeeOption.Identity]++;
                    }
                }
            }

            this.PlaceHolderSummary.Controls.Add(GetTableRow(new string[] { "Attendees", activeAttendeeCount.ToString() }));


            this.PlaceHolderGuests.Controls.Add(GetLiteral("</table>"));

            foreach (ParleyOption parleyOption in parleyOptions)
            {
                if (optionLookup.ContainsKey(parleyOption.Identity))
                {
                    this.PlaceHolderSummary.Controls.Add(GetTableRow(new string[] { parleyOption.Description, optionLookup[parleyOption.Identity].ToString() } ));
                }
                else
                {
                    this.PlaceHolderSummary.Controls.Add(GetTableRow(new string[] { parleyOption.Description, "0" } ));
                }
            }

            this.PlaceHolderSummary.Controls.Add(GetLiteral("</table>"));
        }


        private Literal GetTableRow (string[] texts)
        {
            string literalText = string.Empty;

            foreach (string text in texts)
            {
                literalText += "<td>" + Server.HtmlEncode(text) + "</td>";
            }

            return GetLiteral("<tr>" + literalText + "</tr>");
        }


        private Literal GetLiteral (string text)
        {
            Literal literal = new Literal();
            literal.Text = text;
            return literal;
        }


        protected void ButtonSelectConference_Click(object sender, EventArgs e)
        {
            ViewState[this.ClientID + "_ParleyId"] = this.DropParleys.SelectedValue;
            PopulateParleyData();
        }
    }
}