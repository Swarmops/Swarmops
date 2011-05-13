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

using Activizr.Interface.Objects;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr;


public partial class Charts_MemberCountHistory : System.Web.UI.Page
{



    protected void Page_Load (object sender, EventArgs e)
    {
        // Get organization metadata.

        OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);
        Organization org = Organization.FromIdentity(metadata.OrganizationId);

        // Set the title.
        Chart.Title = "Member count history - " + org.NameShort;

        // Change the shading mode
        Chart.ShadingEffectMode = ShadingEffectMode.Three;

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

        string geoID = Request.QueryString["Geo"];
        string geoValidate = Request.QueryString["Validate"];
        Geography geo = null;
        int geoIntId = -1;
        if (geoID != null || geoValidate != null)
        {
            //want to filter by geography.

            if (geoID == null || geoValidate == null)
            {
                //bad request
                Response.Write("Sorry, that request was not correctly formed");
                Response.End();
            }

            if (geoValidate != MD5.Hash(geoID + "Pirate").Replace(" ", ""))
            {
                //bad request
                Response.Write("Sorry, that request was not correctly formed.");
                Response.End();
            }

            if (!int.TryParse(geoID, out geoIntId))
            {
                //bad request
                Response.Write("Sorry, that request was not correctly formed..");
                Response.End();
            }


            geo = Geography.FromIdentity(geoIntId);

            Chart.Title += " - " + geo.Name;
        }


        // Set the y axis label
        //Chart.ChartArea.YAxis.Label.Text = "Ung Pirat SE - medlemsgraf";

        // Set the directory where the images will be stored.
        Chart.TempDirectory = "temp";

        // Set the chart size.
        Chart.Width = 600;
        Chart.Height = 350;

        // If different sizes were given as parameters, adjust.

        string xSize = Request.QueryString["XSize"];
        string ySize = Request.QueryString["YSize"];
        string format = Request.QueryString["ImageFormat"];

        if (!String.IsNullOrEmpty(xSize))
        {
            Chart.Width = Int32.Parse(xSize);
        }

        if (!String.IsNullOrEmpty(ySize))
        {
            Chart.Height = Int32.Parse(ySize);
        }

        if (!String.IsNullOrEmpty(format))
        {
            Chart.ImageFormat = (ImageFormat)Enum.Parse(typeof(ImageFormat), format);
        }


        Chart.SeriesCollection.Add(GetGrowthData(metadata, geo));

        Chart.LegendBox.Position = LegendBoxPosition.None;
        Chart.Debug = false;
        Chart.Mentor = false;

    }



    SeriesCollection GetGrowthData (OrganizationMetadata metadata, Geography geo)
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

        Organizations organizations = null;
        Dictionary<int, bool> lookup = new Dictionary<int, bool>();

        if (metadata.Recursive)
        {
            organizations = Organization.FromIdentity(metadata.OrganizationId).GetTree();
        }
        else
        {
            organizations = Organizations.FromSingle(Organization.FromIdentity(metadata.OrganizationId));
        }

        foreach (Organization org in organizations)
        {
            lookup[org.Identity] = true;
        }

        Dictionary<int, bool> geoDict = null;
        if (geo != null)
        {
            geoDict = new Dictionary<int, bool>();
            Geographies tree = geo.GetTree();

            foreach (Geography g in tree)
            {
                geoDict[g.GeographyId] = true;
            }
        }

        SeriesCollection collection = new SeriesCollection();
        DateTime dateIterator = new DateTime(2006, 1, 1);

        DateTime today = DateTime.Now.Date;
        DateTime endDate = today;
        string endDateString = Request.QueryString["EndDate"];

        if (!String.IsNullOrEmpty(endDateString))
        {
            endDate = DateTime.Parse(endDateString).AddDays(1);
        }

        string dayCount = Request.QueryString["Days"];

        if (dayCount != null)
        {
            dateIterator = endDate.AddDays(-Int32.Parse(dayCount));
        }

        Series series = new Series();
        series.Name = "";
        int eventIndex = 0;
        int currentCount = 0;

        Dictionary<int, int> personMembershipCountLookup = new Dictionary<int, int>();

        while (dateIterator < endDate)
        {
            DateTime nextDate = dateIterator.AddDays(1);
            while (eventIndex < events.Count && events[eventIndex].DateTime < nextDate)
            {
                // This is fucking problematic because some people can be members of more than one org,
                // so the relatively simple op becomes complicated one of a sudden when we have to keep
                // track of that.

                // The logic is horrible compared to just iterating over DeltaCount over time.

                if (lookup.ContainsKey(events[eventIndex].OrganizationId)
                    && (geo == null || geoDict.ContainsKey(events[eventIndex].GeographyId)))
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

                            currentCount++;
                            personMembershipCountLookup[personId] = 1;
                        }
                    }
                    else if (events[eventIndex].DeltaCount < 0 && personMembershipCountLookup.ContainsKey(personId))
                    {
                        // a membership was lost

                        int membershipCountForPerson = personMembershipCountLookup[personId];

                        // in the extreme majority of cases, membershipCountForPerson will be 1, meaning this
                        // is their only and now terminated membership

                        if (membershipCountForPerson == 1)
                        {
                            personMembershipCountLookup.Remove(personId);
                            currentCount--;
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
            newElement.YValue = currentCount;
            series.Elements.Add(newElement);
            dateIterator = nextDate;
        }

        collection.Add(series);

        collection[0].DefaultElement.Color = metadata.Color;

        return collection;

    }

}
