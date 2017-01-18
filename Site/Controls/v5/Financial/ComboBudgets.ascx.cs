using System;
using Swarmops.Common.Enums;
using Swarmops.Frontend;

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

        public bool SuppressPrompt { get; set; }

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

        protected void Page_Init (object sender, EventArgs e)
        {
            ((PageV5Base) this.Page).RegisterControl (EasyUIControl.Tree | EasyUIControl.Combo | EasyUIControl.ComboBox);
        }

        public LayoutDirection Layout { get; set; }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (this.Layout == LayoutDirection.Unknown)
            {
                this.Layout = LayoutDirection.Vertical;
            }
        }
    }
}