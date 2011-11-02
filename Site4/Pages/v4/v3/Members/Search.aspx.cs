using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using Activizr.Interface.Localization;
using System.Globalization;
using Activizr.Logic.Structure;


public partial class Pages_Members_Search : PageV4Base
{
    protected void PersonList_onEditCommand (object sender, GridViewCommandEventArgs e)
    {

        switch (e.CommandName)
        {
            case "ViewEdit":
                int index = Convert.ToInt32(e.CommandArgument);
                GridView gv = (GridView)sender;
                int personId = Convert.ToInt32(gv.DataKeys[index].Value);
                Response.Redirect("BasicDetails.aspx?id="+personId);
                break;

        }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        PersonList.ViewEditCommand += PersonList_onEditCommand;
        if (!Page.IsPostBack)
        {
            PersonList.PersonList = new People();
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
            People nameResult = People.FromNamePattern(TextNamePattern.Text);
            People emailResult = People.FromEmailPattern(TextEmailPattern.Text);
            People cityResult = People.FromCityPattern(TextCityPattern.Text);
            People pcResult = People.FromPostalCodePattern(TextPostalCodePattern.Text);

            if (nameResult != null || emailResult != null)
            {
                searchResults = People.LogicalAnd(nameResult, emailResult);
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
            //Activism can't be searched for in itself. Too heavy on the DB.

            searchResults = searchResults.Filter(
                delegate(Person p)
                {
                    if (p.IsActivist)
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
            if (auth.HasPermission(Permission.CanSeeNonMembers,Organization.PPSEid,-1,Authorization.Flag.AnyGeographyExactOrganization) )
                PersonList.ShowStatus = true;

            if (!auth.HasPermission(Permission.CanValidateActivistEmail, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization) || searchResults.Count > 5)
            {
                // SERIOUS SECURITY BREACH here: All chairmen for all organizations could see ALL members.
                // "CanValidateActivistEmail" was set for "OrganizationChairman".

                searchResults = searchResults.GetVisiblePeopleByAuthority(auth);
            }
            else
            {
                PersonList.ShowStatus = true;
            }


            if (!auth.HasPermission(Permission.CanValidateActivistEmail, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization) || searchResults.Count > 1)
            {
                // SERIOUS SECURITY BREACH here: All chairmen for all organizations could see ALL members.
                // "CanValidateActivistEmail" was set for "OrganizationChairman".

                searchResults = searchResults.RemoveUnlisted();
            }
            PersonList.PersonList = searchResults;
        }
    }
}