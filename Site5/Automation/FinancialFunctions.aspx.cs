using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using NBitcoin;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class FinancialFunctions : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            // do nothing
        }

        [WebMethod]
        public static AjaxCallResult SetBitcoinPayoutAddress(string bitcoinAddress)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (authData == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Remove whitespace from submitted address (whitespace will probably be entered in some cases)

            bitcoinAddress = bitcoinAddress.Replace (" ", string.Empty);

            if (string.IsNullOrEmpty (authData.CurrentUser.BitcoinPayoutAddress))
            {
                if (!BitcoinUtility.IsValidBitcoinAddress (bitcoinAddress))
                {
                    return new AjaxCallResult {Success = false, DisplayMessage = "Invalid address"};
                }

                authData.CurrentUser.BitcoinPayoutAddress = bitcoinAddress;
                authData.CurrentUser.BitcoinPayoutAddressTimeSet = DateTime.UtcNow.ToString (CultureInfo.InvariantCulture);

                // TODO: Create notifications for CEO and for user

                NotificationCustomStrings strings = new NotificationCustomStrings();
                strings["BitcoinAddress"] = bitcoinAddress;

                OutboundComm userNotify = OutboundComm.CreateNotification (authData.CurrentOrganization,
                    "BitcoinPayoutAddress_Set", strings, People.FromSingle (authData.CurrentUser));

                strings["ConcernedPersonName"] = authData.CurrentUser.Canonical;

                OutboundComm adminNotify = OutboundComm.CreateNotification (authData.CurrentOrganization,
                    "BitcoinPayoutAddress_Set_OfficerNotify", strings); // will send to admins of org as no people specified

                return new AjaxCallResult {Success = true};
            }
            else
            {
                // If the address is already set

                return new AjaxCallResult {Success = false, DisplayMessage = "Address already set"};
            }
        }
    }
}