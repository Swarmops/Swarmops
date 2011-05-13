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
using Activizr.Interface.Objects;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

using Membership=Activizr.Logic.Pirates.Membership;


public partial class Pages_Public_Charts_MembersAgesGenders : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        // Get organization metadata.

        OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);
        Organization org = Organization.FromIdentity(metadata.OrganizationId);

		// Set the title.
		Chart.Title = "Member birthyears and genders - " + org.NameShort;

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

        string cacheDataKey = cacheDataKeyBase + "-" + metadata.OrganizationId.ToString() + "-" + metadata.Recursive.ToString();

        SeriesCollection data = (SeriesCollection) Cache.Get(cacheDataKey);

        if (data == null)
        {
            data = (SeriesCollection) GetAgeGenderData(metadata);
            Cache.Insert(cacheDataKey, data, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		Chart.SeriesCollection.Add(data);

		Chart.LegendBox.Position = LegendBoxPosition.Top;
		Chart.Debug = false;
		//Chart.Mentor = false;

        // Chart.CacheDuration = 5;  // CANNOT cache charts that generate differently depending on URL! CANNOT!
	}



	SeriesCollection GetAgeGenderData(OrganizationMetadata metadata)
	{
		int orgId = metadata.OrganizationId;
		bool recurseTree = metadata.Recursive;

		Series seriesMale = new Series();
		Series seriesFemale = new Series();
		seriesMale.Name = "Male";
		seriesFemale.Name = "Female";

		Memberships memberships = null;
		
		if (recurseTree)
		{
			memberships = Memberships.ForOrganizations(Organization.FromIdentity(orgId).GetTree());
		}
		else
		{
			memberships = Memberships.ForOrganization (Organization.FromIdentity (orgId));
		}

		BasicPerson[] allPeople = PirateDb.GetDatabase().GetAllPeople();

		Dictionary<int, int> geoLookup = new Dictionary<int, int>();
		Dictionary<int, PersonGender> genderLookup = new Dictionary<int, PersonGender>();
		Dictionary<int, int> birthYearLookup = new Dictionary<int, int>();
        Dictionary<int, bool> personLookup = new Dictionary<int, bool>();

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

            if (personLookup.ContainsKey(membership.PersonId))
            {
                continue; // If a person was already counted, do not count again
            }

			if (genderLookup.ContainsKey(membership.PersonId)
                && (membership.OrganizationId == orgId
                    || (recurseTree && membership.Organization.Inherits(orgId))))
			{
				birthYear = birthYearLookup[membership.PersonId];
				gender = genderLookup[membership.PersonId];

                int index = birthYear - 1900;

                if (index < 30 || index >= 100)
                {
                    index = 90; // Put invalid years on 1990, where it won't show up in the noise
                }

				if (gender == PersonGender.Male)
				{
					male[index]++;
				}
				else
				{
					female[index]++;
				}

                personLookup[membership.PersonId] = true;
			}
		}

		Element newElement = new Element();

		for (int yearIndex = 30; yearIndex < 100; yearIndex++)
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
