using System;
using System.Web.UI;
using Swarmops.Common.Enums;
using Swarmops.Frontend;
using Swarmops.Logic.Financial;
using WebSocketSharp;

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
                    case AccountListType.InvoiceableIn:
                        return "/Automation/Json-BudgetsTree.aspx?AccountType=InvoiceableIn";
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

        public FinancialAccount SelectedAccount
        {
            get
            {
                if (!IsPostBack)
                {
                    throw new InvalidOperationException("SelectedAccount can only be retrieved in postback");
                }

                string accountIdString = Request.Form[this.ClientID + "_DropBudgets"];
                if (accountIdString.IsNullOrEmpty())
                {
                    throw new InvalidOperationException("ComboBudgets control submitted a null value");
                }
                int accountId = Int32.Parse(accountIdString);
                FinancialAccount account = FinancialAccount.FromIdentity(accountId);

                return account;
            }
        }

        public string Localized_DropInit
        {
            get { return CommonV5.JavascriptEscape(Resources.Global.Global_DropInits_SelectFinancialAccount); }
        }
    }
}