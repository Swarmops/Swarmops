using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Pages.Financial
{
    public partial class PayOutMoney : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-money-envelope";

            if (!Page.IsPostBack)
            {
                Localize();
            }
        }

        private void Localize()
        {
            this.PageTitle = Resources.Pages.Financial.PayOutMoney_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Financial.PayOutMoney_Info;
            this.LabelGridHeaderAccount.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_BankAccount;
            this.LabelGridHeaderAmount.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Amount;
            this.LabelGridHeaderBank.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_BankName;
            this.LabelGridHeaderDue.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_DueDate;
            this.LabelGridHeaderPaid.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_PaidOut;
            this.LabelGridHeaderRecipient.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Recipient;
            this.LabelGridHeaderReference.Text = Resources.Pages.Financial.PayOutMoney_GridHeader_Reference;

        }
    }
}