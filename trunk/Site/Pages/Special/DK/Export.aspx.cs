using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using System.Text;

public partial class Pages_Special_DK_Export : PageV4Base
{
    public class DateSpan
    {
        public DateTime start;
        public DateTime end;
        public DateSpan (DateTime start, DateTime end)
        {
            this.start = start;
            this.end = end;
        }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        string fileName = "PPDK"+DateTime.Now.ToString("yyyyMMdd_HHmm")+".txt";

        if (Request.Browser.Browser == "IE" )
        {

            fileName = Server.UrlPathEncode(fileName);

            if (fileName != null) fileName = fileName.Replace(@"+", @"%20");

        }


        Response.ContentType = "text/plain";
        Response.ContentEncoding = Encoding.UTF8 ;

        //Response.ContentType = "application/octet-stream";

        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\";");

       if (_authority.HasPermission(Permission.CanSeePeople, Organization.PPDKid, Geography.DenmarkId , Authorization.Flag.ExactGeographyExactOrganization))
        {
            StringBuilder sb = new StringBuilder();
            int org = "" + Request["o"] == "UP" ? Organization.UPDKid : Organization.PPDKid;
            People danishPeople = People.FromOrganizationAndGeography(org, Geography.RootIdentity);
            danishPeople = danishPeople.GetVisiblePeopleByAuthority(_authority).RemoveUnlisted();
            
            sb.Append("Identity");
            sb.Append("\tName");
            sb.Append("\tEmail");
            sb.Append("\tStreet");
            sb.Append("\tPostalCode");
            sb.Append("\tCity");
            sb.Append("\tCountry.Name");
            sb.Append("\tPhone");
            sb.Append("\tBirthdate");
            sb.Append("\tGender");
            sb.Append("\tGeography.Name");
            sb.Append("\tpp.start");
            sb.Append("\tpp.end");
            sb.Append("\tup.start");
            sb.Append("\tup.end");
            sb.Append("\r\n");
            foreach (Person p in danishPeople)
            {
                sb.Append(p.Identity);
                sb.Append("\t" + p.Name);
                sb.Append("\t" + p.Email);
                sb.Append("\t" + p.Street);
                sb.Append("\t" + p.PostalCode);
                sb.Append("\t" + p.CityName);
                sb.Append("\t" + p.Country.Name);
                sb.Append("\t" + p.Phone);
                sb.Append("\t" + p.Birthdate);
                sb.Append("\t" + p.Gender);
                sb.Append("\t" + p.Geography.Name);
                Memberships pMemberships = p.GetMemberships();
                DateSpan ppdkspan = null;
                DateSpan updkspan = null;
                foreach (Membership m in pMemberships)
                {
                    if (m.OrganizationId == Organization.PPDKid)
                        ppdkspan = new DateSpan(m.MemberSince, m.Expires);
                    else if (m.OrganizationId == Organization.UPDKid || m.Organization.Inherits(Organization.UPDKid))
                        updkspan = new DateSpan(m.MemberSince, m.Expires);
                }
                if (ppdkspan != null)
                {
                    sb.Append("\t" + ppdkspan.start);
                    sb.Append("\t" + ppdkspan.end);
                }
                else
                    sb.Append("\t\t");

                if (updkspan != null)
                {
                    sb.Append("\t" + updkspan.start);
                    sb.Append("\t" + updkspan.end);
                }
                else
                    sb.Append("\t\t");
                sb.Append("\r\n");
            }

        Response.Write(sb.ToString());
        }
    }
}
