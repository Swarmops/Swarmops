using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Interface.Objects;
using Activizr.Logic.Special.Sweden;
using System.Globalization;
using Activizr.Logic.Structure;

public partial class PhpIncludes_BallotCoverage : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {

        OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);

        GeographyBallotCoverageLookup lookup = GetLookupFromCache();

        Response.ContentType = "text/plain";
        if (Request["typ"] == null || Request["typ"] == "F")
            Response.Write(lookup[30].WAdvanceVotingCoverage.ToString("##.#", new CultureInfo(Organization.FromIdentity(metadata.OrganizationId).DefaultCountry.Culture)));
        else
            Response.Write(lookup[30].WVotingCoverage.ToString("##.#", new CultureInfo(Organization.FromIdentity(metadata.OrganizationId).DefaultCountry.Culture)));

    }

    private GeographyBallotCoverageLookup GetLookupFromCache ()
    {
        string key = "BallotCoverage-SE";
        GeographyBallotCoverageData data = null;
        lock (GeographyBallotCoverageData.cacheLocker)
        {
            data = (GeographyBallotCoverageData)(Cache.Get(key));

            if (data == null)
            {
                data = GeographyBallotCoverageData.UpdateBallotDistroData();
                Cache.Insert(key, data, null, DateTime.Now.ToUniversalTime().AddMinutes(15), Cache.NoSlidingExpiration);
            }
        }

        return data.ToLookup();
    }

}
