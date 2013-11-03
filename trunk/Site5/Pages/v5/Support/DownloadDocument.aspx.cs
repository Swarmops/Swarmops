using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mime;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Interfaces;
using Activizr.Logic.Communications;
using Activizr.Logic.Financial;
using Activizr.Logic.Support;
using Activizr.Basic.Enums;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_v4_DownloadDocument : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string documentIdString = Request.QueryString["DocumentId"];
        int documentId = Int32.Parse(documentIdString);

        Document document = Document.FromIdentity(documentId);

        //Orgid is needed to safely verify permission
        int orgId=Organization.PPSEid; 
        
        bool hasPermission = false;

        switch (document.DocumentType)
        {
            case DocumentType.FinancialTransaction:
            {
                //TODO: Get the orgId from foreign object
                if (_authority.HasPermission(Permission.CanSeeEconomyDetails, orgId, -1, Authorization.Flag.ExactOrganization))
                {
                    hasPermission = true;
                }
            }
            break;
            case DocumentType.ExpenseClaim:
            case DocumentType.InboundInvoice:
            {

                int budgetId = 0;

                if (document.DocumentType == DocumentType.ExpenseClaim)
                {
                    ExpenseClaim claim = (ExpenseClaim) document.ForeignObject;
                    orgId = claim.Budget.OrganizationId;
                    budgetId = claim.BudgetId;
                }
                else
                {
                    InboundInvoice invoice = (InboundInvoice) document.ForeignObject;
                    orgId = invoice.Budget.OrganizationId;
                    budgetId = invoice.BudgetId;
                }

                if (_authority.HasPermission(Permission.CanSeeEconomyDetails, orgId, -1, Authorization.Flag.ExactOrganization))
                {
                    hasPermission = true;
                    break;
                }

                if (FinancialAccount.FromIdentity(budgetId).OwnerPersonId == _currentUser.Identity)
                {
                    hasPermission = true;
                }
                break;
            }
            case DocumentType.PaperLetter:
            {
                PaperLetter letter = (PaperLetter) document.ForeignObject;
                
                if (letter.Recipient.Identity == _currentUser.Identity)
                {
                    hasPermission = true; // A letter to the viewer
                }

                // Otherwise, are there overriding permissions, if not addressed to him/her?

                else if (!letter.Personal)
                {
                    // Unpersonal paper letter, like a rally permit. Note that bank statements should
                    // be considered personal as they contain donors' information in the transaction info.

                    if (_authority.HasPermission(Permission.CanSeeInsensitivePaperLetters, letter.OrganizationId, -1, Authorization.Flag.Default))
                    {
                        hasPermission = true;
                    }
                }
                else if (letter.ToPersonId == 0)
                {
                    // Addressed to the organization, not to a specific person, but still personal.
                    // Typical examples include political inquiries from private citizens written on
                    // paper.

                    if (_authority.HasPermission(Permission.CanSeeSensitivePaperLetters, letter.OrganizationId, -1, Authorization.Flag.Default))
                    {
                        hasPermission = true;
                    }
                }
                else
                {
                    // Addressed to a specific individual that is not the viewer, and it's personal. 
                    // INVOCATION OF THIS CODE IS A BREACH OF THE POSTAL SECRET and should ONLY EVER 
                    // be done for technical, not operational, reasons and preferably NEVER.

                    if (_authority.HasPermission(Permission.CanBreachPostalSecretPaperLetters, letter.OrganizationId, -1, Authorization.Flag.Default))
                    {
                        hasPermission = true;
                    }
                }
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

        string serverPath = @"C:\Data\Uploads\PirateWeb"; // TODO: Read from web.config

        string contentType = string.Empty;
        
        if (document.ServerFileName.EndsWith(".pdf"))
        {
            contentType = MediaTypeNames.Application.Pdf;
        }
        else if (document.ServerFileName.EndsWith(".png"))
        {
            contentType = "image/png"; // why isn't this in MediaTypeNames?
        }
        else if (document.ServerFileName.EndsWith(".jpg"))
        {
            contentType = MediaTypeNames.Image.Jpeg;
        }

        Response.ContentType = contentType + "; filename=" + document.ClientFileName;
        Response.TransmitFile(serverPath + Path.DirectorySeparatorChar + document.ServerFileName);
    }
}
