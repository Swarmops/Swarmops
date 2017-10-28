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

            Int64 turnoverTotal;
            Int64 inboundTotal;
            Int64 outboundTotal;

            StringBuilder response = new StringBuilder(16384);

            VatReportItems items = report.Items;
            List<string> lines = new List<string>();

            foreach (VatReportItem item in items)
            {
                FinancialTransaction transaction = item.Transaction;

                string element = string.Format("\"id\":\"{0}\",\"txid\":\"{1}\",\"datetime\":\"{2}\",\"description\":\"{3}\"", 
                    transaction.Identity,
                    transaction.OrganizationSequenceId,
                    "date time",
                    JsonSanitize(transaction.Description));


                lines.Add("{" + element + "}");
            }

            Response.Write("[" + String.Join(",", lines.ToArray()) + "]");

            Response.End();
        }


    }
}