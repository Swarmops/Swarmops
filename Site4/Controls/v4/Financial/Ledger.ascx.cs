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
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Controls_v4_Financial_Ledger : System.Web.UI.UserControl
{
    protected void Page_Init (object sender, EventArgs e)
    {
        _accountNames = new Dictionary<int, string>();
        _peopleInitials = new Dictionary<int, string>();
        _accountInboundBalance = new Dictionary<int, decimal>();

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState[this.ClientID] == null)
        {
            _sessionObjectName = System.Guid.NewGuid().ToString();
            ViewState[this.ClientID] = _sessionObjectName;
            this.GridTransactions.DataSource = new LedgerRows();
        }
        else
        {
            _sessionObjectName = (string)ViewState[this.ClientID];
            _accountNames = (Dictionary<int, string>)Session["FinancialAccountNames"];
        }

        if (_simplifiedView)
        {
            this.GridTransactions.Columns[5].HeaderText = "Income";
            this.GridTransactions.Columns[6].HeaderText = "Expense";
            this.GridTransactions.PageSize = 12;
        }
    }

    private string _sessionObjectName;

    private Dictionary<int, string> _accountNames;
    private Dictionary<int, string> _peopleInitials;
    private Dictionary<int, decimal> _accountInboundBalance;

    private FinancialAccounts _accounts;
    private DateTime _dateStart;
    private DateTime _dateEnd;
    private decimal _minAmount;
    private decimal _maxAmount;

    private bool _simplifiedView;

    public FinancialAccounts Accounts
    {
        set { this._accounts = value; }
    }

    public DateTime DateStart
    {
        set { this._dateStart = value; }
    }

    public DateTime DateEnd
    {
        set { this._dateEnd = value; }
    }

    public decimal MaxAmount
    {
        set { this._maxAmount = value; }
    }

    public decimal MinAmount
    {
        set { this._minAmount = value; }
    }

    public bool SimplifiedView
    {
        set { this._simplifiedView = value; }
    }

    public void Populate()
    {
        FinancialAccountRows rows = _accounts.GetRows(this._dateStart, this._dateEnd.AddDays(1));
        Dictionary<int, decimal> accountBalance = new Dictionary<int, decimal>();

        foreach (FinancialAccount account in _accounts)
        {
            _accountNames[account.Identity] = account.Name;

            if (account.AccountType == FinancialAccountType.Asset || account.AccountType == FinancialAccountType.Debt)
            {
                _accountInboundBalance[account.Identity] = account.GetDelta(new DateTime(2006, 1, 1),
                                                                           this._dateStart);
            }
            else
            {
                _accountInboundBalance[account.Identity] = account.GetDelta(new DateTime(this._dateStart.Year, 1, 1),
                                                                           this._dateStart);
            }

            accountBalance[account.Identity] = _accountInboundBalance[account.Identity];
        }

        LedgerRows ledgerRows = new LedgerRows();

        foreach (FinancialAccountRow row in rows)
        {
            accountBalance[row.FinancialAccountId] += row.Amount;

            if (row.Amount <= this._maxAmount)
            {
                ledgerRows.Add(new LedgerRow(row, accountBalance[row.FinancialAccountId]));
            }
        }

        this.GridTransactions.DataSource = ledgerRows;
        Session[_sessionObjectName] = ledgerRows;
        Session["FinancialAccountNames"] = _accountNames;
        this.GridTransactions.DataBind();

        this.GridTransactions.Columns[0].Visible = !_simplifiedView;
        this.GridTransactions.Columns[1].Visible = _simplifiedView;
        this.GridTransactions.Columns[2].Visible = !_simplifiedView;
        this.GridTransactions.Columns[4].Visible = !_simplifiedView;
        this.GridTransactions.Columns[8].Visible = !_simplifiedView;
        this.GridTransactions.Columns[9].Visible = !_simplifiedView;
        this.GridTransactions.Columns[10].Visible = !_simplifiedView;

    }



    protected void GridTransactions_NeedDataSource (object sender, GridNeedDataSourceEventArgs e)
    {
        LedgerRows rows = (LedgerRows) Session[_sessionObjectName];

        this.GridTransactions.DataSource = rows;
    }


    protected void GridTransactions_ItemCreated(object sender, GridItemEventArgs e)
    {
        const string imageUrlNone = "~/Images/Public/Fugue/icons-shadowless/minus-small.png";
        const string imageUrlDoc = "~/Images/Public/Fugue/icons-shadowless/report.png";
        const string imageUrlDoc2 = "~/Images/Public/Fugue/icons-shadowless/reports.png";
        const string imageUrlDoc3 = "~/Images/Public/Fugue/icons-shadowless/reports-stack.png";
        const string imageUrlLink = "~/Images/Public/Fugue/icons-shadowless/chain.png";


        if (e.Item is GridDataItem)
        {
            LedgerRow row = (LedgerRow)e.Item.DataItem;

            if (row == null)
            {
                return;
            }

            decimal delta = row.Amount;
            decimal balance = row.Balance;

            if (_simplifiedView)
            {
                delta = -delta;
                balance = -balance;  // Change accountant's worldview (costs are positive) to normal people's (expenses are negative)
            }


            string field = "LabelDebit";

            if (row.Amount < 0.0m)
            {
                field = "LabelCredit";
            }

            Label labelDelta = (Label)e.Item.FindControl(field);
            labelDelta.Text = delta.ToString("+#,##0.00;-#,##0.00", new CultureInfo("sv-SE"));

            Label labelAccountName = (Label) e.Item.FindControl("LabelAccountName");
            labelAccountName.Text = _accountNames[row.FinancialAccountId];

            Label labelLedgered = (Label) e.Item.FindControl("LabelLedgered");

            if (!_peopleInitials.ContainsKey(row.RowCreatedByPersonId))
            {
                if (row.RowCreatedByPersonId > 0)
                {
                    _peopleInitials[row.RowCreatedByPersonId] = Person.FromIdentity(row.RowCreatedByPersonId).Initials;
                }
                else
                {
                    _peopleInitials[0] = "bot";
                }
            }

            labelLedgered.Text = String.Format("{0:yyMMdd}/{1}", row.RowDateTime,
                                               _peopleInitials[row.RowCreatedByPersonId]);

            
            Label labelBalance = (Label)e.Item.FindControl("LabelBalance");
            labelBalance.Text = balance.ToString("N2", new CultureInfo("sv-SE"));

            Image imageDoxored = (Image) e.Item.FindControl("ImageDocumented");
            Image imageLinked = (Image) e.Item.FindControl("ImageLinked");

            FinancialTransaction transaction = FinancialTransaction.FromIdentity(row.FinancialTransactionId);
            int docCount = Documents.ForObject(transaction).Count;
            bool hasDependency = transaction.Dependency == null ? false : true;

            if (hasDependency)
            {
                imageLinked.ImageUrl = imageUrlLink;
                imageDoxored.ImageUrl = imageUrlDoc;
            }
            else
            {
                imageLinked.ImageUrl = imageUrlNone;

                if (docCount == 0)
                {
                    imageDoxored.ImageUrl = imageUrlNone;
                }
                else if (docCount == 1)
                {
                    imageDoxored.ImageUrl = imageUrlDoc;
                }
                else if (docCount == 2)
                {
                    imageDoxored.ImageUrl = imageUrlDoc2;
                }
                else
                {
                    imageDoxored.ImageUrl = imageUrlDoc3;
                }
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkManage");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowTransactionForm('{0}','{1}');",
                                                           row.FinancialTransactionId, e.Item.ItemIndex);

        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        // no action
    }


    public class LedgerRow
    {
        public LedgerRow (FinancialAccountRow row, decimal balance)
        {
            this.row = row;
            this.balance = balance;
        }

        private FinancialAccountRow row;
        private decimal balance;

        public decimal Balance { get { return balance; } }
        public int FinancialAccountId { get { return row.FinancialAccountId; } }
        public decimal Amount { get { return row.Amount; } }
        public string Description { get { return row.Description; } }
        public DateTime TransactionDateTime { get { return row.TransactionDateTime; } }
        public DateTime RowDateTime { get { return row.RowDateTime; } }
        public int RowCreatedByPersonId { get { return row.RowCreatedByPersonId; } }
        public int FinancialTransactionId { get { return row.FinancialTransactionId; } }
    }

    public class LedgerRows: List<LedgerRow>
    {
        // empty, just want the class name
    }
}
