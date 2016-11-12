using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Media;
using Activizr.Logic.Support;


public partial class Pages_Special_SE_MediaEntriesFra : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string blogResult = string.Empty;
        string oldMediaResult = string.Empty;

        List<BlogEntry> blogEntries = new List<BlogEntry>();

        Dictionary<string, string> blogNameReplace = new Dictionary<string, string>();
        blogNameReplace["Erik Laakso | På Uppstuds"] = "Erik Laakso";
        blogNameReplace["opassande"] = "Opassande";
        blogNameReplace["Tommy k Johanssons blogg om datorer & Internet"] = "Tommy K Johansson";
        blogNameReplace["MinaModerataKarameller..."] = "Mina Moderata Karameller";
        blogNameReplace["d y s l e s b i s k ."] = "Dyslesbisk";
        blogNameReplace["syrrans granne"] = "Syrrans Granne";
        blogNameReplace["SURGUBBEN"] = "Surgubben";
        blogNameReplace["Henrik-Alexandersson.se"] = "Hax";
        blogNameReplace["BIOLOGY & POLITICS"] = "Biology & Politics";
        blogNameReplace["Blogge Bloggelito - regeringsblogg"] = "Blogge Bloggelito";
        blogNameReplace["drottningsylt"] = "Drottningsylt";
        blogNameReplace["RADIKALEN"] = "Radikalen";
        blogNameReplace["Idéer, tankar och reflektionerHeit&hellip;"] = "Idéer Tankar Reflektioner";
        blogNameReplace["stationsvakt"] = "Stationsvakt";
        blogNameReplace["Framtidstanken - Accelererande för&hellip;"] = "Framtidstanken";
        blogNameReplace["CONJOINER"] = "Conjoiner";
        blogNameReplace["..:: josephzohn | gås blogger ::.."] = "Josephzohn";
        blogNameReplace["S I N N E R"] = "Sinner";
        blogNameReplace["mp) blog från Staffanstorp"] = "Olle (mp) från Staffanstorp";
        blogNameReplace["Oväsentligheter ... ?? ???"] = "Oväsentligheter";
        blogNameReplace["SJÖLANDER"] = "Sjölander";
        blogNameReplace["UPPSALAHANSEN - en og en halv nordmann i sverige"] = "Uppsala-Hansen";
        blogNameReplace["se|com|org|nu)"] = "Lex Orwell";
        blogNameReplace["s) blogg"] = "John Johansson (s)";
        blogNameReplace["m) 3.0"] = "Edvin Ala(m) 3.0";
        blogNameReplace["PLIKTEN FRAMFÖR ALLT"] = "Plikten framför allt";
        blogNameReplace["*Café Liberal"] = "Café Liberal";
        blogNameReplace["Disruptive - En blogg om entreprenörskap, riskkapital och webb 2.0"] = "Disruptive";
        blogNameReplace["C)KER"] = "Törnqvist tänker och tycker";
        blogNameReplace["serier mot FRA)"] = "Inte så PK (Serier mot FRA)";
        blogNameReplace["BÄSTIGAST.SNYGGAST.SNÄLLAST.ÖDMJUKAST"] = "Johanna Wiström";
        blogNameReplace["LOKE - KULTUR & POLITIK"] = "Loke kultur & politik";
        blogNameReplace["Webnewspaper)"] = "The Awkward Swedeblog";
        blogNameReplace["SOCIALIST OCH BAJARE- Livets passion"] = "Nicke Grozdanovski";
        blogNameReplace["MÅRTENSSON"] = "Mårtensson";
        blogNameReplace["FRADGA"] = "Fradga";
        blogNameReplace["UD/RK Samhälls Debatt"] = "UD/RK Samhällsdebatt";
        blogNameReplace["GRETAS SVAMMEL"] = "Gretas Svammel";
        blogNameReplace["ISAK ENGQVIST"] = "Isak Engqvist";
        blogNameReplace["Smålandsposten - Blogg - Marcus Svensson"] = "Marcus Svensson (Smålandsposten)";
        blogNameReplace["f.d. Patrik i Politiken)"] = "Mittenradikalen";

        Dictionary<string, double> blogWeight = new Dictionary<string, double>();
        blogWeight["rickfalkvinge.se"] = 3.0;
        blogWeight["opassande.se"] = 3.0;
        blogWeight["christianengstrom"] = 3.0;
        blogWeight["rosettasten"] = 3.0;
        blogWeight["projo"] = 2.0;
        blogWeight["basic70.wordpress.com"] = 2.0;
        blogWeight["scriptorium.se"] = 2.0;
        blogWeight["teflonminne.se"] = 2.0;
        blogWeight["kurvigheter.blogspot"] = 2.0;
        blogWeight["ravennasblogg.blogspot.com"] = 3.0;
        blogWeight["webhackande.se"] = 3.0;
        blogWeight["jinge"] = 0.5; // störig
        blogWeight["klaric.se"] = 0.1; // sd
        blogWeight["patrikohlsson.wordpress.com"] = 0.1; // sd

        double highestWeight = 3.0;
        double maximumAgeDays = 3.0;

        MediaEntries blogEntriesOriginal = MediaEntries.FromBlogKeyword("FRA", DateTime.Now.AddDays(-maximumAgeDays * highestWeight));
        Dictionary<string, bool> dupeCheck = new Dictionary<string, bool>();

        foreach (MediaEntry entry in blogEntriesOriginal)
        {
            BlogEntry blogEntry = new BlogEntry();
            blogEntry.entry = entry;
            blogEntry.ageAdjusted = new TimeSpan((long) ((DateTime.Now - entry.DateTime).Ticks / GetBlogWeight(blogWeight, entry.Url)));

            if (entry.Url.StartsWith ("http://knuff.se/k/"))
            {
                UrlTranslations.Create(entry.Url);
            }
            else if (blogEntry.ageAdjusted.Days < maximumAgeDays)
            {
                if (blogNameReplace.ContainsKey(entry.MediaName))
                {
                    blogEntry.entry = MediaEntry.FromBasic (new Activizr.Basic.Types.BasicMediaEntry(blogEntry.entry.Id, blogEntry.entry.KeywordId, blogNameReplace[entry.MediaName], true, blogEntry.entry.Title, blogEntry.entry.Url, blogEntry.entry.DateTime));
                }

                if (!dupeCheck.ContainsKey(entry.Url))
                {
                    blogEntries.Add(blogEntry);
                    dupeCheck[entry.Url] = true;
                }
            }
        }

        blogEntries.Sort(CompareBlogEntries);

        Dictionary<string, int> lookupSources = new Dictionary<string, int>();
        foreach (BlogEntry entry in blogEntries)
        {
            if (blogResult.Length > 0)
            {
                blogResult += " | ";
            }

            blogResult += String.Format("<a href=\"{0}\" title=\"{1}\">{2}</a>", entry.entry.Url, HttpUtility.HtmlEncode (entry.entry.Title), HttpUtility.HtmlEncode (entry.entry.MediaName));

            if (lookupSources.ContainsKey(entry.entry.MediaName))
            {
                lookupSources[entry.entry.MediaName]++;

                if (lookupSources[entry.entry.MediaName] == 2)
                {
                    blogResult += " igen";
                }
                else
                {
                    blogResult += " #" + lookupSources[entry.entry.MediaName].ToString();
                }
            }
            else
            {
                lookupSources[entry.entry.MediaName] = 1;
            }
        }

        this.literalBlogOutput.Text = blogResult;

        MediaEntries oldMediaEntries = MediaEntries.FromOldMediaKeyword("FRA", DateTime.Now.AddDays(-maximumAgeDays / 2));

        foreach (MediaEntry entry in oldMediaEntries)
        {
            if (!entry.Url.Contains(".se/"))
            {
                continue;
            }

            if (oldMediaResult.Length > 0)
            {
                oldMediaResult += " | ";
            }

            oldMediaResult += String.Format("<a href=\"{0}\">{1} ({2})</a>", entry.Url, HttpUtility.HtmlEncode(entry.Title.Replace ("`", "'").Replace ("´", "'")), HttpUtility.HtmlEncode(entry.MediaName));
        }

        this.literalOldMediaOutput.Text = oldMediaResult;

    }



    static private int CompareBlogEntries(BlogEntry entry1, BlogEntry entry2)
    {
        TimeSpan result = entry1.ageAdjusted - entry2.ageAdjusted;

        if (result > new TimeSpan (0))
        {
            return 1;
        }
        if (result < new TimeSpan (0))
        {
            return -1;
        }

        return 0;
    }


    static private double GetBlogWeight (Dictionary<string, double> weightLookup, string url)
    {
        foreach (string key in weightLookup.Keys)
        {
            if (url.ToLower().Contains(key))
            {
                return weightLookup [key];
            }
        }

        return 1.0;
    }




    private class BlogEntry
    {
        public MediaEntry entry;
        public TimeSpan ageAdjusted;
    }

}
