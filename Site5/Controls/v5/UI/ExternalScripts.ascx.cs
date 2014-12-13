using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.UI;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Controls.v5.UI
{
    public partial class ExternalScripts : ControlV5Base
    {
        public string Package { get; set; }
        public new string Controls { get; set; }

        protected void Page_Load (object sender, EventArgs e)
        {
            string externalScriptUrl = "//hostedscripts.falkvinge.net";

            string testFolderName = Server.MapPath ("~/Scripts/ExternalScripts");
            if (Directory.Exists (testFolderName))
            {
                externalScriptUrl = "/Scripts/ExternalScripts";
            }
            else if (Debugger.IsAttached ||
                     PilotInstallationIds.IsPilot (PilotInstallationIds.DevelopmentSandbox))
            {
                externalScriptUrl += "/staging";
                // use staging area for new script versions on Sandbox and for all debugging
            }

            if (Package == "easyui")
            {
                StringBuilder scriptRef = new StringBuilder();

                scriptRef.Append ("<script src=\"" + externalScriptUrl +
                                  "/easyui/jquery.easyui.min.js\" type=\"text/javascript\"></script>\r\n");
                scriptRef.Append ("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                  "/easyui/themes/icon.css\" />\r\n");
                string[] controlNames = Controls.Split (',');
                foreach (string controlName in controlNames)
                {
                    string controlNameLower = controlName.Trim().ToLowerInvariant();
                    if (controlNameLower != "unknown")
                    {
                        scriptRef.AppendFormat (
                            "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                            "/easyui/themes/default/{0}.css\" />\r\n",
                            controlNameLower);
                    }
                }

                this.LiteralReference.Text = scriptRef.ToString();
            }
        }
    }
}