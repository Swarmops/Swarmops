using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Communications;
using Activizr.Interface.Localization;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Interface.Objects;
using Activizr.Logic.Security;
using Activizr.Logic.Support;

public partial class Pages_Public_SE_SubscriptionSettings : System.Web.UI.Page
{
    Person _displayedPerson = null;

    protected void Page_Load (object sender, EventArgs e)
    {

        // Get requested person
        try
        {
            string inputKey = HttpContext.Current.Request["Person"].ToString();
            string persId = inputKey.Substring(1, inputKey.IndexOf("A") - 2);
            _displayedPerson = (Person)Person.FromIdentity(Convert.ToInt32(persId));
            string key = "1" + CheckDigit.AppendCheckDigit(_displayedPerson.Identity).ToString() + "A" + _displayedPerson.HexIdentifier();
            if (inputKey.ToUpper()!= key.ToUpper())
                throw new Exception("Wrong Call");

            Subscriptions1.CurrentUserId = _displayedPerson.Identity;
            Subscriptions1.DisplayedPerson = _displayedPerson;
            Panel2.Visible = true;
            Panel1.Visible = false;
        }
        catch
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
        }
    }

}
