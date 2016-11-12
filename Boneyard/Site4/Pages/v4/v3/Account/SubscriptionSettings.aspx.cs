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

public partial class Pages_Account_SubscriptionSettings : PageV4Base
{

    protected void Page_Load (object sender, EventArgs e)
    {

        Subscriptions1.DisplayedPerson = _currentUser;
    }



}
