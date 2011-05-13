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


public partial class Pages_Special_SE_MediaEntriesIpred1 : System.Web.UI.Page
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
        blogNameReplace["MinaModerataKarameller..."] = "Mary X Jensen";
        blogNameReplace["d y s l e s b i s k ."] = "Dyslesbisk";
        blogNameReplace["syrrans granne"] = "Syrrans Granne";
        blogNameReplace["SURGUBBEN"] = "Surgubben";
        blogNameReplace["Henrik-Alexandersson.se"] = "Henrik Alexandersson";
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
        blogNameReplace["Smålandsposten - Blogg - Marcus Svensson"] = "Marcus Svensson";
        blogNameReplace["f.d. Patrik i Politiken)"] = "Mittenradikalen";
        blogNameReplace["Rick Falkvinge (pp)"] = "Rick Falkvinge";
        blogNameReplace["Christian Engström (pp)"] = "Christian Engström";
        blogNameReplace["Basic personligt"] = "Daniel Brahneborg";
        blogNameReplace["yo mstuff!"] = "Marcin de Kaminski";
        blogNameReplace["EXORCISTEN"] = "Tommy Funebo";
        blogNameReplace["www.webhackande.se"] = "Lars Holmqvist";
        blogNameReplace["satmaran"] = "Satmaran";
        blogNameReplace["PiratJanne"] = "Jan Lindgren";
        blogNameReplace["EXORCISTEN"] = "Tommy Funebo";
        blogNameReplace["Intensifier"] = "Christopher Kullenberg";
        blogNameReplace["Anders Widén Författare"] = "Anders Widén";
        blogNameReplace["ProjektPåRiktigt"] = "Urban Cat";
        blogNameReplace["Davids åsikter"] = "David Wienehall";
        blogNameReplace["Minimaliteter"] = "Mikael Nilsson";
        blogNameReplace["scaber_nestor"] = "Scaber Nestor";
        blogNameReplace["andra sidan"] = "Andra Sidan";
        blogNameReplace["Klibbnisses Blogg"] = "Klibbnisse";
        blogNameReplace["V, fildelning & upphovsrätt"] = "Mikael von Knorring";
        blogNameReplace["SVT Opinion - redaktionsblogg"] = "SVT Opinion";
        blogNameReplace["Copyriot"] = "Rasmus Fleischer";
        blogNameReplace["Motpol"] = "Hans Engnell";
        blogNameReplace["Dynamic Man"] = "Mattias Swing";
        blogNameReplace["insane psycho clowns"] = "Insane Clowns";
        blogNameReplace["Samtidigt i Uppsala"] = "Mattias Bjärnemalm";
        blogNameReplace["Mattias tycker och tänker"] = "Mattias";
        blogNameReplace["Josef säger..."] = "Josef";
        blogNameReplace["Under den svarta natthimlen"] = "Peter Soilander";
        blogNameReplace["Farmorgun i Norrtälje"] = "Farmor Gun";
        blogNameReplace["lindahls blogg"] = "Johan Lindahl";
        blogNameReplace["kamferdroppar"] = "Charlotte Wiberg";
        blogNameReplace["Kulturbloggen"] = "Rose-Mari Södergren";
        blogNameReplace["Ett Otygs funderingar och betraktelser"] = "Martin Otyg";
        blogNameReplace["Jinges web och fotoblogg"] = "Jinge";
        blogNameReplace["Högerkonspiration"] = "Wilhelm Svenselius";
        blogNameReplace["piratbyran.org"] = "Piratbyrån";
        blogNameReplace["ANGIE ROGER"] = "Angie Roger";
        blogNameReplace["Lasses blogg"] = "Lasse Strömberg";
        blogNameReplace["annarkia"] = "Annarkia";
        blogNameReplace["Gontes förvirrade tankar i bloggvärlden"] = "Jonatan Kindh";
        blogNameReplace["Rejås Blog"] = "Marcus Rejås";
        


        Dictionary<string, double> blogWeight = new Dictionary<string, double>();
        blogWeight["rickfalkvinge.se"] = 3.0;
        blogWeight["opassande.se"] = 3.0;
        blogWeight["christianengstrom"] = 3.0;
        blogWeight["rosettasten"] = 3.0;
        blogWeight["projo"] = 2.0;
        blogWeight["basic70.wordpress.com"] = 2.0;
        blogWeight["scriptorium.se"] = 2.0;
        blogWeight["teflonminne.se"] = 2.0;
        blogWeight["piratjanne"] = 3.0;
        blogWeight["kurvigheter.blogspot"] = 2.0;
        blogWeight["piratbyran.org"] = 2.0; // tekniskt sett inte pp, men...
        blogWeight["ravennasblogg.blogspot.com"] = 3.0;
        blogWeight["webhackande.se"] = 3.0;
        blogWeight["jinge"] = 0.5; // störig
        blogWeight["klaric.se"] = 0.001; // sd
        blogWeight["patrikohlsson.wordpress.com"] = 0.001; // sd
        blogWeight["funebo"] = 0.001; // sd
        blogWeight["astudillo"] = 0.001; // idiot
        blogWeight["wb.blogg.se"] = 0.001; // idiot
        blogWeight["fotolasse"] = 0.001; // kd


        double highestWeight = 3.0;
        double maximumAgeDays = 7.0;

        MediaEntries blogEntriesOriginal = MediaEntries.FromBlogKeyword("Piratjägarlagen", DateTime.Now.AddDays(-maximumAgeDays * highestWeight));
        blogEntriesOriginal.Add (MediaEntries.FromBlogKeyword("Antipiratlagen", DateTime.Now.AddDays(-maximumAgeDays * highestWeight)));
        blogEntriesOriginal.Add(MediaEntries.FromBlogKeyword("IPRED1", DateTime.Now.AddDays(-maximumAgeDays * highestWeight)));
        blogEntriesOriginal.Add(MediaEntries.FromBlogKeyword("sanktionsdirektivet", DateTime.Now.AddDays(-maximumAgeDays * highestWeight)));
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
                blogResult += ", ";
            }

            blogResult += String.Format("<a href=\"{0}\" title=\"{1}\">{2}</a>", entry.entry.Url, HttpUtility.HtmlEncode (entry.entry.Title), HttpUtility.HtmlEncode (entry.entry.MediaName));

            if (lookupSources.ContainsKey(entry.entry.MediaName))
            {
                lookupSources[entry.entry.MediaName]++;

                if (lookupSources[entry.entry.MediaName] == 2)
                {
                    // blogResult += " igen";
                }
                else
                {
                    // blogResult += " #" + lookupSources[entry.entry.MediaName].ToString();
                }
            }
            else
            {
                lookupSources[entry.entry.MediaName] = 1;
            }
        }

        this.literalBlogOutput.Text = blogResult;

        MediaEntries oldMediaEntries = MediaEntries.FromOldMediaKeyword("Piratjägarlagen", DateTime.Now.AddDays(-maximumAgeDays / 2));
        oldMediaEntries.Add (MediaEntries.FromOldMediaKeyword("Antipiratlagen", DateTime.Now.AddDays(-maximumAgeDays / 2)));
        oldMediaEntries.Add (MediaEntries.FromOldMediaKeyword("IPRED1", DateTime.Now.AddDays(-maximumAgeDays / 2)));
        oldMediaEntries.Add(MediaEntries.FromOldMediaKeyword("sanktionsdirektivet", DateTime.Now.AddDays(-maximumAgeDays * highestWeight)));

        foreach (MediaEntry entry in oldMediaEntries)
        {
            if (!entry.Url.Contains(".se/"))
            {
                continue;
            }

            if (oldMediaResult.Length > 0)
            {
                oldMediaResult += ", ";
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
