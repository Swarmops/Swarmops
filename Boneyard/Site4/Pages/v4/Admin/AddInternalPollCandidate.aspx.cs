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
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Admin_AddInternalPollCandidate : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextCandidacy.Style[HtmlTextWriterStyle.Width] = "400px";
        this.PersonCandidate.Authority = _authority;

        if (!Page.IsPostBack)
        {
            int pollId = Int32.Parse(Request.QueryString["PollId"]);

            MeetingElection poll = MeetingElection.FromIdentity(pollId);

            if (_currentUser.Identity != 1 && _currentUser.Identity != poll.CreatedByPersonId)
            {
                throw new UnauthorizedAccessException("Access Denied");
            }


            this.LabelPollName1.Text = poll.Name;
            this.LabelPollName2.Text = poll.Name;
            this.LabelPollName3.Text = poll.Name;

        }
    }


    protected void PersonCandidate_SelectedPersonChanged (object sender, EventArgs e)
    {
        Person candidate = this.PersonCandidate.SelectedPerson; 
        int pollId = Int32.Parse(Request.QueryString["PollId"]);
        MeetingElection poll = MeetingElection.FromIdentity(pollId);


        this.ImageCandidatePhoto.ImageUrl="http://data.piratpartiet.se/Handlers/DisplayPortrait.aspx?YSize=300&PersonId=" + candidate.Identity.ToString();

        this.LabelCandidateName1.Text =
            this.LabelCandidateName2.Text =
            this.LabelCandidateName3.Text = candidate.Canonical;

        // Already running?

        MeetingElectionCandidates candidates = poll.Candidates;
        bool alreadyCandidate = false;

        foreach (MeetingElectionCandidate testCandidate in candidates)
        {
            if (candidate.PersonId == testCandidate.PersonId)
            {
                alreadyCandidate = true;
                break;
            }
        }

        this.PanelCandidacy.Visible = !alreadyCandidate;
        this.PanelAlreadyCandidate.Visible = alreadyCandidate;

        if (!alreadyCandidate)
        {
            this.TextBlogUrl.Text = candidate.BlogUrl;
            this.TextBlogName.Text = candidate.BlogName;
            this.TextPhotographer.Text = candidate.PortraitPhotographer;
            this.TextPersonalNumber.Text = candidate.PersonalNumber;
        }
    }



    protected void ButtonUpload_Click(object sender, EventArgs e)
    {
        ProcessUpload();
    }

    private void ProcessUpload()
    {
        string serverPath = @"C:\Data\Uploads\PirateWeb"; // TODO: Read from web.config

        Person candidate = this.PersonCandidate.SelectedPerson;

        foreach (UploadedFile file in this.UploadPhoto.UploadedFiles)
        {
            string clientFileName = file.GetName();
            string extension = file.GetExtension();

            // Delete old picture, if any

            Documents oldPictures = Documents.ForObject(this.PersonCandidate.SelectedPerson);
            foreach (Document oldPicture in oldPictures)
            {
                oldPicture.Delete();
            }

            Document newDocument =
                Document.Create(Guid.NewGuid().ToString() + ".tmp", clientFileName,
                                file.ContentLength, string.Empty, candidate, _currentUser);

            string serverFileName = String.Format("document_{0:D5}_personphoto_{1:D6}{2}", newDocument.Identity,
                                                  candidate.Identity, file.GetExtension().ToLower());
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


    protected void ValidatorPhotoOkRequired_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = this.CheckPhotoOk.Checked;
    }

    protected void ValidatorCandidacyLength_ServerValidate(object source, ServerValidateEventArgs args)
    {
        int length = this.TextCandidacy.Text.Length;

        if (length > 2048)
        {
            // this.ValidatorCandidacyLength.Text = String.Format("Texten är för lång! ({0} tecken, max 2048.)", length);
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

        Person candidate = this.PersonCandidate.SelectedPerson;

        int pollId = Int32.Parse(Request.QueryString["PollId"]);
        MeetingElection poll = MeetingElection.FromIdentity(pollId);
        
        if (_currentUser.Identity != 1 && _currentUser.Identity != poll.CreatedByPersonId)
        {
            throw new UnauthorizedAccessException("Access Denied");
        }


        candidate.PortraitPhotographer = this.TextPhotographer.Text;
        candidate.PersonalNumber = this.TextPersonalNumber.Text;
        candidate.BlogName = this.TextBlogName.Text;
        candidate.BlogUrl = this.TextBlogUrl.Text;

        poll.AddCandidate(candidate, this.TextCandidacy.Text);

        this.PersonCandidate.SelectedPerson = null;
        this.TextPhotographer.Text = string.Empty;
        this.TextPersonalNumber.Text = string.Empty;
        this.TextCandidacy.Text = string.Empty;
        this.TextBlogName.Text = string.Empty;
        this.TextBlogUrl.Text = string.Empty;
        this.ImageCandidatePhoto.ImageUrl = string.Empty;

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", "alert ('The candidate \"" + candidate.Canonical.Replace("'", "''") + "\" was added to the poll \"" + poll.Name.Replace("'", "''") + "\".\\r\\nCurrent candidate count: " + poll.Candidates.Count.ToString() + ".');", true);
    }
}
