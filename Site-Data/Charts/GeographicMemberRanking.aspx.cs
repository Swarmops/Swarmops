using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;

using Activizr.Logic.Structure;

public partial class Pages_Public_Charts_GeographicMemberRanking : System.Web.UI.Page
{
	protected void Page_Load(Object sender, EventArgs e)
	{
		// Set the title.
		Chart.Title = "Geografisk medlemsranking";

		string orgIdString = Request.QueryString["OrgId"];

		if (orgIdString == null || orgIdString.Length == 0)
		{
			orgIdString = "1";
		}

		Chart.Title += " - " + Organization.FromIdentity(Int32.Parse(orgIdString)).Name;

		string label = Request.QueryString["Label"];

		if (label != null)
		{
			Chart.Title += " - " + label;
		}

		if (Request.QueryString["MaxEntries"] != null)
		{
			Chart.Title += ", topp " + Request.QueryString["MaxEntries"];
		}

		// Set the chart Type
		Chart.Type = ChartType.ComboHorizontal;

		// Turn 3D off.
		Chart.Use3D = false;

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;

		// Set a default transparency
		Chart.DefaultSeries.DefaultElement.Transparency = 20;

		// Set the x axis scale
		Chart.ChartArea.XAxis.Scale = Scale.Stacked;

		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "Medlemsk\xE5r i promille av v\xE4ljarna";

		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "";


		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 550;

		if (Request.QueryString["Height"] != null)
		{
			Chart.Height = Int32.Parse(Request.QueryString["Height"]);
		}

		// Add the random data.
		Chart.SeriesCollection.Add(GetRankingData());

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;
	}

	bool CanZoom(int nodeId)
	{
		if (nodeId > 40)
		{
			return false;
		}

		if (nodeId == 9 || nodeId == 11 || nodeId == 16 || nodeId == 40)
		{
			return false;
		}

		return true;
	}

	Series GetRankingData()
	{
		string geoString = Request.QueryString["GeoIds"];

		if (geoString == null || geoString.Length == 0)
		{
			//geoString = "10,17,24,9,25,16,15,27,6,8,7,11,18,29,20,14,13,12,2,38,40,4,19,3,21,28,26,23,22,5";
			geoString = "10-285-286-287-288-289,17-103-104-105-106-107-108-109-110-111-112-113-114-115-116,24-212-50-49-213-214-215-168-216-217-218-219-220-221-222-223,9,25-178-179-180-181-182-183-184-185-186-187,16,15-323-44-325-46-327-45,27-229-230-231-232-233-234-235-236,6-290-291-292-293-294-295-296-297-298-299-300-301-302,8-311-312-313-314-315-316-317-318-319-320-321-322,7-303-304-305-306-307-308-309-310,11,18-117-118-119-120-121-122-123-124-125-126-127-128-129,29-200-201-202-203-204-205-206-207-54-208-53-209-210-211,20-136-137-138-139-140-141-142-143-144-145-146-147-148-149-150,14-90-91-92-93-94-95-96-97-98-99-100-101-102,13-79-80-81-82-83-84-85-86-87-88-89,12-71-72-73-74-75-76-77-78,2-153-154-57-155-58-159-59-60-160-61-163-162-164-165-167,38-62-63-64-65-156-157-158-66-161-166,40,4-47-171-172-170-48-174-175-176-177,19-130-131-132-133-134-135,3-277-279-278-280-281-282-283-284,21-329-330-331-332-333-334-335-336-337-338-339-340-341-342-343-344,28-237-238-239-240-241-242-243-51-244-245-52-246-247-248-249,26-250-251-252-56-253-254-55,23-188-189-190-191-192-193-194-195-196-199,22-255-256-257-258-259-260-261-262-263-264-265-266,5-267-268-269-42-270-43-41-271-272-273-274-275-276";
		}

		string orgIdString = Request.QueryString["OrgId"];

		if (orgIdString == null || orgIdString.Length == 0)
		{
			orgIdString = "1";
		}

		int orgId = Int32.Parse(orgIdString);

		string[] geoIdStrings = geoString.Split(',');

		SeriesCollection collection = new SeriesCollection();

        GeographyStatistics stats = (GeographyStatistics)Cache.Get("GeographyStatistics");

        if (stats == null)
        {
            stats = GeographyStatistics.GeneratePresent(new int[] { 1, 2 });
            Cache.Insert("GeographyStatistics", stats, null, DateTime.Today.ToUniversalTime().AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		// Gather the data

		GeographyBar[] bars = new GeographyBar[geoIdStrings.Length];

		for (int index = 0; index < geoIdStrings.Length; index++)
		{
			string geoIdString = geoIdStrings[index];
			string[] subIdStrings = geoIdString.Split('-');
			
			int geographyId = Int32.Parse (subIdStrings [0]);

			bars [index] = new GeographyBar();
			bars [index].Geography = Geography.FromIdentity (geographyId);
			bars [index].Value = (double) (stats [geographyId].OrganizationData [orgId-1].MemberCount * 1000.0) / (double) stats [geographyId].VoterCount;

			if (subIdStrings.Length > 1)
			{
				bars[index].DrillDownData = String.Join(",", subIdStrings, 1, subIdStrings.Length - 1);
			}
		}

		Array.Sort(bars);

		int maxCount = 0;

		if (Request.QueryString["MaxEntries"] != null)
		{
			maxCount = Int32.Parse (Request.QueryString ["MaxEntries"]);
		}

		// Populate the data

		Series series = new Series();
		series.Name = "No particular name";
		int count = 0;

		foreach (GeographyBar bar in bars)
		{
			Element element = new Element();
			element.YValue = bar.Value;
			element.Name = bar.Geography.Name;
			element.SmartLabel = new SmartLabel();
			element.SmartLabel.Text = bar.Value.ToString ("0.00");

			if (bar.DrillDownData != null)
			{
				element.URL = "GeographicMemberRanking.aspx?GeoIds=" + (string)bar.DrillDownData + "&OrgId=" + orgIdString + "&Label=" + Server.UrlEncode(Geography.FromIdentity (bar.Geography.Identity).Name);
			}

			series.Elements.Add(element);

			count++;
		}

		// Removing the overflow entries afterwards is a bit backwards, but it's due to the sorting

		if (maxCount > 0)
		{
			while (count > maxCount)
			{
				series.Elements.RemoveAt(0);
				count--;
			}
		}

		series.DefaultElement.Color = System.Drawing.Color.DarkViolet;
		series.DefaultElement.ShowValue = true;

		return series;
	}


}
