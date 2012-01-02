using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Security;
using Telerik.Web.UI;
using Telerik.Web.UI.Upload;


// ReSharper disable CheckNamespace
namespace Activizr.Site.Pages.Ledgers
// ReSharper restore CheckNamespace
{
    public partial class UploadBankFiles : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = Resources.Pages.Ledgers.UploadBankFiles_PageTitle;
            this.PageIcon = "iconshock-bank";
            this.PageAccessRequired = new Access(_currentOrganization, AccessAspect.Bookkeeping, AccessType.Write);

            this.LabelSidebarInfo.Text = Resources.Pages.Global.Sidebar_Information;
            this.LabelSidebarActions.Text = Resources.Pages.Global.Sidebar_Actions;
            this.LabelSidebarTodo.Text = Resources.Pages.Global.Sidebar_Todo;
            this.LabelDownloadInstructions.Text = Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions;
            this.LabelClickImage.Text = Resources.Pages.Global.Global_ClickImageToEnlarge;

            this.LabelUploadBankFilesInfo.Text = Resources.Pages.Ledgers.UploadBankFiles_Info;
            this.LabelActionItemsHere.Text = Resources.Pages.Global.Sidebar_Todo_Placeholder;

            this.LabelSelectBankAndAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectBankAndAccount;
            this.LabelSelectFileType.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectBankFileType;
            this.LabelSelectAccount.Text = Resources.Pages.Ledgers.UploadBankFiles_SelectAccount;
            this.LabelUploadH2Header.Text = Resources.Pages.Ledgers.UploadBankFiles_UploadBankFile;
            this.LabelUploadH3Header.Text = Resources.Pages.Global.Global_UploadFileToActivizr;
            this.Upload.Text = Resources.Pages.Global.Global_UploadFile;
            this.LabelProcessing.Text = Resources.Pages.Global.Global_ProcessingFile;

            if (!IsPostBack)
            {
                //Do not display SelectedFilesCount progress indicator.
                this.ProgressIndicator.ProgressIndicators &= ~ProgressIndicators.SelectedFilesCount;
                RadProgressContext progress = RadProgressContext.Current;
                //Prevent the secondary progress from appearing when the file is uploaded (FileCount etc.)
                progress["SecondaryTotal"] = "0";
                progress["SecondaryValue"] = "0";
                progress["SecondaryPercent"] = "0";
            }
        }

        protected void ButtonSebAccountFile_Click(object sender, ImageClickEventArgs e)
        {
            this.ButtonSebAccountFile.CssClass = "FileTypeImage FileTypeImageSelected";
            this.ButtonSebPaymentFile.CssClass = "FileTypeImage UnselectedType";

            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeType", "$(\".UnselectedType\").fadeTo('fast',0.2);", true);
            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeAccount1",
                                                    "$(\"#DivSelectAccount\").fadeTo('slow', 1.0);", true);
            ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeAccount2",
                                                       "$(\"#DivSelectAccount\").css('display','inline');", true);

            PopulateAccountDropDown();

            this.ButtonSebAccountFile.Enabled = false;
            this.ButtonSebPaymentFile.Enabled = false;

            this.ImageDownloadInstructions.ImageUrl = "~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-small.png";

            this.ImageDownloadInstructionsFull.ImageUrl =
                "~/Images/Ledgers/uploadbankfiles-seb-kontoutdrag-full.png";

            this.LiteralDownloadInstructions.Text =
                this.LiteralDownloadInstructionsModal.Text =
                Resources.Pages.Ledgers.UploadBankFiles_DownloadInstructions_SebAccountFile;

        }

        private void PopulateAccountDropDown()
        {
            FinancialAccounts accounts = FinancialAccounts.ForOrganization(_currentOrganization,
                                                                           FinancialAccountType.Asset);

            this.DropAccounts.Items.Clear();
            this.DropAccounts.Items.Add(Resources.Pages.Global.Global_DropInits_SelectFinancialAccount);

            foreach (FinancialAccount account in accounts)
            {
                this.DropAccounts.Items.Add(new ListItem(account.Name, account.Identity.ToString()));
            }

        }

        protected void DropAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LiteralSelectAccountDivStyle.Text = @"style=""opacity:1;display:inline""";

            if (this.DropAccounts.SelectedIndex > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "FadeDownload2",
                                                        "$(\"#DivInstructions\").fadeTo('slow',1.0);", true);
                ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "ShowInstructions",
                                                        "$(\"#DivInstructions\").css('display','inline');", true);
                ScriptManager.RegisterClientScriptBlock(this.PanelFileTypeAccount, this.PanelFileTypeAccount.GetType(), "ReiterateModality",
                                                        "$(function () { $(\"a[rel*=leanModal]\").leanModal();});", true);


                FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(this.DropAccounts.SelectedValue));

                FinancialAccountRows rows = account.GetLastRows(1);
                DateTime lastTransactionDateTime = new DateTime(1801, 1, 1);

                if (rows != null && rows.Count > 0)
                {
                    lastTransactionDateTime = rows[0].RowDateTime;
                }

                this.LiteralLastAccountRecord.Text = String.Format(Resources.Pages.Ledgers.UploadBankFiles_DownloadDataSince, Server.HtmlEncode(account.Name), lastTransactionDateTime, lastTransactionDateTime.AddDays(-5));
            }
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            bool fileWasUploaded = false;

            foreach (string fileInputID in Request.Files)
            {
                UploadedFile file = UploadedFile.FromHttpPostedFile(Request.Files[fileInputID]);
                if (file.ContentLength > 0)
                {

                    // TODO: PROCESS
                    // file.SaveAs("c:\\temp\\" + file.GetName());
                    RadProgressContext progress = RadProgressContext.Current;
                    progress.Speed = "N/A";

                    const int total = 100;

                    for (int i = 0; i < total; i++)
                    {
                        progress["PrimaryPercent"] = i.ToString();
                        progress["PrimaryTotal"] = total.ToString();
                        progress["PrimaryValue"] = i.ToString();

                        progress["SecondaryTotal"] = total.ToString();
                        progress["SecondaryValue"] = i.ToString();
                        progress["SecondaryPercent"] = i.ToString();
                        progress["PrimaryProgressBar"] = i.ToString();
                        progress["CurrentOperationText"] = "File is being processed...";

                        if (!Response.IsClientConnected)
                        {
                            //Cancel button was clicked or the browser was closed, so stop processing
                            break;
                        }

                        //Stall the current thread for 0.1 seconds
                        System.Threading.Thread.Sleep(100);
                    }

                    this.PanelFileTypeAccount.Visible = false;
                    this.PanelResults.Visible = true;
                    fileWasUploaded = true;
                }

            }

            if (!fileWasUploaded)
            {
                // If no file was uploaded, re-show the instructions div

                this.LabelNoFileUploaded.Text = Resources.Pages.Global.Global_Upload_ErrorSelectFile;
                this.LiteralDivInstructionsStyle.Text = @"style='display:inline'";
            }
            else
            {
                this.LabelNoFileUploaded.Text = string.Empty;
                this.LiteralDivInstructionsStyle.Text = @"style='display:none'";
            }
        }
    }
}