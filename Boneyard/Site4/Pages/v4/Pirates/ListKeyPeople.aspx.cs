using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

using Roles=Activizr.Logic.Pirates.Roles;
using Activizr.Logic.Security;


public partial class Pages_v4_ListKeyPeople : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {

    }

    public void GeographyTree_SelectedNodeChanged (object sender, EventArgs e)
    {
        PopulateFacts();
    }

    private void PopulateFacts()
    {
        Organization org = this.SelectOrganizationLine.SelectedOrganization;
        Geography geo = this.GeographyTree.SelectedGeography;

        if (geo == null || org == null)
        {
            return;
        }

        if (org.Identity == 1 && geo.Identity == 1)
        {
            StringBuilder result = new StringBuilder("<b>Organization-level roles:</b><br/>");
            result.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            result.Append(GenerateTableLine("Board Chairman", 1));
            result.Append(GenerateTableLine("Board Vice Chairman", 5));
            result.Append(GenerateTableLine("Board Second Vice Chairman", 11443));
            result.Append(GenerateTableLine("Party Secretary", 967));
            result.Append(GenerateTableLine("Treasurer", 3419));
            result.Append(GenerateTableLine("Board Member", 2613));
            result.Append(GenerateTableLine("Board Member", 3365));
            result.Append(GenerateTableLine("Board Member", 1007));
            result.Append(GenerateTableLine("Board Member", 9561));
            result.Append(GenerateTableLine("Board Member", 437));
            result.Append(GenerateTableLine("Board Deputy", 290));
            result.Append(GenerateTableLine(" ", " ", string.Empty));
            result.Append(GenerateTableLine("Party Leader", 1));
            result.Append(GenerateTableLine("Vice Party Leader", 5));
            result.Append(GenerateTableLine("Second Vice Party Leader", 11443));
            result.Append("</table><br/>");

            result.Append(GenerateAdministrators(org, geo));

            this.LiteralFacts.Text = result.ToString();
        }
        else
        {
            StringBuilder result = new StringBuilder("<b>");
            result.Append(HttpUtility.HtmlEncode(org.NameShort)).Append(" at ");
            result.Append(HttpUtility.HtmlEncode(geo.Name)).Append(":</b><br />");
            result.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");

            RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, org.Identity);

            result.Append(GenerateLeadLine(officers[RoleType.LocalLead], "Local Lead"));
            result.Append(GenerateLeadLine(officers[RoleType.LocalDeputy], "Local Second"));

            result.Append("</table><br />");

            result.Append(GenerateAdministrators(org, geo, officers));

            result.Append("<br/>").Append(GenerateActivists(org, geo));

            this.LiteralFacts.Text = result.ToString();
        }
    }


    private string GenerateLeadLine (List<PersonRole> roles, string label)
    {
        StringBuilder result = new StringBuilder("<tr><td>");
        result.Append(HttpUtility.HtmlEncode(label).Replace(" ", "&nbsp;")).Append("&nbsp;&nbsp;</td><td>");

        if (roles.Count > 0)
        {
            result.Append(GeneratePerson(roles[0].Person));

            for (int index = 1; index < roles.Count; index++)
            {
                result.Append(", ").Append(GeneratePerson(roles[index].Person));
            }
        }
        else
        {
            result.Append("none");
        }

        result.Append("</td></tr>");

        return result.ToString();
    }


    private string GenerateActivists (Organization org, Geography geo)
    {
        People activists = Roles.GetActivists(org, geo);

        StringBuilder result = new StringBuilder("<b>Activists at this level or below:</b><br/>");

        if (activists.Count > 0)
        {
            result.Append(GeneratePerson(activists[0]));

            for (int index = 1; index < activists.Count; index++)
            {
                result.Append(", ").Append(GeneratePerson(activists[index]));
            }
        }
        else
        {
            result.Append("none");
        }

        result.Append("<br/>");
        return result.ToString();
    }


    private string GenerateAdministrators (Organization org, Geography geo)
    {
        RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, org.Identity);

        return GenerateAdministrators(org, geo, officers);
    }


    private string GenerateAdministrators (Organization org, Geography geo, RoleLookup officers)
    {
        List<Person> aggregate = new List<Person>();
        foreach (PersonRole role in officers[RoleType.LocalActive])
        {
            aggregate.Add(role.Person);
        }
        foreach (PersonRole role in officers[RoleType.LocalAdmin])
        {
            aggregate.Add(role.Person);
        }

        StringBuilder result = new StringBuilder("<b>Administrators at this level:</b><br/>");
        if (aggregate.Count > 0)
        {
            result.Append(GeneratePerson(aggregate[0]));

            for (int index = 1; index < aggregate.Count; index++)
            {
                result.Append(", ").Append(GeneratePerson(aggregate[index]));
            }
        }
        else
        {
            result.Append("none");
        }

        result.Append("<br/>");
        return result.ToString();
    }


    private string GeneratePerson (Person person)
    {
        return GeneratePerson(person.Name, "mailto:" + person.PartyEmail);
    }

    private string GeneratePerson (string name, string url)
    {
        return string.Format("<a href='{0}'>{1}</a>", url, HttpUtility.HtmlEncode(name));
    }

    private string GenerateTableLine (string title, string data)
    {
        return GenerateTableLine(title, data, string.Empty);
    }

    private string GenerateTableLine (string title, int personID)
    {
        Person p = Person.FromIdentity(personID);

        return GenerateTableLine(title, p.Name , p.PartyEmail );
    }

    private string GenerateTableLine (string title, string data, string mailaddress)
    {
        return string.Format("<tr><td>{0}&nbsp;&nbsp;</td><td width=\"100%\"><a href=\"{1}\">{2}</a></td></tr>",
            HttpUtility.HtmlEncode(title).Replace(" ", "&nbsp;"),
            "mailto:" + mailaddress,
            HttpUtility.HtmlEncode(data));
    }

    public void SelectOrganizationLine_SelectedNodeChanged (object sender, EventArgs e)
    {
        GeographyTree.Root = SelectOrganizationLine.SelectedOrganization.PrimaryGeography;
        PopulateFacts();
    }

}