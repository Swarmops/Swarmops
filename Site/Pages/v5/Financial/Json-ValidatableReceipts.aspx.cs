using System;
using System.Text;
using Swarmops.Frontend;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

public partial class Pages_v5_Finance_Json_ValidatableReceipts : DataV5Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.Financials, AccessType.Read)))
        {
            throw new UnauthorizedAccessException();
        }

        ExpenseClaims claims = ExpenseClaims.ForOrganization (CurrentOrganization);
        claims = claims.WhereUnvalidated;

        // Format as JSON and return

        Response.ContentType = "application/json";
        string json = FormatAsJson (claims);
        Response.Output.WriteLine (json);
        Response.End();
    }

    private string FormatAsJson (ExpenseClaims claims)
    {
        StringBuilder result = new StringBuilder (16384);

        string hasDoxString =
            "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\\\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\\\" onmouseout=\\\"this.src='/Images/Icons/iconshock-search-256px.png';\\\" baseid=\\\"E{5}\\\" class=\\\"LocalViewDox\\\" height=\\\"20\\\" width=\\\"20\\\" style=\\\"cursor:pointer\\\" />";

        result.Append ("{\"rows\":[");
        FinancialTransactionTagSets tagSets = FinancialTransactionTagSets.ForOrganization (CurrentOrganization);

        foreach (ExpenseClaim claim in claims)
        {
            StringBuilder extraTags = new StringBuilder();

            FinancialTransaction transaction = claim.FinancialTransaction;

            if (transaction != null)
            {
                foreach (FinancialTransactionTagSet tagSet in tagSets)
                {
                    FinancialTransactionTagType tagType = transaction.GetTag (tagSet);

                    extraTags.AppendFormat ("\"tagSet{0}\":\"{1}\",",
                        tagSet.Identity, tagType != null ? JsonSanitize (tagType.Name) : string.Empty);
                }
            }

            result.Append ("{");
            result.AppendFormat (
                "\"description\":\"{2}\",\"budgetName\":\"{3}\",{6}\"amountRequested\":\"{4:N2}\",\"itemId\":\"E{5}\"," +
                "\"dox\":\"" + (claim.Documents.Count > 0 ? hasDoxString : "&nbsp;") + "\"," +
                "\"actions\":\"" +
                "<img id=\\\"IconApprovalE{5}\\\" class=\\\"LocalIconApproval\\\" baseid=\\\"E{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconApprovedE{5}\\\" class=\\\"LocalIconApproved\\\" baseid=\\\"E{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconDenialE{5}\\\" class=\\\"LocalIconDenial\\\" baseid=\\\"E{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "<img id=\\\"IconDeniedE{5}\\\" class=\\\"LocalIconDenied\\\" baseid=\\\"E{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />\"",
                "olditem", JsonSanitize (claim.ClaimerCanonical), JsonSanitize (claim.Description),
                JsonSanitize (claim.Budget.Name),
                claim.AmountCents/100.0, claim.Identity, extraTags);
            result.Append ("},");
        }

        result.Remove (result.Length - 1, 1); // remove last comma

        result.Append ("]}");

        return result.ToString();
    }
}