using System;
using System.IO;

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

            if (Package == "easyui")
            {
                this.LiteralReference1.Text = "<script src=\"" + externalScriptUrl +
                                        "/easyui/jquery.easyui.min.js\" type=\"text/javascript\"></script>";
                this.LiteralReference2.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                        "/easyui/themes/icon.css\" />";
                this.LiteralReference3.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                        "/easyui/themes/default/" + this.Control + ".css\" />";
                this.LiteralReference4.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                        "/easyui/themes/default/datagrid.css\" />";
                this.LiteralReference5.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + externalScriptUrl +
                                        "/easyui/themes/default/tree.css\" />";
            }
        }

        public string Package { get; set; }
        public string Control { get; set; }
    }
}