using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_v5_Ledgers_Json_ProfitLossData : System.Web.UI.Page
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

        Person currentUser = Person.FromIdentity(currentUserId);
        Authority authority = currentUser.GetAuthority();
        Organization currentOrganization = Organization.FromIdentity(currentOrganizationId);

        // Get accounts

        FinancialAccounts accounts = FinancialAccounts.ForOrganization(currentOrganization, FinancialAccountType.Result);

        // Build tree (there should be a template for this)

        Dictionary<int, List<FinancialAccount>> treeMap = new Dictionary<int, List<FinancialAccount>>();

        foreach (FinancialAccount account in accounts)
        {
            if (!treeMap.ContainsKey(account.ParentIdentity))
            {
                treeMap[account.ParentIdentity] = new List<FinancialAccount>();
            }

            treeMap[account.ParentIdentity].Add(account);
        }


        FinancialAccounts orderedList = new FinancialAccounts(); // This list is guaranteed to have parents before children

        PopulateOrderedList(treeMap, orderedList, 0);  // recursively add nodes parents-first
        PopulateLookups(orderedList);                  // populate the lookup tables for results per account

        Response.ContentType = "application/json";

        int renderRootNodeId = 0;

        if (treeMap[0].Count == 1)
        {
            // assume there's a master root like "Costs"; bypass it

            renderRootNodeId = treeMap[0][0].Identity;
        }

        Response.Output.WriteLine(RecurseTreeMap(treeMap, renderRootNodeId));

        Response.End();
    }


    private string RecurseTreeMap (Dictionary<int, List<FinancialAccount>> treeMap, int renderNodeId)
    {
        List<string> elements = new List<string>();

        foreach (FinancialAccount account in treeMap[renderNodeId])
        {
            string element = string.Format("\"id\":{0},\"name\":\"{1}\"", account.Identity,
                                            account.Name.Replace("\"", "'"));

            if (treeMap.ContainsKey(account.Identity))
            {

                element += string.Format(",\"lastYear\":\"<span class=\\\"profitlossdata-collapsed-{0}\\\"><strong>&Sigma;</strong> {1:N2}</span><span class=\\\"profitlossdata-expanded-{0}\\\" style=\\\"display:none\\\">{2:N2}</span>\"", account.Identity, treeLookups[0][account.Identity] / -100.0, singleLookups[0][account.Identity] / -100.0);

                element += ",\"state\":\"closed\",\"children\":" + RecurseTreeMap(treeMap, account.Identity);
            }
            else
            {
                element += string.Format(",\"lastYear\":\"{0:N2}\"", (double) singleLookups[0][account.Identity] / -100.0);
            }

            elements.Add("{" + element + "}");
        }

        return "[" + String.Join(",", elements.ToArray()) + "]";
    }


    private void PopulateOrderedList (Dictionary<int, List<FinancialAccount>> treeMap, FinancialAccounts orderedList, int renderNodeId)
    {
        foreach (FinancialAccount account in treeMap[renderNodeId])
        {
            orderedList.Add(account);

            if (treeMap.ContainsKey(account.Identity))
            {
                PopulateOrderedList(treeMap, orderedList, account.Identity); // recursive call
            }
        }
    }


    private int _year = 2012;
    private Dictionary<int, Int64>[] singleLookups;
    private Dictionary<int, Int64>[] treeLookups;


    private void PopulateLookups(FinancialAccounts accounts)
    {
        singleLookups = new Dictionary<int, Int64>[6];
        treeLookups = new Dictionary<int, Int64>[6];

        for (int index = 0; index < 6; index++)
        {
            treeLookups[index] = new Dictionary<int, Int64>();
            singleLookups[index] = new Dictionary<int, Int64>();
        }

        DateTime[] quarterBoundaries =
            {
                new DateTime(_year, 1, 1), new DateTime(_year, 3, 1), new DateTime(_year, 6, 1),
                new DateTime(_year, 9, 1), new DateTime(_year + 1, 1, 1)
            };

        // 1) Actually, the accounts are already sorted. Or are supposed to be, anyway,
        // since FinancialAccounts.ForOrganization gets the _tree_ rather than the flat list.

        // 2) Add all values to the accounts.

        foreach (FinancialAccount account in accounts)
        {
            // Find this year's inbound

            singleLookups[0][account.Identity] = account.GetDeltaCents(new DateTime(_year - 1, 1, 1), new DateTime(_year, 1, 1));

            // Find quarter diffs

            for (int quarter = 0; quarter < 4; quarter++)
            {
                singleLookups[quarter + 1][account.Identity] = account.GetDeltaCents(quarterBoundaries[quarter],
                                                                                quarterBoundaries[quarter + 1]);
            }

            // Find outbound

            singleLookups[5][account.Identity] = account.GetDeltaCents(new DateTime(_year, 1, 1), new DateTime(_year + 1, 1, 1));

            // copy to treeLookups

            for (int index = 0; index < 6; index++)
            {
                treeLookups[index][account.Identity] = singleLookups[index][account.Identity];
            }
        }

        // 3) Add all children's values to parents

        for (int index = 0; index < 6; index++)
        {
            AddChildrenValuesToParents(treeLookups[index], accounts);
        }

        // Done.
    }


    private void AddChildrenValuesToParents(Dictionary<int, Int64> lookup, FinancialAccounts accounts)
    {
        // Iterate backwards and add any value to its parent's value, as they are sorted in tree order.

        for (int index = accounts.Count - 1; index >= 0; index--)
        {
            int parentFinancialAccountId = accounts[index].ParentFinancialAccountId;
            int accountId = accounts[index].Identity;

            if (parentFinancialAccountId != 0)
            {
                lookup[parentFinancialAccountId] += lookup[accountId];
            }
        }
    }



    /*
    protected void GridBudgetAccounts_ItemCreated(object sender, GridItemEventArgs e)
    {
        // CreateExpandCollapseButton(e.Item, "Name");

        if (e.Item is GridHeaderItem && e.Item.OwnerTableView != this.GridBudgetAccounts.MasterTableView)
        {
            e.Item.Style["display"] = "none";
        }

        if (e.Item is GridNestedViewItem)
        {
            e.Item.Cells[0].Visible = false;
        }

        if (e.Item is GridDataItem)
        {
            int year = DateTime.Today.Year;

            FinancialAccount account = (FinancialAccount) e.Item.DataItem;

            if (account == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink) e.Item.FindControl("ManageLink");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowAccountForm('{0}','{1}');",
                                                           account.FinancialAccountId, e.Item.ItemIndex);

            PopulateLine(e.Item);
        }
    }

    protected void PopulateLine(GridItem item)
    {
        FinancialAccount account = (FinancialAccount) item.DataItem;

        if (account == null)
        {
            return;
        }

        Label labelAccountName = (Label) item.FindControl("LabelAccountName");
        Literal literalLastYear = (Literal) item.FindControl("LiteralLastYear");
        Literal literalQ1 = (Literal) item.FindControl("LiteralQuarter1");
        Literal literalQ2 = (Literal) item.FindControl("LiteralQuarter2");
        Literal literalQ3 = (Literal) item.FindControl("LiteralQuarter3");
        Literal literalQ4 = (Literal) item.FindControl("LiteralQuarter4");
        Literal literalYtd = (Literal) item.FindControl("LiteralThisYear");

        PopulateDualFigure(literalLastYear, "LastYear", 0, account.Identity);
        PopulateDualFigure(literalQ1, "Q1", 1, account.Identity);
        PopulateDualFigure(literalQ2, "Q2", 2, account.Identity);
        PopulateDualFigure(literalQ3, "Q3", 3, account.Identity);
        PopulateDualFigure(literalQ4, "Q4", 4, account.Identity);
        PopulateDualFigure(literalYtd, "Ytd", 5, account.Identity);
        labelAccountName.Text = account.Name;
    }
    */
    private void PopulateDualFigure(Literal literal, string spanPrefix, int category, int accountId)
    {
        Int64 treeValue = treeLookups[category][accountId];
        Int64 singleValue = singleLookups[category][accountId];

        string span =
            string.Format(
                "<span ID=\"GridSpanExpanded{1}{0}\" style=\"display:none\">{2:N0}</span><span ID=\"GridSpanCollapsed{1}{0}\">{4}{3:N0}</span>",
                accountId, spanPrefix, singleValue, treeValue,
                singleValue != treeValue ? "<b>&Sigma;</b>&nbsp;" : string.Empty);
        literal.Text = span;
    }

    private void PopulateFigureCollapsed(Label label, int category, int accountId)
    {
        string prefix = string.Empty;

        Int64 treeValue = treeLookups[category][accountId];
        Int64 singleValue = singleLookups[category][accountId];

        if (treeValue != singleValue)
        {
            prefix = "Σ  ";
        }

        label.Text = prefix + treeValue.ToString("N0");
    }

}