using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using Swarmops.Common.Enums;
using Swarmops.Frontend;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
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
                    return @"C:\Windows\Temp\Swarmops-Debug\"; // Windows debugging environment
                }
                return "/var/lib/swarmops/upload/"; // production location on Debian installation  TODO: config file
            }
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            string documentIdString = Request.QueryString["DocId"];
            int documentId = Int32.Parse (documentIdString);

            Document document = Document.FromIdentity (documentId);

            //Orgid is needed to safely verify permission
            int orgId = 0; // initialize to invalid

            bool hasPermission = false;
            string serverFileName = document.ServerFileName;

            if (CurrentOrganization.HasOpenLedgers)
            {
                hasPermission = true;
            }
            else
            {


                switch (document.DocumentType)
                {
                    case DocumentType.FinancialTransaction:
                    {
/*
                        //TODO: Get the orgId from foreign object
                        if (this.CurrentAuthority.HasPermission(Permission.CanSeeEconomyDetails, orgId, -1, Authorization.Flag.ExactOrganization))
                        {
                            hasPermission = true;
                        }*/
                    }
                        break;
                    case DocumentType.ExpenseClaim:
                    case DocumentType.InboundInvoice:
                    case DocumentType.OutboundInvoice:
                    {
                        int budgetId = 0;

                        if (document.DocumentType == DocumentType.ExpenseClaim)
                        {
                            ExpenseClaim claim = (ExpenseClaim) document.ForeignObject;
                            orgId = claim.Budget.OrganizationId;
                            budgetId = claim.BudgetId;
                        }
                        else if (document.DocumentType == DocumentType.InboundInvoice)
                        {
                            InboundInvoice invoice = (InboundInvoice) document.ForeignObject;
                            orgId = invoice.Budget.OrganizationId;
                            budgetId = invoice.BudgetId;
                        }
                        else
                        {
                            OutboundInvoice invoice = (OutboundInvoice) document.ForeignObject;
                            orgId = invoice.OrganizationId;
                            budgetId = invoice.BudgetId;
                        }


                        FinancialAccount budget = FinancialAccount.FromIdentity(budgetId);

                        if (budget.OwnerPersonId == CurrentUser.Identity || budget.OwnerPersonId == 0)
                        {
                            hasPermission = true;
                            break;
                        }

                        // TODO: Security leak - check CurrentOrganization against Document's org

                        if (
                            CurrentAuthority.HasAccess(new Access(CurrentOrganization, AccessAspect.Financials,
                                AccessType.Write)))
                        {
                            hasPermission = true;
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
                        PaperLetter letter = (PaperLetter) document.ForeignObject;

                        if (letter.Recipient.Identity == CurrentUser.Identity)
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
                    case DocumentType.Logo:
                    case DocumentType.Artwork:
                    {
                        // These are public

                        hasPermission = true;
                    }
                        break;

                }
            }

            if (!hasPermission)
            {
                throw new Exception ("Access is not allowed");
            }

            string contentType = string.Empty;

            string fileNameLower = document.ClientFileName.ToLowerInvariant();

            if (fileNameLower.EndsWith (".pdf"))
            {
                contentType = MediaTypeNames.Application.Pdf;
            }
            else if (fileNameLower.EndsWith (".png"))
            {
                contentType = "image/png"; // why isn't this in MediaTypeNames?
                if (Request.QueryString["hq"] == "1")
                {
                    string testFileName = serverFileName.Replace(".png", "-hires.png");
                    if (File.Exists(Document.StorageRoot + testFileName))
                    {
                        serverFileName = testFileName;
                    }
                }
            }
            else if (fileNameLower.EndsWith (".jpg"))
            {
                contentType = MediaTypeNames.Image.Jpeg;
            }

            string legacyMarker = string.Empty;

            if (!File.Exists (StorageRoot + serverFileName))
            {
                legacyMarker = "legacy/"; // for some legacy installations, all older files are placed here
            }

            // TODO: If still doesn't exist, perhaps return a friendly error image instead?

            if (!File.Exists (StorageRoot + legacyMarker + serverFileName))
            {
                if (!Debugger.IsAttached) // if running live; ignore FNF errors when debugging
                {
                    throw new FileNotFoundException(StorageRoot + legacyMarker + serverFileName);
                }
                else
                {
                    Response.StatusCode = 404;
                    Response.End();
                    return;
                }
            }

            Response.ContentType = contentType + "; filename=" + document.ClientFileName;
            Response.TransmitFile (StorageRoot + legacyMarker + serverFileName);
        }
    }
}