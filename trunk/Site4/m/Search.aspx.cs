using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Interface.Localization;


public partial class m_Pages_Members_Search : System.Web.UI.Page
{
    private class RememberSearchParams
    {
        public RememberSearchParams ()
        {
        }
        public RememberSearchParams (string init)
            : this()
        {
            string[] par = (init+"||||||").Split(new char[] { '|' });
            Name = par[0];
            Mail = par[1];
            IdNumber = par[2];
            Birth = par[3];
        }
        public string Name;
        public string Mail;
        public string IdNumber;
        public string Birth;
        public override string ToString ()
        {
            return Name + "|" + Mail + "|" + IdNumber + "|" + Birth;
        }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Request.Cookies["mSearchParams"] != null)
            {
                CheckBoxRemember.Checked = true;

                RememberSearchParams r = new RememberSearchParams(Request.Cookies["mSearchParams"].Value);
                //r.Birth=this.textPersonalNumber.Text.Trim();
                this.TextMemberNumber.Text = r.IdNumber;
                this.TextEmailPattern.Text = r.Mail;
                this.TextNamePattern.Text = r.Name;
            }

            //this.LabelBirthDate.Text = LocalizationManager.GetLocalString("PirateWeb.Pages.Members.Search.BirthDate","Birthdate");
            this.labelEmailPattern.Text =
                LocalizationManager.GetLocalString("PirateWeb.Pages.Members.Search.EmailPattern", "E-mail");
            this.labelMemberNumber.Text =
                LocalizationManager.GetLocalString("PirateWeb.Pages.Members.Search.MemberNumber", "Membernumber");
            this.labelNamePattern.Text = LocalizationManager.GetLocalString(
                "PirateWeb.Pages.Members.Search.NamePattern", "Name");
            this.labelSearchHints.Text = LocalizationManager.GetLocalString(
                "PirateWeb.Pages.Members.Search.SearchHints",
                "If you don't get any hits, try filling in less fields or using partial searchwords, like just someones last name.");

            PersonList.PersonList = new People();
        }
    }

    protected void ButtonSearch_Click (object sender, EventArgs e)
    {
        Response.Cookies.Clear();
        if (CheckBoxRemember.Checked)
        {
            RememberSearchParams r = new RememberSearchParams();
            //r.Birth=this.textPersonalNumber.Text.Trim();
            r.IdNumber = this.TextMemberNumber.Text.Trim();
            r.Mail = this.TextEmailPattern.Text;
            r.Name = this.TextNamePattern.Text;
            Response.Cookies["mSearchParams"].Value = r.ToString();
            Response.Cookies["mSearchParams"].Expires = DateTime.Now.AddMonths(1);
            Response.Cookies["mListParams"].Path = "/m";
        }


        ExecuteSearch();
    }

    private void ExecuteSearch ()
    {
        // If a member number is present, use only that as search criteria.

        People searchResults = null;

        string searchMemberNumber = this.TextMemberNumber.Text.Trim();

        if (searchMemberNumber.Length > 0)
        {
            // Lots of things here that will throw on invalid arguments

            int personId = Convert.ToInt32(searchMemberNumber);
            Person person = Person.FromIdentity(personId);
            searchResults = People.FromSingle(person);
        }
        else
        {
            People nameResult = People.FromNamePattern(TextNamePattern.Text);
            People emailResult = People.FromEmailPattern(TextEmailPattern.Text);

            if (nameResult != null || emailResult != null)
            {
                searchResults = People.LogicalAnd(nameResult, emailResult);
            }

            // TODO: Filter on birth date, if set
        }

        if (searchResults != null)
        {
            searchResults =
                searchResults.GetVisiblePeopleByAuthority(
                    Authorization.GetPersonAuthority(Int32.Parse(HttpContext.Current.User.Identity.Name)));
            searchResults = searchResults.RemoveUnlisted();

            PersonList.PersonList = searchResults;
        }
    }
}