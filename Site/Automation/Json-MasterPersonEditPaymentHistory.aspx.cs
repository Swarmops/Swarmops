using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_MasterPersonEditPaymentHistory : DataV5Base
    {
        private AuthenticationData _authenticationData;
        private Person _person;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._authenticationData = GetAuthenticationDataAndCulture();

            int personId = Int32.Parse(Request.QueryString["PersonId"]); // may throw and that's okay, returning a 500 instead of Json the caller shouldn't see
            if (personId == 0)
            {
                personId = _authenticationData.CurrentUser.Identity;
            }

            this._person = Person.FromIdentity(personId);

            List<PaymentHistoryLineItem> list = new List<PaymentHistoryLineItem>();

            list.AddRange(GetAmountsOwed());
            list.AddRange(GetAmountsPaid());

            Response.ContentType = "application/json";
            Response.Output.WriteLine("{\"rows\": " + JsonWriteItems(list) + ", \"footer\": [" +
                                       JsonWriteFooter(list) + "]}");

            Response.End();

        }


        public List<PaymentHistoryLineItem> GetAmountsOwed()
        {
            List<PaymentHistoryLineItem> items = new List<PaymentHistoryLineItem>();

            // Expense claims

            ExpenseClaims expenses = ExpenseClaims.FromClaimingPersonAndOrganization(_person,
                _authenticationData.CurrentOrganization);

            foreach (ExpenseClaim claim in expenses)
            {
                PaymentHistoryLineItem newItem = new PaymentHistoryLineItem();
                newItem.Id = "E" + claim.Identity.ToString(CultureInfo.InvariantCulture);
                newItem.Name = Resources.Global.Financial_ExpenseClaim + " #" + claim.Identity.ToString("N0");
                newItem.Description = claim.Description;
                newItem.OwedToPerson = claim.AmountCents;

                items.Add(newItem);
            }

            return items;
        }

        public List<PaymentHistoryLineItem> GetAmountsPaid()
        {
            return null;
        }

        public string JsonWriteItems(List<PaymentHistoryLineItem> items)
        {
            StringBuilder result = new StringBuilder(16384);
            result.Append("[");

            foreach (PaymentHistoryLineItem item in items)
            { 
                result.Append("{");
                result.AppendFormat(
                    "\"id\":\"{0}\",\"name\":\"{1}\",\"description\":\"{2}\",\"owedToPerson\":\"{3:N2}\"",
                    JsonSanitize(item.Id),
                    JsonSanitize(item.Name),
                    JsonSanitize(item.Description),
                    item.OwedToPerson/100.0
                );

                result.Append("},");
            }

            result.Remove(result.Length - 1, 1); // remove last comma
            result.Append("]");

            return result.ToString();
        }

public string JsonWriteFooter(List<PaymentHistoryLineItem> items)
        {
            return string.Empty;
        }

        public class PaymentHistoryLineItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public Int64 OwedToPerson { get; set; }
            public Int64 PaidToPerson { get; set; }

            public DateTime OpenedDate { get; set; }
            public DateTime ClosedDate { get; set; }
        }

    }
}