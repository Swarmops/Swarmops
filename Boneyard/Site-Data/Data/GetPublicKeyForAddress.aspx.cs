using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;

public partial class Data_GetPublicKeyForAddress : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string address = Request.QueryString["Address"];
        Response.ContentType = "text/plain";

        if (string.IsNullOrEmpty(address))
        {
            Response.Write("No Address parameter (e.g. Address=rickard.olsson@piratpartiet.se) was given on the URL.\r\n");
            return;
        }

        address = address.ToLowerInvariant();
        string account = address.Substring(0, address.IndexOf('@')).Replace(".", " ");

        People candidates = People.FromNamePattern(account);

        bool found = false;

        foreach (Person candidate in candidates)
        {
            if (candidate.PartyEmail == address)
            {
                found = true;
                if (candidate.CryptoFingerprint.Length > 4)
                {
                    Response.Write(candidate.CryptoFingerprint + "\r\nis the pubkey fingerprint for " +
                                   HttpUtility.HtmlEncode(candidate.Canonical) + ".\r\n");
                }
                else
                {
                    Response.Write("No key has been pregenerated for " + HttpUtility.HtmlEncode(candidate.Canonical) + ".\r\nA crypto key has been requested. Return in a few minutes to collect its pubkey fingerprint.\r\n");
                    PWEvents.CreateEvent(EventSource.PirateWeb, EventType.CryptoKeyRequested, 0, 0, 0,
                                         candidate.Identity, 0, string.Empty);
                }
            }
        }

        if (!found)
        {
            Response.Write("No person was found with the supplied address.\r\n");
        }
    }
}
