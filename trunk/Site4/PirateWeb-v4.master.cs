using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Telerik.Web.UI;
using System.Collections.Generic;

public partial class pirateweb_v4 : MasterV4Base
{
    private Person viewingPerson = null;
    protected void Page_Init (object sender, EventArgs e)
    {
        form1.DefaultButton = null;
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        this.LanguageSelector1.LanguageChanged += new EventHandler(LanguageSelector_LanguageChanged);

        viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = viewingPerson.GetAuthority();

        RadMenu MainMenu = FindControl("MainMenu") as RadMenu;

        Dictionary<string, bool> enableCache = new Dictionary<string, bool>();

        if (Session["MainMenu-v4_Enabling"] != null
            && PermissionCacheTimestamp.AddSeconds(10) > DateTime.Now
            && Authorization.lastReload < PermissionCacheTimestamp)
        {
            enableCache = Session["MainMenu-v4_Enabling"] as Dictionary<string, bool>;
        }
        else
            PermissionCacheTimestamp = DateTime.Now;


        Authorization.flagReload = false;
        RadMenuItemCollection menuItems = MainMenu.Items;
        SetupMenuItemsEnabling(authority, enableCache, menuItems);
        Session["MainMenu-v4_Enabling"] = enableCache;

        if (this.Page is PageV4Base)
        {
            bool CurrentPageAccess = false;
            if (((PageV4Base)this.Page).pagePermissionDefault != null)
                CurrentPageAccess = Authorization.CheckAuthorization(((PageV4Base)this.Page).pagePermissionDefault, -1, -1, authority, Authorization.Flag.AnyGeographyAnyOrganization);

            if ((CurrentPageAccess == false && CurrentPageAllowed == false) || CurrentPageProhibited)
            {
                this.AccessDeniedPanel.Visible = true;
                if (CurrentPageAccess == false)
                {
                    this.LabelFailedPermission.Text = (((PageV4Base)this.Page).pagePermissionDefault).ToString();
                }
                else if (CurrentPageProhibited)
                {
                    this.LabelFailedPermission.Text = "CurrentPageProhibited";
                }

                this.BodyContent.Visible = false;
            }
            else
            {
                this.AccessDeniedPanel.Visible = false;
                this.BodyContent.Visible = true;
            }
        }
    }

    public event EventHandler LanguageChanged;

    private void LanguageSelector_LanguageChanged (object sender, EventArgs e)
    {
        // Received event from control - refire

        viewingPerson.PreferredCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;


        if (LanguageChanged != null)
        {
            LanguageChanged(this, new EventArgs());
        }

    }


    private bool SetupMenuItemsEnabling (Authority authority, Dictionary<string, bool> enableCache, RadMenuItemCollection menuItems)
    {
        //TODO: Need to handle setup when user is not swedish, current handling supposes that top org is Organization.PPSEid
        string thisPageUrl = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLower();
        bool anyItem = false;
        foreach (RadMenuItem item in menuItems)
        {
            string itemID = CollectItemID(item);
            if (enableCache.ContainsKey(itemID))
            {
                item.Enabled = enableCache[itemID];
            }
            else
            {
                item.Enabled = true;
                if (string.IsNullOrEmpty(item.Attributes["Permission"]) == false)
                {
                    string permString = item.Attributes["Permission"].ToString();
                    PermissionSet allowedFor = new PermissionSet(permString);
                    item.Enabled = Authorization.CheckAuthorization(allowedFor, -1, -1, authority, Authorization.Flag.AnyGeographyAnyOrganization);
                }
                else
                {
                    RadMenuItem permissionParent = item;
                    while (permissionParent.Parent is RadMenuItem)
                    {
                        if (string.IsNullOrEmpty(permissionParent.Attributes["Permission"]) == false)
                        {
                            item.Enabled = permissionParent.Enabled;
                            item.Attributes["Permission"] = permissionParent.Attributes["Permission"].ToString();
                            break;
                        }
                        permissionParent = permissionParent.Parent as RadMenuItem;
                    }
                }
                if (string.IsNullOrEmpty(item.NavigateUrl) && item.Items.Count == 0)
                    item.Enabled = false;
                enableCache[itemID] = item.Enabled;
            }
            string[] currentItemUrlSplit = item.NavigateUrl.ToLower().Split(new char[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries);
            if (Array.Exists<string>(currentItemUrlSplit,
                            delegate(String s) { if (s == thisPageUrl) return true; else return false; })
                )
            {
                if (string.IsNullOrEmpty(item.Attributes["Permission"]) == false)
                {
                    string permString = item.Attributes["Permission"].ToString();
                    ((PageV4Base)(this.Page)).pagePermissionDefault = new PermissionSet(permString);
                }
            }
            if (item.Items.Count > 0)
            {
                bool enabledSubItems = SetupMenuItemsEnabling(authority, enableCache, item.Items);
                if (enabledSubItems)
                    item.Enabled = true;
                else
                    item.Enabled = false;
            }
            else if (string.IsNullOrEmpty(item.NavigateUrl))
            {
                item.Enabled = false;
            }

            anyItem |= item.Enabled;
        }
        return anyItem;
    }

    string CollectItemID (IRadMenuItemContainer item)
    {
        if (item.Owner == null)
            return "";
        else
            return CollectItemID(item.Owner) + "_" + ((RadMenuItem)item).ID;

    }

    protected void ScriptManager1_AsyncPostBackError (object sender, AsyncPostBackErrorEventArgs e)
    {
        if (this.Page is PageV4Base)
        {
            ((PageV4Base)this.Page).AsyncPostBackError(ScriptManager1, sender, e);
        }
        else
        {
            ScriptManager1.AsyncPostBackErrorMessage = "There was an error while doing a partial page update:\r\n" + e.Exception.ToString();
        }

    }
}