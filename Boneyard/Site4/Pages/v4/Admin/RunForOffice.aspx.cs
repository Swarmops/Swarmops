using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Governance;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Admin_RunForOffice : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextCandidacy.Style[HtmlTextWriterStyle.Width] = "400px";
        this.TextEditCandidacy.Style[HtmlTextWriterStyle.Width] = "400px";

        if (!Page.IsPostBack)
        {
            MeetingElection poll = MeetingElection.Primaries2010;

            MeetingElectionCandidates candidates = MeetingElection.Primaries2010.Candidates;
            bool thisUserIsCandidate = false;
            MeetingElectionCandidate thisCandidate = null;

            foreach (MeetingElectionCandidate candidate in candidates)
            {
                if (candidate.PersonId == _currentUser.Identity)
                {
                    thisUserIsCandidate = true;
                    thisCandidate = candidate;
                    break;
                }
            }

            if (!poll.RunningOpen)
            {
                this.PanelCallForCandidatesIntro.Visible = false;
                this.PanelCallForCandidatesClosed.Visible = true;
            }
            if (!_currentUser.MemberOf (Organization.PPSE))
            {
                this.PanelNotEligible.Visible = true;
                this.PanelCandidacy.Visible = false;
                this.PanelCandidate.Visible = false;
            }
            else if (thisUserIsCandidate)
            {
                this.PanelCandidate.Visible = true;
                this.PanelCandidacy.Visible = false;
                this.PanelEditCandidacy.Visible = true;
                this.TextEditCandidacy.Text = thisCandidate.CandidacyStatement;
            }

            this.TextPersonalNumber.Text = _currentUser.PersonalNumber;
            this.TextPhotographer.Text = _currentUser.PortraitPhotographer;
            this.TextBlogName.Text = _currentUser.BlogName;

            if (String.IsNullOrEmpty(_currentUser.BlogUrl))
            {
                this.TextBlogUrl.Text = "http://";
            }
            else
            {
                this.TextBlogUrl.Text = _currentUser.BlogUrl;
            }

            string tShirtSize = _currentUser.TShirtSize;

            if (!String.IsNullOrEmpty(tShirtSize))
            {
                this.DropTShirtSizes.SelectedValue = tShirtSize;
            }
        }
    }


    protected void ButtonUpload_Click(object sender, EventArgs e)
    {
        ProcessUpload();
    }

    private void ProcessUpload()
    {
        string serverPath = @"C:\Data\Uploads\PirateWeb"; // TODO: Read from web.config

        foreach (UploadedFile file in this.UploadPhoto.UploadedFiles)
        {
            string clientFileName = file.GetName();
            string extension = file.GetExtension();

            // Delete old picture, if any

            Documents oldPictures = Documents.ForObject(_currentUser);
            foreach (Document oldPicture in oldPictures)
            {
                oldPicture.Delete();
            }

            Document newDocument =
                Document.Create(Guid.NewGuid().ToString() + ".tmp", clientFileName,
                                file.ContentLength, string.Empty, _currentUser, _currentUser);

            string serverFileName = String.Format("document_{0:D5}_personphoto_{1:D6}{2}", newDocument.Identity,
                                                  _currentUser.Identity, file.GetExtension().ToLower());
            file.SaveAs(serverPath + Path.DirectorySeparatorChar + serverFileName);

            newDocument.ServerFileName = serverFileName;

            File.Delete(serverPath + Path.DirectorySeparatorChar + file.GetName());
        }
    }


    protected void ValidatorPhotoRequired_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (_currentUser.Portrait != null)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }


    protected void ValidatorChecksRequired_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = this.CheckUnderstand1.Checked && this.CheckUnderstand2.Checked;
    }

    protected void ValidatorPhotoOkRequired_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = this.CheckPhotoOk.Checked;
    }

    protected void ValidatorCandidacyLength_ServerValidate(object source, ServerValidateEventArgs args)
    {
        int length = this.TextCandidacy.Text.Length;

        if (length > 300)
        {
            this.ValidatorCandidacyLength.Text = String.Format("Texten är för lång! ({0} tecken, max 300.)", length);
            args.IsValid = false;
        }
        else
        {
            args.IsValid = true;
        }
    }


    protected void ValidatorEditCandidacyLength_ServerValidate(object source, ServerValidateEventArgs args)
    {
        int length = this.TextEditCandidacy.Text.Length;

        if (length > 300)
        {
            this.ValidatorEditCandidacyLength.Text = String.Format("Texten är för lång! ({0} tecken, max 300.)", length);
            args.IsValid = false;
        }
        else
        {
            args.IsValid = true;
        }
    }


    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
            return;
        }

        _currentUser.PortraitPhotographer = this.TextPhotographer.Text;
        _currentUser.PersonalNumber = this.TextPersonalNumber.Text;
        _currentUser.TShirtSize = this.DropTShirtSizes.SelectedValue;
        _currentUser.BlogName = this.TextBlogName.Text;
        _currentUser.BlogUrl = this.TextBlogUrl.Text;

        MeetingElectionCandidate.Create(MeetingElection.Primaries2010, _currentUser, this.TextCandidacy.Text);

        this.PanelCandidacy.Visible = false;
        this.PanelCandidate.Visible = true;
        this.PanelCallForCandidatesIntro.Visible = false;
        this.PanelEditCandidacy.Visible = true;
    }

    protected void ButtonSaveChangedCandidacy_Click (object sender, EventArgs e)
    {
        if (Page.IsValid)
        {

            MeetingElectionCandidate candidate = MeetingElectionCandidate.FromPersonAndPoll(_currentUser,
                                                                                      MeetingElection.Primaries2010);

            candidate.CandidacyStatement = this.TextEditCandidacy.Text;
        }
    }
}
