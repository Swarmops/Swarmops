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
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Charts_GenderDistributionHistory : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// Set the title.
		Chart.Title = "";

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;

        Chart.YAxis.Scale = Scale.FullStacked;

		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

        Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

        string dayCountString = Request.QueryString["Days"];

        if (dayCountString != null)
        {
            int dayCount = Int32.Parse(dayCountString);

            if (dayCount < 32)
            {
                Chart.XAxis.TimeScaleLabels.DayFormatString = "dd";
                Chart.XAxis.TimeScaleLabels.MonthFormatString = "dd";
                Chart.XAxis.TimeInterval = TimeInterval.Day;
            }
            else if (dayCount < 180)
            {
                Chart.XAxis.TimeInterval = TimeInterval.Month;
            }
            
        }

		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "Ung Pirat SE - medlemsgraf";

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

    	Chart.SeriesCollection.Add(GetDistributionData(Organizations.FromSingle (Organization.FromIdentity (1))));

		Chart.LegendBox.Position = LegendBoxPosition.None;
       
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetDistributionData(Organizations orgs)
	{
        string cacheDataKey = "ChartData-AllMembershipEvents";

        MembershipEvents events = (MembershipEvents)Cache.Get(cacheDataKey);

        if (events == null)
        {
            events = MembershipEvents.LoadAll();
            Cache.Insert(cacheDataKey, events, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		/*
		using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Data/MembershipEvents.xml")))
		{
			string xml = reader.ReadToEnd();

			events = MembershipEvents.FromXml(xml);
		}*/

		Dictionary<int, bool> lookup = new Dictionary<int,bool>();

		foreach (Organization org in orgs)
		{
			lookup[org.Identity] = true;
		}


		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = new DateTime(2006, 1, 1);

		string dayCount = Request.QueryString["Days"];

        if (dayCount != null)
		{
            dateIterator = DateTime.Now.Date.AddDays(-Int32.Parse(dayCount));
		}

        Series seriesMale = new Series();
        Series seriesFemale = new Series();
        seriesMale.Name = "";
        seriesFemale.Name = "";
        DateTime today = DateTime.Now.Date;
		int eventIndex = 0;

        Dictionary<PersonGender, int> genderCount = new Dictionary<PersonGender, int>();
        genderCount[PersonGender.Male] = 0;
        genderCount[PersonGender.Female] = 0;

        Dictionary<int, int> personMembershipCountLookup = new Dictionary<int, int>();

		while (dateIterator < today)
		{
			DateTime nextDate = dateIterator.AddDays (1);
			while (eventIndex < events.Count && events[eventIndex].DateTime < nextDate)
			{
                // This is fucking problematic because some people can be members of more than one org,
                // so the relatively simple op becomes complicated one of a sudden when we have to keep
                // track of that.

                // The logic is horrible compared to just iterating over DeltaCount over time.

				if (lookup.ContainsKey (events[eventIndex].OrganizationId))
				{
                    int personId = events[eventIndex].PersonId;

                    if (events[eventIndex].DeltaCount > 0)
                    {
                        // A membership was added.

                        // Was this person already a member?

                        if (personMembershipCountLookup.ContainsKey(personId))
                        {
                            // yes, increment that person's membership count, not the people count

                            personMembershipCountLookup[personId]++;
                        }
                        else
                        {
                            // no, create the key, increment the people count and set this person's membership count to 1

                            genderCount[events [eventIndex].Gender]++;
                            personMembershipCountLookup[personId] = 1;
                        }
                    }
                    else if (events [eventIndex].DeltaCount < 0)
                    {
                        // a membership was lost

                        int membershipCountForPerson = 1;

                        try
                        {
                            membershipCountForPerson = personMembershipCountLookup[personId];
                        }
                        catch (Exception)
                        {
                            // some corrupt data in db, so need to handle key not present
                        }

                        // in the extreme majority of cases, membershipCountForPerson will be 1, meaning this
                        // is their only and now terminated membership

                        if (membershipCountForPerson == 1)
                        {
                            personMembershipCountLookup.Remove(personId);
                            genderCount[events[eventIndex].Gender]--;
                        }
                        else
                        {
                            // but this person had more than one, decrement their membership count but not the total

                            personMembershipCountLookup[personId]--;
                        }
                    }

                    // no case for when DeltaCount is 0, it can't be at the time of this writing,
                    // but who knows how PirateWeb will expand and grow

                    // assumes DeltaCount is always 1 or -1
				}

				eventIndex++;
			}

			Element newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = genderCount [PersonGender.Male];
			seriesMale.Elements.Add(newElement);

            newElement = new Element();
            newElement.XDateTime = dateIterator;
            newElement.YValue = genderCount[PersonGender.Female];
            seriesFemale.Elements.Add(newElement);

			dateIterator = nextDate;
		}

        seriesMale.DefaultElement.Transparency = 98;
        seriesMale.DefaultElement.Color = Color.Blue;
        seriesFemale.DefaultElement.Color = Color.Red;

		collection.Add(seriesFemale);
        collection.Add(seriesMale);

		return collection;

	}

}
