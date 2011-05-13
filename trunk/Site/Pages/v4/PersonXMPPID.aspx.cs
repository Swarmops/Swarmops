using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Pirates;
using Activizr.Basic.Enums;
using System.Text.RegularExpressions;
using Activizr.Logic.Security;
using Activizr.Logic.Support;

public partial class Pages_v4_PersonXMPPID : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        LabelError.Text = "";
        LabelChars.Visible = false;
        LabelBadPw.Visible = false;
        LabelMatch.Visible = false;
        LabelOccupied.Visible = false;
        if (!IsPostBack)
        {
            ExternalIdentity extID = null;
            try
            {
                extID = ExternalIdentity.FromPersonIdAndType(_currentUser.Identity, ExternalIdentityType.PPXMPPAccount);
                TextBoxJabberID.Text = extID.UserID.Split('@')[0];
            }
            catch { }

        }

    }
    protected void ButtonOK_Click (object sender, EventArgs e)
    {
        TextBoxJabberID.Text = TextBoxJabberID.Text.Trim();
        string jid = "" + TextBoxJabberID.Text + "@piratechat.net";
        Regex reValidate = new Regex(@"^([^""&/:<>@\\]|\w|[!#$%'\)\(\*\+,-.\}\{])+$", RegexOptions.Singleline);
        if (TextBoxJabberID.Text.Length < 1
            || !reValidate.IsMatch(TextBoxJabberID.Text.Trim()))
        {
            LabelChars.Visible = true;
            return;
        }

        ExternalIdentity extID = null;
        try
        {
            extID = ExternalIdentity.FromUserIdAndType(jid, ExternalIdentityType.PPXMPPAccount);
        }
        catch { }

        if (extID != null)
        {
            if (extID.AttachedToPersonID != _currentUser.Identity)
            {
                LabelOccupied.Visible = true;
                return;
            }
        }

        Password1.Text = Password1.Text.Trim();
        Password2.Text = Password2.Text.Trim();

        if (Password1.Text != Password2.Text)
        {
            LabelMatch.Visible = true;
            return;
        }

        if (_currentUser.ValidatePassword(Password1.Text))
        {
            LabelBadPw.Visible = true;
            return;
        }
        try
        {
            extID = ExternalIdentity.FromPersonIdAndType(_currentUser.Identity, ExternalIdentityType.PPXMPPAccount);
        }
        catch { }

        if (extID == null)
        {
            extID = ExternalIdentity.CreateExternalIdentity("xmpp.piratpartiet.se", jid, Password1.Text, _currentUser.Identity, ExternalIdentityType.PPXMPPAccount);
            LabelError.Text = " Identity created.";
            PWLog.Write(_currentUser,PWLogItem.ExtAccount,extID.Identity,PWLogAction.ExtAccountChanged,"Created XMPPAccount","","","",jid);
        }
        else
        {
            PWLog.Write(_currentUser, PWLogItem.ExtAccount, extID.Identity, PWLogAction.ExtAccountChanged, "Updated XMPPAccount", "", "UserID", extID.UserID, jid);
            extID.SetExternalIdentity("xmpp.piratpartiet.se", jid, Password1.Text, _currentUser.Identity, ExternalIdentityType.PPXMPPAccount);
            LabelError.Text = " Identity updated.";
        }
    }
}
