using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Pirates;
using Activizr.Logic.Tasks;

public partial class Controls_v4_ListOfficerTodos : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Tasks tasks = Tasks.ForPerson(person);

        bool foundAny = false;

        if (tasks.Count > 0)
        {

            string literal =
                "<span style=\"line-height:150%\">";

            foreach (TaskGroup taskGroup in tasks)
            {
                literal += "<img src=\"" + taskGroup.IconUrl + "\" style=\"position:relative;top:3px\" />&nbsp;<a href=\"" + taskGroup.NavigateUrl + "\">" +
                           Server.HtmlEncode(taskGroup.Description) + "</a><br/>";
            }

            literal += "</span>";

            this.LiteralTodos.Text = literal;
        }
        else
        {
            this.LiteralTodos.Text = "No open tasks.";
        }
    }

    public Person Person
    {
        get { return this.person; }
        set { this.person = value; }
    }

    private Person person;

}
