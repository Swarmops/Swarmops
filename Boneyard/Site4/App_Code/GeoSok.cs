using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Threading;
using HtmlAgilityPack;
using System.Globalization;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for PostNummerSok
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class GeoSok : System.Web.Services.WebService
{
    public class PostenResult
    {
        private string _Street;

        public string Street
        {
            get { return _Street; }
            set { _Street = value; }
        }
        private string _Number;

        public string Number
        {
            get { return _Number; }
            set { _Number = value; }
        }
        private string _Code;

        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        private string _City;

        public string City
        {
            get { return _City; }
            set { _City = value; }
        }
    }

    public class EniroResult
    {
        private string _Street;

        public string Street
        {
            get { return _Street; }
            set { _Street = value; }
        }
        private string _Number;

        public string Number
        {
            get { return _Number; }
            set { _Number = value; }
        }
        private string _Code;

        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        private string _City;

        public string City
        {
            get { return _City; }
            set { _City = value; }
        }        private string _Municipality;

        public string Municipality
        {
            get { return _Municipality; }
            set { _Municipality = value; }
        }
        private double _Lat;

        public double Lat
        {
            get { return _Lat; }
            set { _Lat = value; }
        }
        private double _Lon;

        public double Lon
        {
            get { return _Lon; }
            set { _Lon = value; }
        }

        private double _Dist = 0;

        public double Dist
        {
            get { return _Dist; }
            set { _Dist = value; }
        }

    }

    public class GeoPoint
    {
        public double Lat;
        public double Long;
        public GeoPoint ()
        {
            Lat = 0;
            Long = 0;
        }
        public GeoPoint(double pLat,double pLong)
        {
            Lat = pLat;
            Long = pLong;
        }

    }

    public GeoSok ()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public List<GeoSok.PostenResult> FindByStreetAddressInPosten (string streetAddress, string city)
    {
        List<GeoSok.PostenResult> resList = new List<GeoSok.PostenResult>();
        WebRequest w = WebRequest.Create("http://www.posten.se/soktjanst/postnummersok/resultat.jspv?gatunamn=" + HttpUtility.UrlEncode(streetAddress, Encoding.GetEncoding("Windows-1252")) + "&po=" + HttpUtility.UrlEncode(city, Encoding.GetEncoding("Windows-1252")) + "");
        StreamReader inp = new StreamReader(w.GetResponse().GetResponseStream(), Encoding.GetEncoding(1252));

        HtmlDocument d = new HtmlDocument();
        d.LoadHtml(inp.ReadToEnd());
        HtmlNode table = d.DocumentNode.SelectSingleNode("//table[@class='result']");
        HtmlNodeCollection rows = table.SelectNodes("./tr");
        foreach (HtmlNode row in rows)
        {
            GeoSok.PostenResult res = new PostenResult();
            res.Street = row.ChildNodes[0].InnerText;
            res.Number = row.ChildNodes[1].InnerText;
            res.Code = row.ChildNodes[2].InnerText;
            res.City = row.ChildNodes[3].InnerText;

            resList.Add(res);
        }

        return resList;
    }

    [WebMethod]
    public List<GeoSok.PostenResult> FindByPostCodeInPosten (string postCode)
    {
        List<GeoSok.PostenResult> resList = new List<GeoSok.PostenResult>();
        WebRequest w = WebRequest.Create("http://www.posten.se/soktjanst/postnummersok/resultat.jspv?pnr=" + HttpUtility.UrlEncode(postCode, Encoding.GetEncoding("Windows-1252")));
        StreamReader inp = new StreamReader(w.GetResponse().GetResponseStream(), Encoding.GetEncoding(1252));

        HtmlDocument d = new HtmlDocument();
        d.LoadHtml(inp.ReadToEnd());
        HtmlNode table = d.DocumentNode.SelectSingleNode("//table[@class='result']");
        HtmlNodeCollection rows = table.SelectNodes("./tr");
        foreach (HtmlNode row in rows)
        {
            GeoSok.PostenResult res = new PostenResult();
            res.Street = row.ChildNodes[0].InnerText;
            res.Number = row.ChildNodes[1].InnerText;
            res.Code = row.ChildNodes[2].InnerText;
            res.City = row.ChildNodes[3].InnerText;

            resList.Add(res);
        }

        return resList;
    }

    [WebMethod]
    public List<GeoSok.EniroResult> FindNearInEniro (string address, GeoPoint closeToPoint)
    { 
         List<GeoSok.EniroResult> addresses = FindInEniro ( address);
         foreach (GeoSok.EniroResult res in addresses)
             res.Dist = Math.Abs(GeoDist(closeToPoint, new GeoPoint(res.Lat, res.Lon)));
         addresses.Sort(
            delegate(GeoSok.EniroResult p1, GeoSok.EniroResult p2)
                 {
                     return p2.Dist.CompareTo(p1.Dist);
                 }
            );
         return addresses;
    }

    [WebMethod]
    public List<GeoSok.EniroResult> FindInEniro (string address)
    {
        List<GeoSok.EniroResult> resList = new List<GeoSok.EniroResult>();
        WebRequest w = WebRequest.Create("http://kartor.eniro.se//query?"
        + "mop=aq"
        + "&mapstate="
        + "&what=map_adr"
        + "&geo_area=" + HttpUtility.UrlEncode(address, Encoding.GetEncoding("Windows-1252"))
        + "&stq=0"
        + "&pis=0"
        + "&tpl=map_ajax_map_adr"
        + "&ajax_get_cache="
        );
        StreamReader inp = new StreamReader(w.GetResponse().GetResponseStream(), Encoding.GetEncoding(1252));
        string fullResponse = inp.ReadToEnd();
        string[] responseB;
        if (fullResponse.Contains("selectAddress"))
        {
            Regex re = new Regex(@"selectAddress\((.+?)\)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection matches = re.Matches(fullResponse);
            foreach (Match match in matches)
            {
                string[] responseA = match.Value.Split(new string[] { "," }, StringSplitOptions.None);
                EniroResult res = new EniroResult();
                responseB = responseA[3].Split(new string[] { ";" }, StringSplitOptions.None);
                res.City = responseB[7];
                res.Code = responseB[6];
                res.Municipality = responseB[17];
                res.Street = responseB[3];
                res.Number = responseB[4];

                double parseResult = 0;
                double.TryParse(responseB[12], NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US").NumberFormat, out parseResult);
                res.Lon = parseResult;

                parseResult = 0;
                double.TryParse(responseB[13], NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US").NumberFormat, out parseResult);
                res.Lat = parseResult;

                resList.Add(res);

            }
        }
        else
        {
            EniroResult res = new EniroResult();
            string[] responseA = fullResponse.Split(new string[] { "!_!" }, StringSplitOptions.None);
            string responsDecoded = HttpUtility.UrlDecode(responseA[3], Encoding.GetEncoding(1252));
            responseB = responsDecoded.Split(new string[] { ";" }, StringSplitOptions.None);
            res.City = responseB[7];
            res.Code = responseB[6];
            res.Municipality = responseB[17];
            res.Street = responseB[3];
            res.Number = responseB[4];

            double parseResult = 0;
            double.TryParse(responseB[12], NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US").NumberFormat, out parseResult);
            res.Lon = parseResult;

            parseResult = 0;
            double.TryParse(responseB[13], NumberStyles.Float | NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US").NumberFormat, out parseResult);
            res.Lat = parseResult;

            resList.Add(res);
        }

        return resList;
    }

    public static double GeoDist (GeoPoint p1, GeoPoint p2)
    {
        return GeoDist (p1.Lat, p1.Long, p2.Lat, p2.Long);
    }
    public static double GeoDist (double Lat1, double Long1, double Lat2, double Long2)
    {
        return Math.Acos(Math.Cos(ToRadians(90 - Lat1)) * Math.Cos(ToRadians(90 - Lat2)) + Math.Sin(ToRadians(90 - Lat1)) * Math.Sin(ToRadians(90 - Lat2)) * Math.Cos(ToRadians(Long1 - Long2))) * 6371;

    }
    public static double ToRadians (double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
