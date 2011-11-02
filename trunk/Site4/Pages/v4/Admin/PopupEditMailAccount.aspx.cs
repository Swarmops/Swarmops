using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Mail;
using Activizr.Logic.Support;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;

public partial class Pages_v4_Admin_PopupEditMailAccount : System.Web.UI.Page
{
    string state = "";
    string account = "";

    int currentUserId = 0;
    Person _currentUser = null;
    Authority _authority = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        state = ("" + Request["state"]).Trim().ToLower();
        account = ("" + Request["account"]).Trim().ToLower();
        currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        _currentUser = Person.FromIdentity(currentUserId);
        _authority = _currentUser.GetAuthority();
        if (!_authority.HasPermission(Permission.CanEditMailDB, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
        {
            Response.Write("You do not have authority to use this.");
            Response.End();
        }

        if (!IsPostBack)
        {
            if (state == "edit")
            {
                LabelAccount.Text = account;
                LabelAccount.Font.Bold = true;
                addSpan.Visible = false;
                pwTR.Visible = true;
                ButtonDelete.Visible = true;
                List<MailServerDatabase.MailAccount> objAccList = MailServerDatabase.SearchAccount(account);
                TextBoxForward.Text = objAccList.Count > 0 ? "" + (objAccList[0].forwardedTo.Count > 0 ? objAccList[0].forwardedTo[0] : "") : "";
            }
            else if (state == "add")
            {
                LabelAccount.Text = "";
                TextBoxAccount.Text = account;
                addSpan.Visible = true;
                pwTR.Visible = true;
                ButtonDelete.Visible = false;
            }
        }
    }

    private string CheckAvailability ()
    {
        string suggestedAccount = (TextBoxAccount.Text.Trim() + "@" + DropDownMailDomain.SelectedValue).ToLower();
        if (!Formatting.ValidateEmailFormat(suggestedAccount))
        {
            suggestedAccount = (TextBoxAccount.Text.Trim()).ToLower();
            if (!Formatting.ValidateEmailFormat(suggestedAccount))
            {
                return "Bad format";
            }
        }
        try
        {
            LabelAccount.Text = suggestedAccount.Split('@')[0]; ;
            DropDownMailDomain.SelectedValue = suggestedAccount.Split('@')[1];
        }
        catch
        {
            return "Bad format";

        }

        List<MailServerDatabase.MailAccount> objAcc = MailServerDatabase.SearchAccount(suggestedAccount);
        foreach (MailServerDatabase.MailAccount acc in objAcc)
        {
            if (acc.account.Trim().ToLower() == suggestedAccount)
            {
                return "Not available";
            }
        }
        return "";
    }

    protected void ButtonAvail_Click (object sender, EventArgs e)
    {

        string resultText = CheckAvailability();
        if (resultText == "")
            LabelAvailability.Text = "Account is available.";
        return;
    }

    protected void ButtonValidate_Click (object sender, EventArgs e)
    {
        string suggestedAccount = TextBoxForward.Text.Trim().ToLower();
        if (!Formatting.ValidateEmailFormat(suggestedAccount))
        {
            LabelValidateError.Text = "Format error";
        }
        else if (suggestedAccount.Contains("@piratpartiet.se"))
        {
            List<MailServerDatabase.MailAccount> objAcc = MailServerDatabase.SearchAccount(suggestedAccount);
            LabelValidateError.Text = "Account not found.";
            foreach (MailServerDatabase.MailAccount acc in objAcc)
            {
                if (acc.account.Trim().ToLower() == suggestedAccount)
                {
                    LabelValidateError.Text = "";
                    break;
                }
            }
        }

    }

    protected void ButtonSaveChanges_Click (object sender, EventArgs e)
    {
        LabelSaveError.Text = "Not Allowed.";
        if (_authority.HasPermission(Permission.CanEditMailDB, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
        {
            LabelSaveError.Text = "Saved.";
            string suggestedAccount = (TextBoxAccount.Text.Trim() + "@" + DropDownMailDomain.SelectedValue).ToLower();
            if (state == "add")
            {
                if (CheckAvailability() == "")
                {
                    if (!Formatting.ValidateEmailFormat(suggestedAccount))
                    {
                        suggestedAccount = (TextBoxAccount.Text.Trim()).ToLower();
                    }
                    try
                    {
                        //MailServerDatabase.AddAccount(suggestedAccount, TextBoxPassword.Text, 1024);
                        PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Created account", "Manually changed in PW", suggestedAccount, "", "");
                        string forwardAccount = TextBoxForward.Text.Trim().ToLower();
                        if (forwardAccount != "")
                        {
                            MailServerDatabase.StartForwarding(suggestedAccount, forwardAccount);
                            PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Changed forwarding", "Manually changed in PW", account, "", forwardAccount);
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "saved", "CloseAndRebind();", true);
                    }
                    catch (Exception ex)
                    {
                        LabelSaveError.Text = ex.Message;
                    }
                }
                else
                {
                    LabelSaveError.Text = "Account is NOT available";
                }
            }
            else
            {
                try
                {
                    List<MailServerDatabase.MailAccount> objAccList = MailServerDatabase.SearchAccount(account);
                    string wasForwarded = objAccList.Count > 0 ? "" + (objAccList[0].forwardedTo.Count > 0 ? objAccList[0].forwardedTo[0] : "") : "";

                    if (TextBoxPassword.Text != "")
                    {
                        MailServerDatabase.SetNewPassword(account, TextBoxPassword.Text);
                        PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Changed password", "Manually changed in PW", account, "", "");
                    }

                    string forwardAccount = TextBoxForward.Text.Trim().ToLower();
                    if (forwardAccount == "" && wasForwarded != "")
                    {
                        MailServerDatabase.StopForwarding(account);
                        PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Stopped forwarding", "Manually changed in PW", account, forwardAccount, "");
                    }
                    else if (forwardAccount != wasForwarded)
                    {
                        MailServerDatabase.StartForwarding(account, forwardAccount);
                        PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Changed forwarding", "Manually changed in PW", account, wasForwarded, forwardAccount);
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "saved", "CloseAndRebind();", true);
                }
                catch (Exception ex)
                {
                    LabelSaveError.Text = ex.Message;
                }
            }
        }
    }

    protected void ButtonDelete_Click (object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanEditMailDB, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
        {
            MailServerDatabase.DeleteAccount(account);
            PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Deleted account", "Manually changed in PW", account, "", "");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "saved", "CloseAndRebind();", true);
        }

    }
}
