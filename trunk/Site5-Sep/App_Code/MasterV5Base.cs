using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for MasterV4Base
/// </summary>

public class MasterV5Base : System.Web.UI.MasterPage
{

    public bool CurrentPageAllowed = false;
    public bool CurrentPageProhibited = false;

    public string CurrentPageTitle = string.Empty;
    public string CurrentPageIcon = string.Empty;

    public DateTime PermissionCacheTimestamp
    {
        get
        {
            if (Session["MainMenu-v5_Enabling_TimeStamp"] != null)
            {
                return new DateTime((long)Session["MainMenu-v5_Enabling_TimeStamp"]);
            }
            else
                return DateTime.MinValue;

        }
        set
        {
            Session["MainMenu-v5_Enabling_TimeStamp"] = value.Ticks;
        }
    }
}


