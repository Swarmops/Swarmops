using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Interfaces;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

public partial class Pages_v5_Finance_Json_AttestableCosts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Current authentication

        string identity = HttpContext.Current.User.Identity.Name;
        string[] identityTokens = identity.Split(',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        int currentUserId = Convert.ToInt32(userIdentityString);
        int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

        _currentUser = Person.FromIdentity(currentUserId);
        Authority authority = _currentUser.GetAuthority();
        _currentOrganization = Organization.FromIdentity(currentOrganizationId);

        Response.ContentType = "application/json";

        _attestationRights = GetAttestationRights();

        // TODO: Set language for localization

        // Get all attestable items

        _items = new AttestableItems();

        PopulateCashAdvances();

        // Format as JSON and return

        string json = FormatAsJson();
        Response.Output.WriteLine(json);
        Response.End();
    }

    private string FormatAsJson()
    {
        StringBuilder result = new StringBuilder(16384);

        result.Append("{\"rows\":[");

        foreach (AttestableItem item in _items)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"beneficiary\":\"{1}\",\"description\":\"{2}\",\"budgetName\":\"{3}\",\"amountRequested\":\"{4:N2}\",\"itemId\":\"{5}\"",
                item.IdentityDisplay, item.Beneficiary, item.Description, item.BudgetName,
                item.AmountRequestedCents/100.0, item.Identity);
            result.Append("},");
        }

        result.Remove(result.Length - 1, 1); // remove last comma

        result.Append("]}");

        return result.ToString();
    }

    private Dictionary<int, bool> _attestationRights;
    private Person _currentUser;
    private Organization _currentOrganization;
    private AttestableItems _items;

    protected class AttestableItem
    {
        public AttestableItem(string identity, string beneficiary, Int64 amountCents, FinancialAccount account, string description, string identityDisplay, IHasIdentity item)
        {
            this.IdentityDisplay = identityDisplay;
            this.Identity = identity;
            this.Beneficiary = beneficiary;
            this.AmountRequestedCents = amountCents;
            this.Budget = account;
            this.Description = description;
            this.Item = item;
        }

        public FinancialAccount Budget { get; private set; }
        public string BudgetName { get { return this.Budget.Name; } }
        public string Identity { get; private set; }
        public string Beneficiary { get; private set; }
        public Int64 AmountRequestedCents { get; private set; }
        public string Description { get; private set; }
        public string IdentityDisplay { get; private set; }
        public IHasIdentity Item { get; private set; }
    }

    protected class AttestableItems : List<AttestableItem>
    {
        // empty class, we just want the name definition        
    }



    private Dictionary<int, bool> GetAttestationRights()
    {
        // Right now, this function is quite primitive. At some point in the future, it needs to take into
        // account that a budget may have several attesters. Right now, it just loops over all accounts and
        // checks the owner.

        Dictionary<int, bool> result = new Dictionary<int, bool>();
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(_currentOrganization);

        foreach (FinancialAccount account in accounts)
        {
            if (account.OwnerPersonId == _currentUser.Identity)
            {
                result[account.Identity] = true;
            }
        }

        return result;
    }


    private void PopulateCashAdvances()
    {
        CashAdvances advances = CashAdvances.ForOrganization(_currentOrganization).WhereUnattested;

        foreach (CashAdvance advance in advances)
        {
            _items.Add(new AttestableItem("A" + advance.Identity.ToString(CultureInfo.InvariantCulture), advance.Person.Name, advance.AmountCents, advance.FinancialAccount, advance.Description, "AttestCosts_Advance", advance));
        }
    }


}