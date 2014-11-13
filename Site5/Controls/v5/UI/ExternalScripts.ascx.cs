using System;
using System.IO;
using System.Text;
using NBitcoin;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Controls.v5.UI
{
    public partial class ExternalScripts : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string externalScriptUrl = "//hostedscripts.falkvinge.net";

            string testFolderName = Server.MapPath("~/Scripts/ExternalScripts");
            if (Directory.Exists(testFolderName))
            {
                externalScriptUrl = "/Scripts/ExternalScripts";
            }
            else if (System.Diagnostics.Debugger.IsAttached || PilotInstallationIds.IsPilot(PilotInstallationIds.DevelopmentSandbox))
            {
                externalScriptUrl += "/staging"; // use staging area for new script versions on Sandbox and for all debugging
            }

            if (Package == "easyui")
            {
                StringBuilder scriptRef = new StringBuilder();

                scriptRef.Append("<script src=\"" + externalScriptUrl +
                                 "/easyui/jquery.easyui.min.js\" type=\"text/javascript\"></script>\r\n");
                scriptRef.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                        "/easyui/themes/icon.css\" />\r\n");
                string[] controlNames = Controls.Split(',');
                foreach (string controlName in controlNames)
                {
                    scriptRef.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                            "/easyui/themes/default/{0}.css\" />\r\n", controlName.Trim().ToLowerInvariant());
                }

                this.LiteralReference.Text = scriptRef.ToString();
            }
        }

        public string Package { get; set; }
        public new string Controls { get; set; }
    }
}