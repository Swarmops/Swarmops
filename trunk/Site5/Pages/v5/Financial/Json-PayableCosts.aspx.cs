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
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.Financial
{
    public partial class JsonPayableCosts : System.Web.UI.Page
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

            // Assert economic access



            // TODO: Set language for localization

            // Get all payable items

            Payouts payouts = Payouts.Construct(_currentOrganization);

            // Format as JSON and return

            string json = FormatAsJson(payouts);
            Response.Output.WriteLine(json);
            Response.End();
        }

        private string FormatAsJson(Payouts payouts)
        {
            StringBuilder result = new StringBuilder(16384);

            string hasDoxString =
                "<img src=\\\"/Images/Icons/iconshock-glass-16px.png\\\" onmouseover=\\\"this.src='/Images/Icons/iconshock-glass-16px-hot.png';\\\" onmouseout=\\\"this.src='/Images/Icons/iconshock-glass-16px.png';\\\" onclick=\\\"alert('foo');\\\" style=\\\"cursor:pointer\\\" />";

            result.Append("{\"rows\":[");

            DateTime today = DateTime.Today;

            foreach (Payout payout in payouts)
            {
                result.Append("{");
                result.AppendFormat(
                    "\"itemId\":\"{0}\"," +
                    "\"due\":\"{1}\"," +
                    "\"recipient\":\"{2}\"," +
                    "\"bank\":\"{3}\"," +
                    "\"account\":\"{4}\"," +
                    "\"reference\":\"{5}\"," +
                    "\"amount\":\"{6:N2}\"," +
                    "\"action\":\"<span style=\\\"position:relative;top:3px\\\">" +
                    "<img id=\\\"IconApproval{0}\\\" class=\\\"LocalIconApproval\\\" baseid=\\\"{0}\\\" height=\\\"16\\\" width=\\\"16\\\" />" +
                    "<img id=\\\"IconApproved{0}\\\" class=\\\"LocalIconApproved\\\" baseid=\\\"{0}\\\" height=\\\"16\\\" width=\\\"16\\\" />\"",
                    payout.ProtoIdentity,
                    (payout.ExpectedTransactionDate <= today? Resources.Global.Global_ASAP: payout.ExpectedTransactionDate.ToShortDateString()),
                    TryLocalize(payout.Recipient),
                    payout.Bank,
                    payout.Account,
                    TryLocalize(payout.Reference),
                    payout.AmountCents/100.0);
                result.Append("},");
            }

            result.Remove(result.Length - 1, 1); // remove last comma

            result.Append("]}");

            return result.ToString();
        }

        private string TryLocalize(string input)
        {
            if (!input.StartsWith("[Loc]"))
            {
                return input;
            }

            string[] inputParts = input.Split('|');

            string resourceKey = inputParts[0].Substring(5);
            string translatedResource = GetGlobalResourceObject("Global", resourceKey).ToString();

            if (inputParts.Length == 1)
            {
                return translatedResource;
            }
            else
            {
                object argument = null;

                if (inputParts[1].StartsWith("[Date]"))
                {
                    argument = DateTime.Parse(inputParts[1].Substring(6), CultureInfo.InvariantCulture);
                }
                else
                {
                    argument = inputParts[1];
                }

                return String.Format(translatedResource, argument);
            }
        }

        private Dictionary<int, bool> _attestationRights;
        private Person _currentUser;
        private Organization _currentOrganization;

    }

}