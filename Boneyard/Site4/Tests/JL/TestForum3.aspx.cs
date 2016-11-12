using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Tests_JL_TestForum3 : System.Web.UI.Page
{
    protected int batchsize = 100;

    protected void Page_Load (object sender, EventArgs e)
    {
        Server.ScriptTimeout = 300;

        if (Label1.Text == "" && Label2.Text == "")
        {
            Label1.Text = "1";
            Label2.Text = "0";
            Session["Memberships"] = Memberships.ForOrganization(Organization.PPSE);
            Session["memberAccountLookup"] = new Dictionary<int, bool>();
            Button1.Text = "Press OK to start";
        }
        else if (Label1.Text == "1")
        {
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
            Dictionary<int, bool> memberAccountLookup = Session["memberAccountLookup"] as Dictionary<int, bool>;
            Memberships memberships = Session["Memberships"] as Memberships;
            int startRec = int.Parse(Label2.Text);

            SetMembers(startRec, forumDatabase, memberAccountLookup, memberships);
            Button1.Text = "Press OK to continue setting memberships";
        }

        else if (Label1.Text == "2")
        {
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
            Session["accountIds"] = forumDatabase.GetAccountList();

            Label1.Text = "3";
            Label2.Text = "0";
            Button1.Text = "Press OK to Start removing memberships";

        }
        else if (Label1.Text == "3")
        {
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
            Dictionary<int, bool> memberAccountLookup = Session["memberAccountLookup"] as Dictionary<int, bool>;
            int[] accountIds = Session["accountIds"] as int[];

            int startRec = int.Parse(Label2.Text);

            RemoveMembers(startRec, forumDatabase, memberAccountLookup, accountIds);

            Button1.Text = "Press OK to continue removing memberships";
        }
        else if (Label1.Text == "4")
        {

            Button1.Text = "DONE!";
        }

        if (Label1.Text != "4" && Label1.Text != "")
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
            if (currentMember.Handle != "")
            {
                int currentNewId = forumDatabase.GetAccountId(currentMember.Handle);
                if (currentNewId > 0)
                {
                    //currentMember.SwedishForumAccountId = currentNewId;
                    forumDatabase.SetPartyMember(currentNewId);

                    memberAccountLookup[currentNewId] = true;
                }
            }
        }
        catch (Exception)
        {
            // The forum account probably doesn't exist. Just remove it from the profile.

            try
            {
                //currentMember.SwedishForumAccountId = 0;
            }
            catch (Exception)
            {

            }
        }
    }
}
