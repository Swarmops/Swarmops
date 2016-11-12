using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Activism_LogActivism : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.DropGeographies.Root = Organization.PPSE.DefaultCountry.Geography;
            try
            {
                this.DropGeographies.SelectedGeography = _currentUser.Geography;
            }
            catch { }
            this.TextDescription.Style[HtmlTextWriterStyle.Width] = "300px";
            this.DropActivityType.Style[HtmlTextWriterStyle.Width] = "300px";
            this.DatePicker.SelectedDate = DateTime.Today;
        }
    }

    protected void ButtonUpload_Click (object sender, EventArgs e)
    {
        ProcessUpload();

        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        this.DocumentList.Documents = Documents.ForObject(new TemporaryIdentity(temporaryId));
    }


    private void ProcessUpload ()
    {
        TemporaryIdentity tempId = new TemporaryIdentity(Int32.Parse(this.TemporaryDocumentIdentity.Text));

        if (tempId.Identity == 0)
        {
            // Ok, storing the temporary id in the page state is REALLY ugly.
            // How are you supposed to solve this class of problems, where you
            // need to persist data on a page for later processing?

            tempId = TemporaryIdentity.GetNew();
            this.TemporaryDocumentIdentity.Text = tempId.Identity.ToString();
        }

        string serverPath = @"C:\Data\Uploads\PirateWeb";  // TODO: Read from web.config

        foreach (UploadedFile file in this.Upload.UploadedFiles)
        {
            string clientFileName = file.GetName();
            string extension = file.GetExtension();

            Document newDocument =
                Document.Create(Guid.NewGuid().ToString() + ".tmp", clientFileName,
                                file.ContentLength, string.Empty, tempId, _currentUser);

            string serverFileName = String.Format("document_{0:D5}_activityphotobyperson_{1:D6}{2}", newDocument.Identity, _currentUser.Identity, file.GetExtension().ToLower());
            file.SaveAs(serverPath + Path.DirectorySeparatorChar + serverFileName);

            newDocument.ServerFileName = serverFileName;

            File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());
        }
    }

    protected void ButtonLogActivity_Click (object sender, EventArgs e)
    {
        // First, if there's an upload that the user hasn't processed, process it first.

        if (this.Upload.UploadedFiles.Count > 0)
        {
            ProcessUpload();
        }

        // If args were invalid, abort

        if (!Page.IsValid)
        {
            return;
        }


        // Read the form data

        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Geography geography = this.DropGeographies.SelectedGeography;

        DateTime created = DateTime.Now;
        DateTime activityDate = (DateTime)this.DatePicker.SelectedDate;
        string description = this.TextDescription.Text;
        ExternalActivityType type =
            (ExternalActivityType)Enum.Parse(typeof(ExternalActivityType), this.DropActivityType.SelectedValue);

        // Create the activism record

        ExternalActivity activity = ExternalActivity.Create(Organization.FromIdentity(organizationId), geography, type,
                                                            activityDate, description, _currentUser);

        // Move documents to the new activism

        Documents.ForObject(new TemporaryIdentity(temporaryId)).SetForeignObjectForAll(activity);

        // Create the event for PirateBot-Mono to send off mails

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ActivismLogged,
                                                   _currentUser.Identity, organizationId, geography.Identity, _currentUser.Identity,
                                                   activity.Identity, string.Empty);

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('The activism has been logged.');", true);

        // Clear the text fields

        this.TextDescription.Text = string.Empty;
        this.TemporaryDocumentIdentity.Text = "0";

    }


    protected void Validator_DocumentList_Custom_ServerValidate (object source, ServerValidateEventArgs args)
    {
        // There need to be documents, either already uploaded or in the upload pipe now.

        if (this.TemporaryDocumentIdentity.Text != "0" || this.Upload.UploadedFiles.Count > 0)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }


    protected void Validator_DropGeographies_Custom_ServerValidate (object source, ServerValidateEventArgs args)
    {
        // A budget must be selected.

        // First, an organization must be selected.

        if (this.DropOrganizations.SelectedValue == "0")
        {
            args.IsValid = false;
            return;
        }

        if (this.DropGeographies.SelectedGeography == null)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void Validator_DatePicker_Custom_ServerValidate (object source, ServerValidateEventArgs args)
    {
        if (this.DatePicker.SelectedDate == null)
        {
            args.IsValid = false;
            return;
        }

        DateTime selectedDate = (DateTime)this.DatePicker.SelectedDate;

        if (selectedDate > DateTime.Today)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }

    /*
    protected void Validator_TextAmount_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // Validate that the TextAmount box holds a parsable double.

        double dummy;

        args.IsValid = Double.TryParse(this.TextAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"), out dummy);
    }*/
}
