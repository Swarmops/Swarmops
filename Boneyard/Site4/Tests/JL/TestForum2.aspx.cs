using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Pirates;

public partial class Tests_JL_TestForum2 : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {

    }
    protected void Button1_Click (object sender, EventArgs e)
    {
        IForumDatabase db = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
        int pid = int.Parse(TextBox2.Text);
        Person p = Person.FromIdentity(pid);
        int acc = db.GetAccountId(p.Handle);
        Label2.Text = p.Handle +":"+acc.ToString();
        db.SetPartyMember(acc);
        Label1.Text = db.IsPartyMember(acc).ToString();

    }
    protected void Button2_Click (object sender, EventArgs e)
    {
        IForumDatabase db = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
        int pid = int.Parse(TextBox2.Text);
        Person p = Person.FromIdentity(pid);
        int acc = db.GetAccountId(p.Handle);
        Label2.Text = p.Handle + ":" + acc.ToString();
        db.SetPartyNonmember(acc);
        Label1.Text = db.IsPartyMember(acc).ToString();

    }
}
