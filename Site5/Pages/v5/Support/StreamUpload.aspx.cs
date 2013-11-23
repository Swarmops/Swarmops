using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Pages.v5.Support
{
    public partial class StreamUpload : DataV5Base
    {
        public string StorageRoot
        {
            get
            {
                if (Debugger.IsAttached)
                {
                    return @"C:\Windows\Temp\\Swarmops-Debug\\"; // Windows debugging environment
                }
                else
                {
                    return "/opt/swarmops/upload/"; // production location on Debian installation  TODO: config file
                }
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string documentIdString = Request.QueryString["DocId"];
            int documentId = Int32.Parse(documentIdString);

            Document document = Document.FromIdentity(documentId);

            //Orgid is needed to safely verify permission
            int orgId = Organization.PPSEid;

            bool hasPermission = false;

            switch (document.DocumentType)
            {
                case DocumentType.FinancialTransaction:
                    {/*
                        //TODO: Get the orgId from foreign object
                        if (this.CurrentAuthority.HasPermission(Permission.CanSeeEconomyDetails, orgId, -1, Authorization.Flag.ExactOrganization))
                        {
                            hasPermission = true;
                        }*/
                    }
                    break;
                case DocumentType.ExpenseClaim:
                case DocumentType.InboundInvoice:
                    {

                        int budgetId = 0;

                        if (document.DocumentType == DocumentType.ExpenseClaim)
                        {
                            ExpenseClaim claim = (ExpenseClaim)document.ForeignObject;
                            orgId = claim.Budget.OrganizationId;
                            budgetId = claim.BudgetId;
                        }
                        else
                        {
                            InboundInvoice invoice = (InboundInvoice)document.ForeignObject;
                            orgId = invoice.Budget.OrganizationId;
                            budgetId = invoice.BudgetId;
                        }


                        FinancialAccount budget = FinancialAccount.FromIdentity(budgetId);

                        if (budget.OwnerPersonId == this.CurrentUser.Identity || budget.OwnerPersonId == 0)
                        {
                            hasPermission = true;
                            break;
                        }
                        /*
                        if (this.CurrentAuthority.HasPermission(Permission.CanSeeEconomyDetails, orgId, -1, Authorization.Flag.ExactOrganization))
                        {
                            hasPermission = true;
                            break;
                        }*/

                        break;
                    }
                case DocumentType.PaperLetter:
                    {
                        PaperLetter letter = (PaperLetter)document.ForeignObject;

                        if (letter.Recipient.Identity == this.CurrentUser.Identity)
                        {
                            hasPermission = true; // A letter to the viewer
                        }
                            /*
                        // Otherwise, are there overriding permissions, if not addressed to him/her?

                        else if (!letter.Personal)
                        {
                            // Unpersonal paper letter, like a rally permit. Note that bank statements should
                            // be considered personal as they contain donors' information in the transaction info.

                            if (this.CurrentAuthority.HasPermission(Permission.CanSeeInsensitivePaperLetters, letter.OrganizationId, -1, Authorization.Flag.Default))
                            {
                                hasPermission = true;
                            }
                        }
                        else if (letter.ToPersonId == 0)
                        {
                            // Addressed to the organization, not to a specific person, but still personal.
                            // Typical examples include political inquiries from private citizens written on
                            // paper.

                            if (this.CurrentAuthority.HasPermission(Permission.CanSeeSensitivePaperLetters, letter.OrganizationId, -1, Authorization.Flag.Default))
                            {
                                hasPermission = true;
                            }
                        }
                        else
                        {
                            // Addressed to a specific individual that is not the viewer, and it's personal. 
                            // INVOCATION OF THIS CODE IS A BREACH OF THE POSTAL SECRET and should ONLY EVER 
                            // be done for technical, not operational, reasons and preferably NEVER.

                            if (this.CurrentAuthority.HasPermission(Permission.CanBreachPostalSecretPaperLetters, letter.OrganizationId, -1, Authorization.Flag.Default))
                            {
                                hasPermission = true;
                            }
                        }*/
                    }
                    break;
                case DocumentType.PersonPhoto:
                    {
                        // These are public

                        hasPermission = true;
                    }
                    break;
            }

            if (!hasPermission)
            {
                throw new Exception("Access is not allowed");
            }

            string contentType = string.Empty;

            string fileNameLower = document.ClientFileName.ToLowerInvariant();

            if (fileNameLower.EndsWith(".pdf"))
            {
                contentType = MediaTypeNames.Application.Pdf;
            }
            else if (fileNameLower.EndsWith(".png"))
            {
                contentType = "image/png"; // why isn't this in MediaTypeNames?
            }
            else if (fileNameLower.EndsWith(".jpg"))
            {
                contentType = MediaTypeNames.Image.Jpeg;
            }

            string legacyMarker = string.Empty;

            if (!File.Exists(StorageRoot + document.ServerFileName))
            {
                legacyMarker = "legacy/"; // for some legacy installations, all older files are placed here
            }

            // TODO: If still doesn't exist, perhaps return a friendly error image instead?

            Response.ContentType = contentType + "; filename=" + document.ClientFileName;
            Response.TransmitFile(StorageRoot + legacyMarker + document.ServerFileName);
        }
    }
}