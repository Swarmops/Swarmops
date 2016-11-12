using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Tests_JL_TestForum4 : System.Web.UI.Page
{
    protected int batchsize = 199;

    protected void Page_Load (object sender, EventArgs e)
    {
        Server.ScriptTimeout = 300;

        Label3.Text = "";
        Label4.Text = "";

        if (Label1.Text == "" && Label2.Text == "")
        {
            Label1.Text = "1";
            Label2.Text = "0";
            Session["Memberships"] = Memberships.ForOrganization(Organization.PPSE);
            Session["memberAccountLookup"] = new Dictionary<int, bool>();
            Button1.Text = "Press OK to converting Person forumaccountIDs to new forum";
        }
        else if (Label1.Text == "1")
        {
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
            Dictionary<int, bool> memberAccountLookup = Session["memberAccountLookup"] as Dictionary<int, bool>;
            Memberships memberships = Session["Memberships"] as Memberships;
            int startRec = int.Parse(Label2.Text);

            SetMembers(startRec, forumDatabase, memberAccountLookup, memberships);
            Button1.Text = "continuing setting memberships";
        }

        else if (Label1.Text == "2")
        {
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
            Session["accountIds"] = forumDatabase.GetAccountList();

            Label1.Text = "3";
            Label2.Text = "0";
            Button1.Text = "removing memberships";

        }
        else if (Label1.Text == "3")
        {
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
            Dictionary<int, bool> memberAccountLookup = Session["memberAccountLookup"] as Dictionary<int, bool>;
            int[] accountIds = Session["accountIds"] as int[];

            int startRec = int.Parse(Label2.Text);

            RemoveMembers(startRec, forumDatabase, memberAccountLookup, accountIds);

            Button1.Text = "removing memberships";
        }

        if (Label1.Text == "4")
        {

            Button1.Text = "DONE!";
        }

        if (Label1.Text != "4" && Label1.Text != "" && Label3.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "reloadscript", "document.getElementById('" + Button1.ClientID + "').click();", true);
        }


    }

    protected void Button1_Click (object sender, EventArgs e)
    {


    }

    private void RemoveMembers (int startRecord, IForumDatabase forumDatabase, Dictionary<int, bool> memberAccountLookup, int[] accountIds)
    {
        int recCnt = -1;
        int recCnt2 = 0;

        foreach (int accountId in accountIds)
        {
            recCnt++;
            if (recCnt < startRecord) continue;
            if (!memberAccountLookup.ContainsKey(accountId))
            {
                recCnt2++;
                forumDatabase.SetPartyNonmember(accountId);
            }
            if (recCnt2 > batchsize)
            {
                Label2.Text = "" + (recCnt + 1);
                return;
            }
        }
        Label2.Text = "0";
        Label1.Text = "4";

    }
    private void SetMembers (int startRecord, IForumDatabase forumDatabase, Dictionary<int, bool> memberAccountLookup, Memberships memberships)
    {
        Person currentMember = null;
        int recCnt = -1;
        int recCnt2 = 0;
        foreach (Membership membership in memberships)
        {
            recCnt++;
            if (recCnt < startRecord) continue;

            if (!membership.Active)
            {
                continue;
            }
            recCnt2++;
            currentMember = membership.Person;
            SetOneMember(forumDatabase, memberAccountLookup, ref currentMember);
            if (recCnt2 > batchsize)
            {
                Label2.Text = "" + (recCnt + 1);
                return;
            }
        }
        Label2.Text = "0";
        Label1.Text = "2";

    }
    private void SetOneMember (IForumDatabase forumDatabase, Dictionary<int, bool> memberAccountLookup, ref Person currentMember)
    {
        try
        {
            if (currentMember.SwedishForumAccountName != "")
            {
                int currentNewId = forumDatabase.GetAccountId(currentMember.SwedishForumAccountName);
                if (currentNewId > 0)
                {
                    forumDatabase.SetPartyMember(currentNewId);

                    currentMember.SwedishForumAccountId = currentNewId;

                    memberAccountLookup[currentNewId] = true;
                    Label4.Text += "<br>" + currentMember.SwedishForumAccountName + ":" + currentNewId;
                }
            }
        }
        catch (Exception e)
        {
            Label3.Text += "<br>" + currentMember.SwedishForumAccountName + ":" + e.Message;

        }
    }
}
