using System;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Membership = Activizr.Logic.Pirates.Membership;

namespace Controls.v4.Financial
{
    public partial class ListBudgets : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FinancialAccounts allOwned = new FinancialAccounts();

            Memberships memberships = this.person.GetMemberships();

            int orgOwned = 0;
            bool multipleOrgs = false;

            foreach (Membership membership in memberships)
            {
                FinancialAccounts orgAccounts = FinancialAccounts.ForOrganization(membership.Organization);
                foreach (FinancialAccount account in orgAccounts)
                {
                    if (account.OwnerPersonId == this.person.Identity)
                    {
                        allOwned.Add(account);

                        if (orgOwned == 0 || orgOwned == membership.OrganizationId)
                        {
                            orgOwned = membership.OrganizationId;
                        }
                        else
                        {
                            multipleOrgs = true;
                        }
                    }
                }
            }

            if (allOwned.Count > 0)
            {

                string literal =
                    "<span style=\"line-height:150%\"><table border=\"0\" cellspacing=\"0\" width=\"100%\">";

                int year = DateTime.Today.Year;
                DateTime now = DateTime.Now;
                DateTime jan1 = new DateTime(year, 1, 1);
                bool budgetFound = false;

                foreach (FinancialAccount account in allOwned)
                {
                    double budget = -account.GetBudget(year);
                    decimal spent = account.GetDelta(jan1, now);
                    decimal remaining = (decimal) budget - spent;

                    if (budget == 0.0 && spent == 0.0m)
                    {
                        continue; // not interesting
                    }

                    budgetFound = true;
                    literal += "<tr><td>";

                    if (multipleOrgs)
                    {
                        literal += Server.HtmlEncode(account.Organization.NameShort) + " - ";
                    }

                    literal += "<a href=\"/Pages/v4/Financial/ManageBudget.aspx?AccountId=" + account.Identity + "\">" + Server.HtmlEncode(account.Name) + "</a>&nbsp;</td><td align=\"right\">";

                    literal += Server.HtmlEncode(remaining.ToString("N2"));

                    literal += "</td><td>&nbsp;of&nbsp;</td><td align=\"right\">";

                    literal += Server.HtmlEncode(budget.ToString("N2"));

                    literal += "</td><td align=\"right\">&nbsp;(";

                    if (budget != 0.0)
                    {
                        literal += (remaining/(decimal) budget).ToString("P0");
                    }
                    else
                    {
                        literal += "--%";
                    }

                    literal += " left)</td><tr>\r\n";

                }

                if (!budgetFound)
                {
                    literal += "<tr><td>No budget allocated to you (yet?) for " + year.ToString() + ".</td></tr>\r\n";
                }

                literal +=
                    "<tr><td>&nbsp;</td></tr><tr><td colspan=\"5\" align=\"right\"><a href=\"/Pages/v4/Financial/AllocateFunds.aspx\">Allocate funds...</a></td></tr>\r\n";

                literal += "</table></span>";

                this.LiteralBudgetData.Text = literal;
            }
            else
            {
                this.LiteralBudgetData.Text = "You own no budgets.";
            }
        }

        public Person Person
        {
            get { return this.person; }
            set { this.person = value; }
        }

        private Person person;

    }
}
