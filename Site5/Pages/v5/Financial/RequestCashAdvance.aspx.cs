using System;
using System.Globalization;
using System.Web;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class RequestCashAdvance : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageTitle = Resources.Pages_Financial.RequestCashAdvance_PageTitle;
            this.PageIcon = "iconshock-walletmoney";
            this.InfoBoxLiteral = Resources.Pages_Financial.RequestCashAdvance_Info;

            if (!Page.IsPostBack)
            {
                // Prime bank details

                this.TextBank.Text = this.CurrentUser.BankName;
                this.TextClearing.Text = this.CurrentUser.BankClearing;
                this.TextAccount.Text = this.CurrentUser.BankAccount;
                this.TextAmount.Text = 0.ToString("N2");
                this.TextAmount.Focus();

                Localize();
            }
        }


        private void Localize()
        {
            this.LabelAmount.Text = string.Format(Resources.Pages_Financial.RequestCashAdvance_Amount,
                                                  CurrentOrganization.Currency.Code);
            this.LabelPurpose.Text = Resources.Pages_Financial.RequestCashAdvance_Purpose;
            this.LabelBudget.Text = Resources.Pages_Financial.RequestCashAdvance_Budget;
            this.LabelHeaderBankDetails.Text = Resources.Pages_Financial.RequestCashAdvance_HeaderBankDetails;
            this.LabelBankName.Text = Resources.Pages_Financial.RequestCashAdvance_BankName;
            this.LabelBankClearing.Text = Resources.Pages_Financial.RequestCashAdvance_BankClearing;
            this.LabelBankAccount.Text = Resources.Pages_Financial.RequestCashAdvance_BankAccount;

            this.LiteralErrorAmount.Text = Resources.Pages_Financial.RequestCashAdvance_ValidationError_Amount;
            this.LiteralErrorPurpose.Text = Resources.Pages_Financial.RequestCashAdvance_ValidationError_Purpose;
            this.LiteralErrorBudget.Text = Resources.Pages_Financial.RequestCashAdvance_ValidationError_Budget;
            this.LiteralErrorBankName.Text = Resources.Pages_Financial.RequestCashAdvance_ValidationError_BankName;
            this.LiteralErrorBankClearing.Text = Resources.Pages_Financial.RequestCashAdvance_ValidationError_BankClearing;
            this.LiteralErrorBankAccount.Text = Resources.Pages_Financial.RequestCashAdvance_ValidationError_BankAccount;
        }


        protected void ButtonRequest_Click(object sender, EventArgs e)
        {
            // The data has been validated client-side already. We'll throw unfriendly exceptions if invalid data is passed here.

            double amount = Double.Parse(this.TextAmount.Text, NumberStyles.Number);
                // parses in current culture - intentional
            Int64 amountCents = (Int64) amount*100;

            string description = this.TextPurpose.Text;

            FinancialAccount budget = FinancialAccount.FromIdentity(Int32.Parse(this.Request.Form["DropBudgets"]));

            // sanity check

            if (budget.Organization.Identity != CurrentOrganization.Identity)
            {
                throw new InvalidOperationException("Budget-organization mismatch; won't file cash advance");
            }

            // Store bank details for current user

            CurrentUser.BankName = this.TextBank.Text;
            CurrentUser.BankClearing = this.TextClearing.Text;
            CurrentUser.BankAccount = this.TextAccount.Text;

            // Create cash advance

            CashAdvance cashAdvance = CashAdvance.Create(CurrentOrganization, CurrentUser, CurrentUser, amountCents,
                                                         budget,
                                                         description);

            // Create success message

            string successMessage = string.Format(Resources.Pages_Financial.RequestCashAdvance_SuccessMessagePartOne,
                                                  HttpUtility.HtmlEncode(CurrentUser.Name),
                                                  HttpUtility.HtmlEncode(description), CurrentOrganization.Currency.Code,
                                                  (double) (amountCents/100.0));

            if (budget.OwnerPersonId != CurrentUser.Identity)
            {
                successMessage += "<br/><br/>" + Resources.Pages_Financial.RequestCashAdvance_SuccessMessagePartTwo +
                                  "<br/>";
            }
            else
            {
                successMessage += "<br/><br/>" +
                                  Resources.Pages_Financial.RequestCashAdvance_SuccessMessagePartTwoOwnBudget +
                                  "<br/>";
                cashAdvance.Attest (CurrentUser);
            }

            Response.AppendCookie(new HttpCookie("DashboardMessage", HttpUtility.UrlEncode(successMessage)));

            // Redirect to dashboard

            Response.Redirect("/", true);
        }
    }
}