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
using Activizr.Logic.Structure;
using Activizr.Logic.Tasks;
using Roles=Activizr.Logic.Pirates.Roles;

public partial class Controls_v4_ListLocalContacts : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Geography geography = this.person.Geography;
        Person lead = null;

        if (!person.MemberOf(Organization.PPSE))
        {
            // PPSE only atm

            return;
        }

        while (lead == null)
        {
            try
            {
                lead = Roles.GetLocalLead(Organization.PPSE, geography);
            }
            catch (ArgumentException)
            {
                geography = geography.Parent;
            }
        }

        People deputies = Roles.GetLocalDeputies(Organization.PPSE, geography);

        string literal =
            "<span style=\"line-height:150%\">";

        literal += FormatPerson("lead", lead) + " (" + Server.HtmlEncode(geography.Name) + ")<br/>";

        foreach (Person deputy in deputies)
        {
            literal += FormatPerson("deputy", deputy);
        }

        literal += "</span>";
        /*
        literal +=
            "<br/><img src=\"/Images/Public/Fugue/icons-shadowless/pwcustom/orglevel-2-male.png\" style=\"position:relative;top:3px\" />&nbsp;&nbsp;" +
            "<a href=\"mailto:marten.fjallstrom@piratpartiet.se\">Party Secretary</a>, <a href=\"callto:+46-10-3333-404\">010-3333-404</a>";*/

        this.LiteralContacts.Text = literal;
    }

    private string FormatPerson (string iconPart, Person person)
    {
        string result = string.Empty;

        result = "<img src=\"/Images/Public/Fugue/icons-shadowless/pwcustom/officer-" + iconPart + "-" +
                 (person.IsFemale ? "female" : "male") + ".png\" style=\"position:relative;top:3px\" />&nbsp;&nbsp;" +
                 "<a href=\"mailto:" + Server.HtmlEncode(person.PartyEmail) + "\">" + Server.HtmlEncode(person.Name) + "</a>, <a href=\"callto:" + Server.HtmlEncode(person.Phone) + "\">" + Server.HtmlEncode(person.Phone) + "</a>";

        return result;
    }

    public Person Person
    {
        get { return this.person; }
        set { this.person = value; }
    }

    private Person person;

}
