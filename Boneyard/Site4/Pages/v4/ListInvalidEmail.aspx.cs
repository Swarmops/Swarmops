using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Media;
using Activizr.Logic.Pirates;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Logic.Support;

public partial class Pages_v4_ListInvalidEmail : PageV4Base
{

    static readonly object cacheLockObject = new object();
    static readonly string cacheDataKey = "ListInvalidEmail-BadEmails";

    class DisplayPerson : Person
    {
        private string actionResult;
        private string member;
        public string Member
        {
            get
            {
                return member;
            }
            set
            {
                member = value;
            }
        }
        public string ActionResult
        {
            get
            {
                return actionResult;
            }
            set
            {
                actionResult = value;
            }
        }
        public DisplayPerson (Person p)
            : base(p)
        {
        }
    }

    class DisplayPersons : Dictionary<int, DisplayPerson>
    { }
    DateTime stopTime;

    protected void Page_Load (object sender, EventArgs e)
    {
        stopTime = DateTime.Now.AddSeconds(Server.ScriptTimeout / 3);
        LoadGrid();
    }


    private void LoadGrid ()
    {
        DisplayPersons badAdresses = GetBadAdresses();
        if (""+ViewState["isLoading"] != "1")
        {
            gridPanel.Visible = true;
            continueLoadPanel.Visible = false;
            this.GridPeople.DataSource = badAdresses.Values;
            this.GridPeople.DataBind();
        }
        else
        {
            gridPanel.Visible = false;
            continueLoadPanel.Visible = true;
            btnContinue.Text = "Found " + badAdresses.Count.ToString() + " Continues to load automatically. please wait...";
            ScriptManager.RegisterStartupScript(this,this.GetType(),"Clickagain","document.getElementById('"+btnContinue.ClientID+"').click();",true);
        }
    }

