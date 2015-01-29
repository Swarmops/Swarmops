using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic;
using Swarmops.Logic.Support;
using Swarmops.Logic.Security;
using System.Globalization;
using System.Web.Services;
using System.Text.RegularExpressions;
using Swarmops.Frontend.Controls.v5.Base;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class Settings : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // HACK: The organization part must be removed once proper access control is in place
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.System, AccessType.Write);
            this.IncludedControlsUsed = IncludedControl.JsonParameters | IncludedControl.SwitchButton;
            this.EasyUIControlsUsed = EasyUIControl.Tabs;
            this.PageTitle = Resources.Pages.Admin.SystemSettings_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Admin.SystemSettings_Info;

            if (!Page.IsPostBack)
            {
                this.TextSmtpServer.Text = FormatSmtpAccessString(SystemSettings.SmtpUser, SystemSettings.SmtpPassword,
                    SystemSettings.SmtpHost, SystemSettings.SmtpPort);
                this.TextExternalUrl.Text = SystemSettings.ExternalUrl;
                this.TextInstallationName.Text = SystemSettings.InstallationName;

                Localize();
            }
        }

        private static string FormatSmtpAccessString (string user, string pass, string host, int port)
        {
            string result = host;
            if (port != 25)
            {
                result += ":" + port.ToString(CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(pass))
            {
                user += ":" + pass;
            }

            if (!string.IsNullOrEmpty(user))
            {
                result = user + "@" + result;
            }

            return result;
        }

        private void Localize()
        {
            this.LabelExternalUrl.Text = Resources.Pages.Admin.SystemSettings_ExternalUrl;
            this.LabelSmtpServer.Text = Resources.Pages.Admin.SystemSettings_SmtpServer;
            this.LabelInstallationName.Text = Resources.Pages.Admin.SystemSettings_InstallationName;
        }

        [WebMethod]
        static public AjaxTextBox.CallbackResult StoreCallback(string newValue, string cookie)
        {
            AjaxTextBox.CallbackResult result = new AjaxTextBox.CallbackResult();

            switch (cookie)
            {
                case "Smtp":
                    Match match = Regex.Match (newValue, "((?<user>[a-z0-9_]+)(:(?<pass>[^@]+))?@)?(?<host>[a-z0-9_\\-\\.]+)(:(?<port>[0-9]+))?", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string user = match.Groups["user"].Value;
                        string pass = match.Groups["pass"].Value;
                        string host = match.Groups["host"].Value;
                        string portString = match.Groups["port"].Value;
                        int port = 25;

                        if (!string.IsNullOrEmpty(portString))
                        {
                            try
                            {
                                port = Int32.Parse(portString);
                            }
                            catch (FormatException)
                            {
                                result.DisplayMessage = Resources.Pages.Admin.SystemSettings_Error_SmtpHostPort;
                                result.ResultCode = AjaxTextBox.CodeInvalid;
                                return result; // return early
                            }
                        }

                        SystemSettings.SmtpUser = user ?? string.Empty;
                        SystemSettings.SmtpPassword = pass ?? string.Empty;
                        SystemSettings.SmtpHost = host;
                        SystemSettings.SmtpPort = port;

                        OutboundComm.CreateNotification(Organization.Sandbox, Logic.Communications.Transmission.NotificationResource.System_MailServerTest);

                        result.ResultCode = AjaxTextBox.CodeChanged;
                        result.NewData = FormatSmtpAccessString (user, pass, host, port);
                        result.DisplayMessage = Resources.Pages.Admin.SystemSettings_TestMailSent;
                    }
                    else
                    {
                        result.ResultCode = AjaxTextBox.CodeInvalid;
                        result.DisplayMessage = Resources.Pages.Admin.SystemSettings_Error_SmtpSyntax;
                    }
                    break;

                case "ExtUrl":
                    if (!newValue.EndsWith("/"))
                    {
                        newValue = newValue + "/";
                    }
                    if (!newValue.StartsWith("http://") && !newValue.StartsWith("https://"))
                    {
                        newValue = "https://" + newValue;
                    }

                    SystemSettings.ExternalUrl = newValue;

                    result.NewData = newValue;
                    result.ResultCode = AjaxTextBox.CodeSuccess;
                    break;

                case "InstallationName":
                    result.NewData = newValue.Trim();
                    result.ResultCode = AjaxTextBox.CodeSuccess;
                    SystemSettings.InstallationName = newValue;
                    break;

                default:
                    throw new NotImplementedException("Unknown cookie in StoreCallback");
            }

            return result;
        }


    }

}