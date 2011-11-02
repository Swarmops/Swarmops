using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Sweden;
using System.Text;
using MySql.Data.MySqlClient;

public partial class Tests_JL_TestForum : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
    }
    protected void Button1_Click (object sender, EventArgs e)
    {
        IForumDatabase db = SwedishForumDatabase.GetDatabase(2, TextBox1.Text);
        int[] accounts = db.GetAccountList();
        StringBuilder sb=new StringBuilder();
        foreach (int i in accounts)
            sb.Append("," + i);
        TextBox2.Text = sb.ToString();
    } 
}
