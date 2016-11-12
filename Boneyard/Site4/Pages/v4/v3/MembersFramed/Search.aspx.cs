using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using Activizr.Logic.Structure;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Interface.Localization;
using System.Globalization;


public partial class Pages_Members_Search : PageV4Base
{

    protected void Page_Load (object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PersonList.PersonList = new People();
            // Populate list of organizations (initial population)

            DropOrganizations.LoadTree(Organization.Root);
            //DropGeographies.LoadTree(Geography.Root);


            Organizations organizationList = _authority.GetOrganizations(RoleTypes.AllRoleTypes);
            organizationList = organizationList.RemoveRedundant();
            DropOrganizations.Content.pruneToSubtree(organizationList.Identities);
            DropOrganizations.Content.expandLevels(true, 2);
            DropOrganizations.SetValue("-1", "- any -");
            DropGeographies.Content.expandSubnode("" + _currentUser.Country.GeographyId, true, 2);
            DropGeographies.SetValue("-1", "- any -");

        }
        PersonList.JsEditFunction = "LoadMember"; // Load subframe via javascript
        PersonList.ViewEditCommand += PersonList_onEditCommand;
    }

    protected void PersonList_onEditCommand (object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "ViewEdit":
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;
                int personId = Convert.ToInt32(gv.DataKeys[index].Value);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LoadMember", "parent.LoadMember('" + personId + "');", true);
                break;

        }
    }

    protected void ButtonSearch_Click (object sender, EventArgs e)
    {
        // If a member number is present, use only that as search criteria.

        People searchResults = null;

        string searchMemberNumber = this.TextMemberNumber.Text.Trim();

        if (searchMemberNumber.Length > 0)
        {
            // Lots of things here that will throw on invalid arguments
            Regex re = new Regex(@"[\s,;]+");
            string[] numbers = re.Replace(searchMemberNumber, ";").Split(';');
            searchResults = new People();
            foreach (string pers in numbers)
            {
                try
                {
                    string persID = pers;
                    if (searchMemberNumber.Trim().ToUpper().StartsWith("PP"))
                    {
                        //Hexcoded number
                        char[] mnr = persID.Substring(2).ToCharArray();
                        Array.Reverse(mnr);
                        persID = new string(mnr);
                        int num = (System.Int32.Parse(searchMemberNumber, System.Globalization.NumberStyles.AllowHexSpecifier));
                        persID = num.ToString();
                    }
                    int personId = Convert.ToInt32(persID);
                    Person person = Person.FromIdentity(personId);
                    searchResults.Add(person);
                }
                catch (Exception)
                { }
            }

            if (numbers.Length > 1)
            {
                PersonList.GridView.PageSize = numbers.Length;
                PersonList.ShowStreet = true;
            }
        }
        else
        {
            searchResults = null;
            try
            {
                if (DropOrganizations.SelectedValue == "-1")
                {
                    if (DropGeographies.SelectedValue != "-1")
                        searchResults = People.FromGeography(Convert.ToInt32(DropGeographies.SelectedValue));
                    else
                        searchResults = null;
                }
                else
                    searchResults = People.FromOrganizationAndGeography(Convert.ToInt32(DropOrganizations.SelectedValue),
                                                                        Convert.ToInt32(DropGeographies.SelectedValue));
            }
            catch { }

            People nameResult = People.FromNamePattern(TextNamePattern.Text);
            People emailResult = People.FromEmailPattern(TextEmailPattern.Text);
            People cityResult = People.FromCityPattern(TextCityPattern.Text);
            People pcResult = People.FromPostalCodePattern(TextPostalCodePattern.Text);

            if (nameResult != null)
            {
                searchResults = People.LogicalAnd(searchResults, nameResult);
            }
            if (searchResults != null || emailResult != null)
            {
                searchResults = People.LogicalAnd(searchResults, emailResult);
            }
            if (searchResults != null || cityResult != null)
            {
                searchResults = People.LogicalAnd(searchResults, cityResult);
            }
            if (searchResults != null || pcResult != null)
            {
                searchResults = People.LogicalAnd(searchResults, pcResult);
            }

            string inputDateText = textPersonalNumber.Text.Trim();
            if (inputDateText != "")
            {
                int inpLen = inputDateText.Length;
                int yLen = 2;

                string[] formats2 = new string[] { "yy", "yyMM", "yyMMdd", "yyyy", "yyyyMM", "yyyyMMdd", "yyyy", "yyyy-MM", "yyyy-MM-dd", "yy", "yy-MM", "yy-MM-dd" };
                string[] formats4 = new string[] { "yyyy", "yyyyMM", "yyyyMMdd", "yy", "yyMM", "yyMMdd", "yyyy", "yyyy-MM", "yyyy-MM-dd", "yy", "yy-MM", "yy-MM-dd" };
                string[] formats;

                DateTime parsedDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;

                if (inputDateText.StartsWith("19")
                    || inputDateText.StartsWith("20"))
                {
                    formats = formats4;
                    yLen = 4;
                }
                else
                    formats = formats2;
                string[] datesSplit = (textPersonalNumber.Text + "--").Split(new string[] { "--" }, StringSplitOptions.None);

                DateTime.TryParseExact(datesSplit[0].Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
                if (parsedDate != DateTime.MinValue)
                {
                    if (datesSplit[1] != "")
                    {
                        DateTime.TryParseExact(datesSplit[1].Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate);
                        inpLen = datesSplit[1].Trim().Length;
                        bool dash = textPersonalNumber.Text.Contains("-");
                        if (inpLen == yLen)
                        {
                            endDate = endDate.AddYears(1);
                        }
                        else if (inpLen == yLen + 2 || (dash && inpLen == yLen + 3))
                        {
                            endDate = endDate.AddMonths(1);
                        }
                        else
                            endDate = endDate.AddDays(1);
                    }
                    else
                    {
                        inpLen = datesSplit[0].Trim().Length;
                        bool dash = textPersonalNumber.Text.Contains("-");
                        if (inpLen == yLen)
                        {
                            endDate = parsedDate.AddYears(1);
                        }
                        else if (inpLen == yLen + 2 || (dash && inpLen == yLen + 3))
                        {
                            endDate = parsedDate.AddMonths(1);
                        }
                        else
                            endDate = parsedDate.AddDays(1);
                    }

                    People dateResults = People.FromBirtDatePattern(parsedDate, endDate);
                    if (searchResults != null || dateResults != null)
                    {
                        searchResults = People.LogicalAnd(searchResults, dateResults);
                    }
                }
            }
        }

        if (searchResults != null && CheckBoxActivist.Checked)
        {
            Geography geoTree = Geography.FromIdentity(Convert.ToInt32(DropGeographies.SelectedValue));

            //Activism can't be searched for in itself. Too heavy on the DB.
            Activists activists = Activists.FromGeography(Geography.FromIdentity(Convert.ToInt32(DropGeographies.SelectedValue)));
            int[] activistIds = activists.Identities;
            Dictionary<int, bool> activistsHashed = new Dictionary<int, bool>();
            foreach (int id in activistIds)
                activistsHashed[id] = true;

            searchResults = searchResults.Filter(
                delegate(Person p)
                {
                    if (activistsHashed.ContainsKey(p.Identity))
                        return true;
                    else
                        return false;
                });
        }


        if (searchResults != null)
        {
            PersonList.ShowStatus = false;

            Authority auth = _authority;

            //TODO: All this supposes that these permissions can be applied to mon swedish people as well, ie Organization.PPSEid is supposed
            if (!auth.HasPermission(Permission.CanSeeNonMembers, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization) )
                PersonList.ShowStatus = true;

            if (!auth.HasPermission(Permission.CanValidateActivistEmail, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization) 
                || searchResults.Count > 5)
            {
                // SERIOUS SECURITY BREACH here: All chairmen for all organizations could see ALL members.
                // "CanValidateActivistEmail" was set for "OrganizationChairman".

                searchResults = searchResults.GetVisiblePeopleByAuthority(auth);
            }
            else
            {
                PersonList.ShowStatus = true;
            }


            if (!auth.HasPermission(Permission.CanValidateActivistEmail, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization) 
                    || searchResults.Count > 1)
            {
                // SERIOUS SECURITY BREACH here: All chairmen for all organizations could see ALL members.
                // "CanValidateActivistEmail" was set for "OrganizationChairman".

                searchResults = searchResults.RemoveUnlisted();
            }
            PersonList.PersonList = searchResults;
        }
    }

    protected void PopulateGeographies (Organization org)
    {

        this.DropGeographies.LoadTree(Geography.Root);

        Geographies geoList = _authority.GetGeographiesForOrganization(org);

        geoList = geoList.RemoveRedundant();
        //geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));

        List<int> permitted = new List<int>();

        foreach (Geography nodeRoot in geoList)
        {
            Geographies nodeTree = nodeRoot.GetTree();

            foreach (Geography node in nodeTree)
            {

                if (_authority.HasPermission(Permission.CanSeePeople, org.Identity, node.Identity, Authorization.Flag.Default))
                    permitted.Add(node.GeographyId);
            }
        }

        this.DropGeographies.Content.pruneToValues(permitted.ToArray());
        if (permitted.Count > 0)
        {
            this.DropGeographies.Content.expandSubnode("" + permitted[0], true, 2);
            this.DropGeographies.SelectedValue = "" + permitted[0];
        }

        RecalculatePersonCount();
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        if (DropOrganizations.SelectedValue != "0")
        {
            Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));

            PopulateGeographies(org);
        }
        else
        {
            this.DropGeographies.LoadTree(Geography.Root);

            this.DropGeographies.SetValue("-1", "- any -");
        }
    }




    protected void RecalculatePersonCount ()
    {
        this.ButtonSearch.Text = "Search ";
        try
        {
            Organization selectedOrg = null;
            Organizations orgTree = null;
            Geography selectedGeo = null;
            Geographies geoTree = null;

            if (this.DropOrganizations.SelectedValue != "0")
            {
                selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
            }
            else
            {
                selectedOrg = Organization.Root;
            }
            orgTree = selectedOrg.GetTree();

            selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));
            geoTree = selectedGeo.GetTree();

            int memberCount = 0;

            memberCount = orgTree.GetMemberCountForGeographies(geoTree);

            this.ButtonSearch.Text = "Search " + memberCount.ToString("#,##0") + " people";
            this.ButtonSearch.Enabled = (memberCount > 0 ? true : false);

        }
        catch { }
    }

    protected void DropGeographies_SelectedIndexChanged (object sender, EventArgs e)
    {
        RecalculatePersonCount();
    }


}