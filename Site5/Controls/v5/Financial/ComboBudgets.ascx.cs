using System;

namespace Swarmops.Controls.Financial
{
    public partial class ComboBudgets : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string OnClientLoaded { get; set; }
        public string OnClientSelect { get; set; }

        public AccountListType ListType { get; set; }

        protected string DataUrl
        {
            get
            {
                switch (this.ListType)
                {
                    case AccountListType.Unknown:
                    case AccountListType.Expensable:
                    default:
                        return "/Automation/Json-BudgetsTree.aspx?AccountType=Expensable";
                        break;
                    case AccountListType.InvoiceableOut:
                        return "/Automation/Json-BudgetsTree.aspx?AccountType=InvoiceableOut";
                        break;
                    case AccountListType.All:
                        return "/Automation/Json-FinancialAccountsTree.aspx?AccountType=All";
                }
            }
        }

        public enum AccountListType
        {
            Unknown = 0,
            Expensable,
            InvoiceableOut,
            InvoiceableIn,
            All
        }
    }
}