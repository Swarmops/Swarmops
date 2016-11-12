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

public partial class Pages_Public_Data_MemberCountHistory : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.PickDateTime.MaxDate = DateTime.Now;
            this.PickDateTime.SelectedDate = DateTime.Now;
            this.PickDateTime.FocusedDate = DateTime.Now;
            this.PickDateTime.DateInput.Culture = new System.Globalization.CultureInfo("sv-SE");
            this.PickEndDate.DateInput.Culture = new System.Globalization.CultureInfo("sv-SE");
            this.PickDateTime.TimeView.Culture = new System.Globalization.CultureInfo("sv-SE");
            this.LabelCountResults.Text = Organization.FromIdentity(Organization.PPSEid).GetMemberCount().ToString();
            this.LabelStartDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            this.TextHeight.Text = "5";
            this.TextWidth.Text = "7";
        }

        this.TextHeight.Style.Add(HtmlTextWriterStyle.Width, "40px");
        this.TextWidth.Style.Add(HtmlTextWriterStyle.Width, "40px");
        this.DropUnit.Style.Add(HtmlTextWriterStyle.Width, "70px");
    }


    protected int GetMemberCount(Organization org, bool recursive, Geography geo, DateTime dateTime)
    {
        string cacheDataKey = "ChartData-AllMembershipEvents-5min";

        MembershipEvents events = (MembershipEvents)Cache.Get(cacheDataKey);

        if (events == null)
        {
            events = MembershipEvents.LoadAll();
            Cache.Insert(cacheDataKey, events, null, DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        DateTime endDateTime = dateTime;

        int eventIndex = 0;
        int currentCount = 0;

        while (eventIndex < events.Count && events[eventIndex].DateTime < endDateTime)
        {
            if (events[eventIndex].OrganizationId == Organization.PPSEid)
            {
                currentCount += events[eventIndex].DeltaCount;
            }

            eventIndex++;
        }

        return currentCount;
    }

    protected void ButtonGetCount_Click(object sender, EventArgs e)
    {
        this.LabelCountResults.Text = this.GetMemberCount(Organization.FromIdentity(Organization.PPSEid), false, Geography.Root, (DateTime)this.PickDateTime.SelectedDate).ToString();
        this.LabelStartDate.Text = ((DateTime)this.PickDateTime.SelectedDate).Date.ToString ("yyyy-MM-dd");
        this.PickEndDate.SelectedDate = this.PickDateTime.SelectedDate.Value.AddDays(30);
    }

    protected void ButtonGenerateGraphUrl_Click(object sender, EventArgs e)
    {
        this.PanelGraphUrl.Visible = true;

        double xSize = Double.Parse(this.TextWidth.Text.Replace(",", "."));
        double ySize = Double.Parse(this.TextHeight.Text.Replace(",", "."));

        if (this.DropUnit.SelectedValue == "cm")
        {
            xSize /= 2.54;
            ySize /= 2.54;
        }

        xSize *= 300; // dpi
        ySize *= 300;

        string url = String.Format("http://data.piratpartiet.se/Charts/MemberCountHistory.aspx?EndDate={0}&Days={1}&XSize={2}&YSize={3}",
            this.PickEndDate.SelectedDate.Value.ToString("yyyy-MM-dd"), (this.PickEndDate.SelectedDate.Value.Date - this.PickDateTime.SelectedDate.Value.Date).Days, (int)xSize, (int)ySize);

        this.LinkGraph.NavigateUrl = url;
        this.LinkGraph.Text = url;
        this.LabelCountResults.Text = this.GetMemberCount(Organization.FromIdentity(Organization.PPSEid), false, Geography.Root, (DateTime)this.PickDateTime.SelectedDate).ToString();
    }

}
