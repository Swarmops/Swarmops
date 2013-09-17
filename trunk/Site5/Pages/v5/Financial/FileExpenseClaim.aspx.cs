﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class FileExpenseClaim : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageTitle = Resources.Pages.Financial.FileExpenseClaim_PageTitle;
            this.PageIcon = "iconshock-moneyback";
            this.InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;

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
            this.LabelAmount.Text = string.Format(Resources.Pages.Financial.FileExpenseClaim_Amount,
                                                  CurrentOrganization.Currency.Code);
            this.LabelPurpose.Text = Resources.Pages.Financial.FileExpenseClaim_Description;
            this.LabelBudget.Text = Resources.Pages.Financial.FileExpenseClaim_Budget;
            this.LabelCostType.Text = Resources.Pages.Financial.FileExpenseClaim_CostType;
            this.LabelHeaderBankDetails.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderBankDetails;
            this.LabelBankName.Text = Resources.Pages.Financial.FileExpenseClaim_BankName;
            this.LabelBankClearing.Text = Resources.Pages.Financial.FileExpenseClaim_BankClearing;
            this.LabelBankAccount.Text = Resources.Pages.Financial.FileExpenseClaim_BankAccount;
            this.LabelHeaderImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_HeaderReceiptImages;
            this.LabelImageFiles.Text = Resources.Pages.Financial.FileExpenseClaim_UploadRecieptImages;

            this.LiteralErrorAmount.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_Amount;
            this.LiteralErrorPurpose.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_Purpose;
            this.LiteralErrorBudget.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_Budget;
            this.LiteralErrorBankName.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankName;
            this.LiteralErrorBankClearing.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankClearing;
            this.LiteralErrorBankAccount.Text = Resources.Pages.Financial.RequestCashAdvance_ValidationError_BankAccount;
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
            /*
            string successMessage = string.Format(Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartOne,
                                                  HttpUtility.HtmlEncode(CurrentUser.Name),
                                                  HttpUtility.HtmlEncode(description), CurrentOrganization.Currency.Code,
                                                  (double) (amountCents/100.0));

            if (budget.OwnerPersonId != CurrentUser.Identity)
            {
                successMessage += "<br/><br/>" + Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartTwo +
                                  "<br/>";
            }
            else
            {
                successMessage += "<br/><br/>" +
                                  Resources.Pages.Financial.FileExpenseClaim_SuccessMessagePartTwoOwnBudget +
                                  "<br/>";
                // TODO: Auto-attest
            }

            Response.AppendCookie(new HttpCookie("DashboardMessage", HttpUtility.UrlEncode(successMessage)));
            */

            // Redirect to dashboard

            Response.Redirect("/", true);
        }
    }
}