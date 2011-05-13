using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Special.Sweden;
using System.Web.UI.HtmlControls;
using Activizr.Logic.Structure;

public partial class Data_BallotsData : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        GeographyBallotCoverageLookup lookup = GetLookupFromCache();

        HtmlTableRow tr = null;
        HtmlTableCell td = null;
        tr = new HtmlTableRow(); mainTable.Rows.Add(tr);
        td = CreateCell(tr, "Geography");

        td = CreateCell(tr, "GeographyId");
        td = CreateCell(tr, "VoterCount");
        td = CreateCell(tr, "AdvanceVotingStationsDistro");
        td = CreateCell(tr, "AdvanceVotingStationsTotal");
        td = CreateCell(tr, "VotingStationsTotal");
        td = CreateCell(tr, "VotingStationsDistroSingle");
        td = CreateCell(tr, "VotingStationsDistroDouble");
        td = CreateCell(tr, "VotingStationsComplete");
        td = CreateCell(tr, "WAdvanceVotingStationsDistro");
        td = CreateCell(tr, "WAdvanceVotingStationsTotal");
        td = CreateCell(tr, "WWAdvanceVotingStationsDistro");
        td = CreateCell(tr, "WWAdvanceVotingStationsTotal");
        td = CreateCell(tr, "WVotingStationsTotal");
        td = CreateCell(tr, "WVotingStationsDistroSingle");
        td = CreateCell(tr, "WVotingStationsDistroDouble");
        td = CreateCell(tr, "WVotingStationsComplete");
        
        foreach (int geoId in lookup.Keys)
        {
            tr = new HtmlTableRow(); mainTable.Rows.Add(tr);
            GeographyBallotCoverageDataPoint pt = lookup[geoId];

            td = CreateCell(tr, "" + Geography.FromIdentity((pt.GeographyId)).Name);

            td = CreateCell(tr, "" + pt.GeographyId);
            td = CreateCell(tr, "" + pt.VoterCount);
            td = CreateCell(tr, "" + pt.AdvanceVotingStationsDistro);
            td = CreateCell(tr, "" + pt.AdvanceVotingStationsTotal);
            td = CreateCell(tr, "" + pt.VotingStationsTotal);
            td = CreateCell(tr, "" + pt.VotingStationsDistroSingle);
            td = CreateCell(tr, "" + pt.VotingStationsDistroDouble);
            td = CreateCell(tr, "" + pt.VotingStationsComplete);
            td = CreateCell(tr, "" + pt.WAdvanceVotingStationsDistro);
            td = CreateCell(tr, "" + pt.WAdvanceVotingStationsTotal);
            td = CreateCell(tr, "" + pt.WWAdvanceVotingStationsDistro);
            td = CreateCell(tr, "" + pt.WWAdvanceVotingStationsTotal  );
            td = CreateCell(tr, "" + pt.WVotingStationsTotal);
            td = CreateCell(tr, "" + pt.WVotingStationsDistroSingle);
            td = CreateCell(tr, "" + pt.WVotingStationsDistroDouble);
            td = CreateCell(tr, "" + pt.WVotingStationsComplete);

        }


    }

    private static HtmlTableCell CreateCell (HtmlTableRow tr, string txt)
    {
        HtmlTableCell td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerHtml = txt;
        return td;
    }


    private GeographyBallotCoverageLookup GetLookupFromCache ()
    {
        string key = "BallotCoverage-SE";
        GeographyBallotCoverageData data = null;
        lock (GeographyBallotCoverageData.cacheLocker)
        {
            data = (GeographyBallotCoverageData)(Cache.Get(key));

            if (data == null || "" + Request["reload"] != "")
            {
                data = GeographyBallotCoverageData.UpdateBallotDistroData();
                Response.Write("Reloading");
                Cache.Insert(key, data, null, DateTime.Now.ToUniversalTime().AddMinutes(15), Cache.NoSlidingExpiration);
            }
        }

        return data.ToLookup();
    }
}