    private DisplayPersons GetBadAdresses ()
    {
        lock (cacheLockObject)
        {
            DisplayPersons badAdresses = Cache.Get(cacheDataKey) as DisplayPersons;

            if (badAdresses == null || ""+ViewState["isLoading"] == "1")
            {
                int lastread = int.Parse("0" + ViewState["lastread"]);
                if (""+ViewState["isLoading"] != "1")
                    lastread = 0;

                People people = Cache.Get(cacheDataKey+"People"+_currentUser.Identity ) as People;
                if (people == null)
                {
                    people = People.GetAll();
                    people =Authorization.FilterPeopleToMatchAuthority(people, _authority);
                    Cache.Insert(cacheDataKey+"People"+_currentUser.Identity, people, null, DateTime.Now.AddMinutes(5).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                if (badAdresses == null)
                    badAdresses = new DisplayPersons();

                ViewState["isLoading"] = "0";

                foreach (Person p in people)
                {
                    if (p.Identity > lastread)
                    {
                        if ((!Formatting.ValidateEmailFormat(p.Email) || p.EMailIsInvalid || p.MailUnreachable) && ((p.MailUnreachable == false && p.Email != "") || CheckBoxUnreachable.Checked) && p.Email != "raderat")
                        {
                            DisplayPerson dp = new DisplayPerson(p);
                            dp.Member = "";
                            Memberships memberships = p.GetMemberships();
                            foreach (Membership membership in memberships)
                            {
                                if (membership.Active)
                                {
                                    dp.Member += membership.Organization.Name + " ";
                                }
                            }
                            if (CheckBoxNonMembers.Checked || dp.Member != "")
                                badAdresses.Add(p.Identity, dp);
                            if (dp.Member == "")
                                dp.Member = "No";
                        }
                        lastread = p.Identity;
                    }
                    if (DateTime.Now > stopTime)
                    {
                        ViewState["isLoading"] = "1";
                        break;
                    }
                }
                ViewState["lastread"] = lastread.ToString();

                Cache.Insert(cacheDataKey, badAdresses, null, DateTime.Now.AddMinutes(5).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return badAdresses;
        }
    }


    protected void GridPeople_ItemDataBound (object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem gridDataItem = e.Item as GridDataItem;
            DisplayPerson person = e.Item.DataItem as DisplayPerson;
            if (person == null)
            {
                return;
            }
            gridDataItem["IsInvalid"].Text = (person.EMailIsInvalid ? "Yes" : "");
            gridDataItem["IsUnreachable"].Text = (person.MailUnreachable ? "Yes" : "");

        }
    }

    protected void RadAjaxManager1_AjaxRequest (object sender, AjaxRequestEventArgs e)
    {
        // TODO: There is a problem with re-getting the query parameters here -- the user 
        // may have changed the data in the web form, which will repopulate the grid 
        // with different data when the popup closes. This would be extremely confusing 
        // to the user. Is there  a good way to invisibly cache the query base 
        // (account, start date, end date)?

        /*
        if (e.Argument == "Rebind")
        {
            this.GridTransactions.MasterTableView.SortExpressions.Clear();
            this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid(accountId, (DateTime) this.DateStart.SelectedDate, (DateTime) this.DateEnd.SelectedDate);
            this.GridTransactions.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            this.GridTransactions.MasterTableView.SortExpressions.Clear();
            this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            this.GridTransactions.MasterTableView.CurrentPageIndex = this.GridTransactions.MasterTableView.PageCount - 1;
            PopulateGrid(accountId, (DateTime) this.DateStart.SelectedDate, (DateTime) this.DateEnd.SelectedDate);
            this.GridTransactions.Rebind();
        }*/
    }


    protected void GridPeople_ItemCommand (object source, GridCommandEventArgs e)
    {

    }
    protected void ButtonSetUnreachable_Click (object sender, EventArgs e)
    {
        lock (cacheLockObject)
        {
            DisplayPersons badAdresses = GetBadAdresses();
            foreach (GridDataItem item in this.GridPeople.MasterTableView.Items)
            {
                int personId = (int)item.GetDataKeyValue("Identity");
                CheckBox box = (CheckBox)item["SendSMS"].FindControl("cbSendSMS");
                if (box.Checked)
                {
                    if (badAdresses.ContainsKey(personId))
                    {
                        DisplayPerson person = badAdresses[personId];

                        bool hasActiveMemberships = person.Member != "No";

                        if (hasActiveMemberships)
                        {
                            // Attempt to contact by SMS.
                            if (person.Phone.Trim().Length > 2)
                            {
                                try
                                {
                                    person.SendPhoneMessage("Piratpartiet: den mailadress vi har till dig (" + person.Email + ") studsar. Kontakta medlemsservice@piratpartiet.se med ny adress.");
                                    person.ActionResult = "SMS skickat";
                                }
                                catch (Exception)
                                {
                                    person.ActionResult = "Det gick inte att skicka SMS";
                                }
                            }
                        }
                        else
                        {
                            person.ActionResult = "Har inget aktivt medlemsskap";
                        }
                        person.MailUnreachable = true;
                    }
                }
                box = (CheckBox)item["MarkUnreachable"].FindControl("cbMarkUnreachable");
                if (box.Checked)
                {
                    if (badAdresses.ContainsKey(personId))
                    {
                        badAdresses[personId].MailUnreachable = true;
                    }
                }

            }
            Cache.Insert(cacheDataKey, badAdresses, null, DateTime.Now.AddMinutes(5).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
            this.GridPeople.DataSource = badAdresses.Values;
            this.GridPeople.DataBind();
        }

    }

    protected void ButtonButtonRefresh_Click (object sender, EventArgs e)
    {
        ViewState["isLoading"] = "0";
        Cache.Remove(cacheDataKey);
        LoadGrid();
    }
    protected void btnContinue_Click (object sender, EventArgs e)
    {
        LoadGrid();

    }

}