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
using Activizr.Basic.Types;
using Activizr.Logic.Communications;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Communications_ReceivedPaperLetter : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ComboRecipient.Authority = _authority;

        // TODO: Re-prime DocumentList

        if (!Page.IsPostBack)
        {
            this.DatePicker.SelectedDate = DateTime.Today;
            this.DocumentList.Documents = new Documents();
        }
    }

    protected void ButtonUpload_Click(object sender, EventArgs e)
    {
        ProcessUpload();

        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        if (temporaryId == 0)
        {
            throw new Exception("Temporary Id is still zero. This should not be possible. Please report this.");
        }

        this.DocumentList.Documents = Documents.ForObject(new TemporaryIdentity(temporaryId));
    }


    private void ProcessUpload()
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
                Document.Create (Guid.NewGuid().ToString() + ".tmp", clientFileName,
                                file.ContentLength, string.Empty, tempId, _currentUser);

            string serverFileName = String.Format("document_{0:D5}_paperletterbyperson_{1:D6}{2}", newDocument.Identity, _currentUser.Identity, file.GetExtension().ToLower());
            file.SaveAs(serverPath + Path.DirectorySeparatorChar + serverFileName);

            newDocument.ServerFileName = serverFileName;

            File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());
        }
    }

    protected void ButtonStoreLetter_Click(object sender, EventArgs e)
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

        string from = this.TextFrom.Text;
        string[] replyAddressLines = this.TextAddress.Text.Trim().Replace("\r", "").Split('\n');
        
        int temporaryId = Int32.Parse(this.TemporaryDocumentIdentity.Text);

        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        DateTime created = DateTime.Now;
        DateTime receivedDate = (DateTime) this.DatePicker.SelectedDate;

        int toPersonId = this.ComboRecipient.HasSelection ? this.ComboRecipient.SelectedPerson.Identity : 0;

        RoleType roleType = RoleType.Unknown;

        if (toPersonId > 0)
        {
            roleType = (RoleType) Int32.Parse(this.DropRoles.SelectedValue);
        }

        bool personal = true;

        if (this.DropPersonal.SelectedValue=="NotPersonal")
        {
            personal = false;
        }

        // Create the paper letter record

        PaperLetter newLetter = PaperLetter.Create(_currentUser.Identity, organizationId, from, replyAddressLines,
                                                   receivedDate, toPersonId, roleType, personal);

        // Move documents to the new letter

        Documents.ForObject(new TemporaryIdentity(temporaryId)).SetForeignObjectForAll(newLetter);

        // Create the event for PirateBot-Mono to send off mails

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.PaperLetterReceived,
                                                   _currentUser.Identity, organizationId, 1, toPersonId,
                                                   newLetter.Identity, string.Empty);
        
  		Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Letter #" + newLetter.Identity.ToString() + " was successfully stored.');", true);
        // Clear the text fields

        this.TextFrom.Text = string.Empty;
        this.TextAddress.Text = string.Empty;
        this.ComboRecipient.Text = string.Empty;
        this.DatePicker.SelectedDate = DateTime.Today;
        this.TemporaryDocumentIdentity.Text = "0";
        this.DropPersonal.SelectedIndex = 0;
        this.DocumentList.Documents = new Documents();
    }


    protected void Validator_DocumentList_Custom_ServerValidate(object source, ServerValidateEventArgs args)
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


    protected void Validator_DatePicker_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (this.DatePicker.SelectedDate == null)
        {
            args.IsValid = false;
            return;
        }

        if (this.DatePicker.SelectedDate > DateTime.Today)
        {
            args.IsValid = false;
            return;
        }

        args.IsValid = true;
    }


    protected void Validator_ComboRecipient_Custom_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = this.ComboRecipient.HasSelection;
    }


    protected void ComboRecipient_SelectedPersonChanged (object source, EventArgs args)
    {
        bool enableRoleSelection = this.ComboRecipient.HasSelection;

        this.LabelRoleAs.Visible = enableRoleSelection;
        this.DropRoles.Visible = enableRoleSelection;

        if (!enableRoleSelection)
        {
            return;
        }

        this.DropRoles.Items.Clear();

        Person selectedPerson = this.ComboRecipient.SelectedPerson;

        Authority authority = selectedPerson.GetAuthority();

        this.DropRoles.Items.Add(new ListItem("-- Select role --", "0"));
        this.DropRoles.Items.Add(new ListItem("Private individual", ((int) RoleType.PrivateIndividual).ToString()));

        Dictionary<RoleType, bool> dupeCheck = new Dictionary<RoleType, bool>();

        // Move this to some sort of localization.

        Dictionary<RoleType, string> roleNames = new Dictionary<RoleType, string>();

        roleNames[RoleType.SystemAdmin] = "System Administrator";
        roleNames[RoleType.OrganizationVice1] = "Org Vice Chairman";
        roleNames[RoleType.OrganizationVice2] = "Org Vice Chairman";
        roleNames[RoleType.LocalDeputy] = "Local Vice Leader";
        roleNames[RoleType.LocalLead] = "Local Leader";
        roleNames[RoleType.OrganizationTreasurer] = "Org Treasurer";
        roleNames[RoleType.OrganizationSecretary] = "Org Secretary";
        roleNames[RoleType.OrganizationAdmin] = "Org Admin";
        roleNames[RoleType.OrganizationAuditor] = "Org Auditor";
        roleNames[RoleType.OrganizationElectedRepresentative] = "Elected Representative";
        roleNames[RoleType.OrganizationChairman] = "Org Chairman";

        foreach (BasicPersonRole role in authority.AllPersonRoles)
        {
            if (!dupeCheck.ContainsKey(role.Type) && roleNames.ContainsKey(role.Type))
            {
                this.DropRoles.Items.Add(new ListItem(roleNames [role.Type], ((int) role.Type).ToString()));
                dupeCheck[role.Type] = true;
            }
        }

        this.LabelRoleAs.Text = "in " + (selectedPerson.Gender == PersonGender.Male ? "his" : "her") + " role as";

    }
}
