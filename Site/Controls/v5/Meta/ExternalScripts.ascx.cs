using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.UI;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Controls.Meta
{
    public partial class ExternalScripts : ControlV5Base
    {
        public string Package { get; set; }
        public new string Controls { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string externalScriptUrl = "//hostedscripts.falkvinge.net";

            string testFolderName = Server.MapPath("~/Scripts/ExternalScripts");
            if (Directory.Exists(testFolderName))
            {
                externalScriptUrl = "/Scripts/ExternalScripts";
            }

            // If we're debugging a seriously experimental new version of JEasyUI, look for it in /Scripts/Experimental
            // (a folder which doesn't commit to the github repo)

            if (File.Exists(Server.MapPath("~/Scripts/Experimental/easyui/jquery.easyui.min.js")))
            {
                externalScriptUrl = "/Scripts/Experimental";
            }

            StringBuilder scriptRef = new StringBuilder();

            switch (Package.ToLowerInvariant())
            {
                case "easyui":

                    scriptRef.Append("<script src=\"" + externalScriptUrl +
                                     "/easyui/jquery.easyui.min.js\" type=\"text/javascript\"></script>\r\n");
                    scriptRef.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                     "/easyui/themes/icon.css\" />\r\n");
                    scriptRef.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                     "/easyui/themes/default/easyui.css\" />\r\n"); // Supposed to contain all CSS

                    if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
                    {
                        scriptRef.Append("<script src=\"" + externalScriptUrl +
                                         "/easyui/extensions/easyui-rtl.js\" type=\"text/javascript\"></script>\r\n");
                        scriptRef.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                         "/easyui/extensions/easyui-rtl.css\" />\r\n");
                    }

                    /* -- with the inclusion of the catchall CSS file, this code _should_ no longer be necessary...
                    string[] controlNames = Controls.Split(',');
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
                    }*/

                    break;

                case "fancybox":

                    scriptRef.Append("<script src=\"" + externalScriptUrl +
                                     "/fancybox/jquery.fancybox.min.js\" type=\"text/javascript\"></script>\r\n");
                    scriptRef.Append("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                     "/fancybox/jquery.fancybox.min.css\" />\r\n"); // Supposed to contain all CSS

                    // If we're including Fancybox, always also include Elevated Zoom, which isn't external

                    scriptRef.Append(
                        "<script src='/Scripts/jquery.elevateZoom-3.0.8.min.js' type='text/javascript'></script>\r\n");

                    break;

                default:
                    throw new NotImplementedException("Unimplemented external script package");
            }

            this.LiteralReference.Text = scriptRef.ToString();

        }
    }
}
