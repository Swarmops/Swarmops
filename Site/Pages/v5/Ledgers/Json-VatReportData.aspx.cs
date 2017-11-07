using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_VatReportData : DataV5Base
    {
        private AuthenticationData _authenticationData;

        private int _reportId = 0;

        protected void Page_Load (object sender, EventArgs e)
        {
            // Get auth data

            this._authenticationData = GetAuthenticationDataAndCulture();

            string reportIdParameter = Request.QueryString["ReportId"];
            string reportKeyParameter = Request.QueryString["ReportKey"];

            if (!string.IsNullOrEmpty (reportIdParameter))
            {
                this._reportId = Int32.Parse (reportIdParameter); // will throw if non-numeric - don't matter for app
            }
            else if (!string.IsNullOrEmpty(reportKeyParameter))
            {
                this._reportId =
                    SwarmDb.GetDatabaseForReading().GetVatReportIdFromGuid(reportKeyParameter.ToLowerInvariant().Trim());
            }
            else
            {
                throw new ArgumentException("No Report Requested");
            }

            VatReport report = VatReport.FromIdentity(_reportId);

            // TODO: AUTH CHECK

            Response.ContentType = "application/json";

            Int64 turnoverCentsTotal = 0;
            Int64 inboundCentsTotal = 0;
            Int64 outboundCentsTotal = 0;

            StringBuilder response = new StringBuilder(16384);

            VatReportItems items = report.Items;
            List<string> lines = new List<string>();

            string hasDoxString =
                "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\\\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\\\" onmouseout=\\\"this.src='/Images/Icons/iconshock-search-256px.png';\\\" baseid='{0}' class='LocalViewDox' style='cursor:pointer' height='20' width='20' />";

            foreach (VatReportItem item in items)
            {
                FinancialTransaction transaction = item.Transaction;
                bool include = false;

                string element = string.Format("\"id\":\"{0}\",\"txid\":\"{1}\",\"datetime\":\"{2:MMM dd}\",\"description\":\"{3}\"", 
                    transaction.Identity,
                    transaction.OrganizationSequenceId,
                    transaction.DateTime,
                    JsonSanitize(transaction.Description));

                if (item.TurnoverCents > 0)
                {
                    element += String.Format(",\"turnover\":\"{0:N2}\"", item.TurnoverCents / 100.0);
                    turnoverCentsTotal += item.TurnoverCents;
                    include = true;
                }

                if (item.VatInboundCents > 0)
                {
                    element += String.Format(",\"inbound\":\"{0:N2}\"", item.VatInboundCents/ 100.0);
                    inboundCentsTotal += item.VatInboundCents;
                    include = true;
                }

                if (item.VatOutboundCents > 0)
                {
                    element += String.Format(",\"outbound\":\"{0:N2}\"", item.VatOutboundCents/ 100.0);
                    outboundCentsTotal += item.VatOutboundCents;
                    include = true;
                }

                if (transaction.Dependency != null)
                {

                    element += String.Format(",\"dox\":\"" + hasDoxString + "\"", transaction.Identity);
                }

                if (include)
                {
                    lines.Add("{" + element + "}");
                }
            }

            if (lines.Count == 0) // add empty report line 
            {
                lines.Add("{" + String.Format("\"id\":\"0\",\"description\":\"{0}\"",
                    JsonSanitize(Resources.Pages.Ledgers.ViewVatReports_EmptyReport)) + "}");
            }

            Response.Write("{\"rows\":[" + String.Join(",", lines.ToArray()) + "]");

            Response.Write(",\"footer\":[{"); // needs to be on separate line to not trigger a String.Format warning

            Response.Write(String.Format("\"id\":\"0\",\"description\":\"{0}\",\"turnover\":\"{1:N2}\",\"inbound\":\"{2:N2}\",\"outbound\":\"{3:N2}\"",
                JsonSanitize(Resources.Pages.Ledgers.ViewVatReports_Footer_Total.ToUpperInvariant()), turnoverCentsTotal / 100.0, inboundCentsTotal / 100.0, outboundCentsTotal / 100.0));

            Response.Write("}]}"); // needs to be separate to not trigger String.Format warning

            Response.End();
        }


    }
}