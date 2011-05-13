using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

namespace Activizr.Site.Pages.v4.Financial
{

    public partial class UploadFinancialData : PageV4Base
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            // Identify person

            // HACK as to who has access to what data -- improve this later
            if (!IsPostBack)
            {
                if (_currentUser.Identity == 1)
                {
                    this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString()));
                    this.DropOrganizations.Items.Add(new ListItem("Sandbox", "3"));
                    this.DropOrganizations.Items.Add(new ListItem("Rick's Sandbox", "55")); // Debug & Test
                }
                else if (_authority.HasPermission(Permission.CanDoEconomyTransactions, Organization.PPSEid, -1, Authorization.Flag.ExactOrganization))
                {


                    this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString()));
                }
                else
                {
                    // Show some dialog saying that the user has no access to the tools on this page
                }
            }

        }



        protected void DropOrganizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
            Organization organization = Organization.FromIdentity(organizationId);

            // TODO: CHANGE FILTER SETTINGS

        }

        protected void DropFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            switch (this.DropFilters.SelectedValue)
            {
                case "SEBgmax":
                    this.LabelFilterInstructions.Text =
                        "Download the payment file from the bank. Open it - it's a text file. Paste its contents here.";
                    break;
                default:
                    this.LabelFilterInstructions.Text =
                        "Select an import filter.";
                    break;
            }*/
        }

        protected void ButtonUpload_Click(object sender, EventArgs e)
        {
            ProcessUpload();
        }


        private void ProcessUpload()
        {
            string serverPath = @"C:\Data\Uploads\PirateWeb";  // TODO: Read from web.config

            foreach (Telerik.Web.UI.UploadedFile file in this.Upload.UploadedFiles)
            {
                string clientFileName = file.GetName();
                string extension = file.GetExtension();


                string base64 = Convert.ToBase64String(File.ReadAllBytes(serverPath + Path.DirectorySeparatorChar + file.GetName()));

                PWEvents.CreateEvent(EventSource.PirateWeb, EventType.FinancialDataUploaded, _currentUser.Identity,
                                     Int32.Parse(this.DropOrganizations.SelectedValue), Geography.RootIdentity,
                                     _currentUser.Identity, Int32.Parse(this.DropFilters.SelectedValue), base64);

                File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());

                Page.ClientScript.RegisterStartupScript(Page.GetType(), "alldone",
                                "alert ('File " + file.GetName().Replace("'", "''") + " was sent for further processing using the " + this.DropFilters.SelectedItem.Text + " filter. You will receive mail and/or SMS when processing has completed.');",
                                true);

            }
        }

        /*

        */



    }
}