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

            Response.ContentType = "application/json";

            Int64 turnoverCentsTotal = 0;
            Int64 inboundCentsTotal = 0;
            Int64 outboundCentsTotal = 0;

            StringBuilder response = new StringBuilder(16384);

            VatReportItems items = report.Items;
            List<string> lines = new List<string>();

            foreach (VatReportItem item in items)
            {
                FinancialTransaction transaction = item.Transaction;

                string element = string.Format("\"id\":\"{0}\",\"txid\":\"{1}\",\"datetime\":\"{2:MMM dd}\",\"description\":\"{3}\"", 
                    transaction.Identity,
                    transaction.OrganizationSequenceId,
                    transaction.DateTime,
                    JsonSanitize(transaction.Description));

                if (item.TurnoverCents > 0)
                {
                    element += String.Format(",\"turnover\":\"{0:N2}\"", item.TurnoverCents / 100.0);
                    turnoverCentsTotal += item.TurnoverCents;
                }

                if (item.VatInboundCents > 0)
                {
                    element += String.Format(",\"inbound\":\"{0:N2}\"", item.VatInboundCents/ 100.0);
                    inboundCentsTotal += item.VatInboundCents;
                }

                if (item.VatOutboundCents > 0)
                {
                    element += String.Format(",\"outbound\":\"{0:N2}\"", item.VatOutboundCents/ 100.0);
                    outboundCentsTotal += item.VatOutboundCents;
                }

                lines.Add("{" + element + "}");
            }

            Response.Write("[" + String.Join(",", lines.ToArray()) + "]");

            // TODO: FOOTER

            Response.End();
        }


    }
}