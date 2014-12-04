using System;

namespace Swarmops.Controls.Financial
{
    public partial class ComboBudgets : ControlV5Base
    {
        public enum AccountListType
        {
            Unknown = 0,
            Expensable,
            InvoiceableOut,
            InvoiceableIn,
            All
        }

        public string OnClientLoaded { get; set; }
        public string OnClientSelect { get; set; }

        public AccountListType ListType { get; set; }

        protected string DataUrl
        {
            get
            {
                switch (ListType)
                {
                    case AccountListType.Unknown:
                    case AccountListType.Expensable:
                    default:
                        return "/Automation/Json-BudgetsTree.aspx?AccountType=Expensable";
                    case AccountListType.InvoiceableOut:
                        return "/Automation/Json-BudgetsTree.aspx?AccountType=InvoiceableOut";
                    case AccountListType.All:
                        return "/Automation/Json-FinancialAccountsTree.aspx?AccountType=All";
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}