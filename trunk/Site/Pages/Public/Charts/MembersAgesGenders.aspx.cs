using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;

using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

using Membership = Activizr.Logic.Pirates.Membership;


public partial class Pages_Public_Charts_MembersAgesGenders : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        // Set the title.
        Chart.Title = "";

        // Change the shading mode
        Chart.ShadingEffectMode = ShadingEffectMode.Three;

        // Set the x axis label
        Chart.ChartArea.XAxis.Label.Text = "";

        Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

        // Set the y axis label
        Chart.YAxis.Scale = Scale.Stacked;
        // Chart.LegendBox.Template = "%Icon%Name";


        // Set the directory where the images will be stored.
        Chart.TempDirectory = "temp";

        // Set the chart size.
        Chart.Width = 600;
        Chart.Height = 350;

        string orgIdString = Request.QueryString["OrgId"];
        string recurseTreeString = Request.QueryString["RecurseTree"];
        string reloadString = Request.QueryString["Reload"];

        string cacheDataKey = cacheDataKeyBase + "-" + orgIdString + "-" + recurseTreeString;

        SeriesCollection data = (SeriesCollection)Cache.Get(cacheDataKey);

        if (data == null || reloadString == "1")
        {
            data = (SeriesCollection)GetAgeGenderData();
            Cache.Insert(cacheDataKey, data, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        Chart.SeriesCollection.Add(data);

        Chart.LegendBox.Position = LegendBoxPosition.Top;
        Chart.Debug = false;
        //Chart.Mentor = false;

        Chart.CacheDuration = 5;
    }



    SeriesCollection GetAgeGenderData ()
    {
        string orgIdString = Request.QueryString["OrgId"];
        string recurseTreeString = Request.QueryString["RecurseTree"];

        int orgId = Organization.PPSEid;
        bool recurseTree = false;

        if (orgIdString != null)
        {
            orgId = Int32.Parse(orgIdString);
        }

        if (recurseTreeString == "1")
        {
            recurseTree = true;
        }

        Chart.ChartArea.YAxis.Label.Text = "Medlems\xE5ldrar och k\xF6n - " + Organization.FromIdentity(orgId).Name;

        Series seriesMale = new Series();
        Series seriesFemale = new Series();
        seriesMale.Name = "M\xE4n";
        seriesFemale.Name = "Kvinnor";

        Memberships memberships = null;

        if (recurseTree)
        {
            memberships = Memberships.ForOrganizations(Organization.FromIdentity(orgId).GetTree());
        }
        else
        {
            memberships = Memberships.ForOrganization(Organization.FromIdentity(orgId));
        }

        BasicPerson[] allPeople = PirateDb.GetDatabase().GetAllPeople();

        Dictionary<int, int> geoLookup = new Dictionary<int, int>();
        Dictionary<int, PersonGender> genderLookup = new Dictionary<int, PersonGender>();
        Dictionary<int, int> birthYearLookup = new Dictionary<int, int>();
        Dictionary<int, int> personLookup = new Dictionary<int, int>();

        foreach (BasicPerson person in allPeople)
        {
            geoLookup[person.Identity] = person.GeographyId;
            genderLookup[person.Identity] = person.IsMale ? PersonGender.Male : PersonGender.Female;
            birthYearLookup[person.Identity] = person.Birthdate.Year;
        }

        int[] male = new int[200];
        int[] female = new int[200];

        foreach (Membership membership in memberships)
        {
            int birthYear = 0;
            PersonGender gender = PersonGender.Unknown;

            if (genderLookup.ContainsKey(membership.PersonId)
                && !personLookup.ContainsKey(membership.PersonId)
                && (membership.OrganizationId == orgId 
                    || (recurseTree && membership.Organization.Inherits(orgId))))
            {
                personLookup[membership.PersonId] = 1;

                birthYear = birthYearLookup[membership.PersonId];
                gender = genderLookup[membership.PersonId];

                if (birthYear > 1900 && birthYear < (1900 + 200))
                {
                    if (gender == PersonGender.Male)
                    {
                        male[birthYear - 1900]++;
                    }
                    else
                    {
                        female[birthYear - 1900]++;
                    }
                }
            }
        }

        Element newElement = new Element();

        for (int yearIndex = 30; yearIndex <= 100; yearIndex++)
        {
            newElement = new Element();
            newElement.Name = (1900 + yearIndex).ToString();
            newElement.YValue = male[yearIndex];
            seriesMale.Elements.Add(newElement);

            newElement = new Element();
            newElement.Name = (1900 + yearIndex).ToString();
            newElement.YValue = female[yearIndex];
            seriesFemale.Elements.Add(newElement);
        }


        seriesMale.DefaultElement.Color = Color.Blue;
        seriesFemale.DefaultElement.Color = Color.Red;

        SeriesCollection collection = new SeriesCollection();
        collection.Add(seriesFemale);
        collection.Add(seriesMale);

        return collection;
    }

    private string cacheDataKeyBase = "ChartData-Members-Ages-Genders";
}
