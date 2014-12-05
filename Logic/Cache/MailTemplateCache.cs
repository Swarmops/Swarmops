using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Cache
{
    public class MailTemplateCache
    {
        private static DateTime lastAccess = DateTime.MinValue;
        private static readonly object loadCacheLock = new object();
        public static bool loadCache = true;

        private static Dictionary<string, List<BasicMailTemplate>> __MailTemplateCache =
            new Dictionary<string, List<BasicMailTemplate>>();

        private static readonly int cacheLifeSpanMinutes = 1;

        static MailTemplateCache()
        {
            lastAccess = DateTime.MinValue;
        }

        private static List<BasicMailTemplate> GetCachedTemplates (string templateName)
        {
            lock (loadCacheLock)
            {
                if (loadCache || lastAccess.AddMinutes (cacheLifeSpanMinutes) < DateTime.Now)
                {
                    __MailTemplateCache = new Dictionary<string, List<BasicMailTemplate>>();
                    loadCache = false;
                }

                if (!__MailTemplateCache.ContainsKey (templateName))
                {
                    BasicMailTemplate[] basicTemplates =
                        SwarmDb.GetDatabaseForReading().GetMailTemplatesByName (templateName);
                    List<BasicMailTemplate> tmplList = new List<BasicMailTemplate> (basicTemplates);
                    __MailTemplateCache[templateName] = tmplList;
                }

                lastAccess = DateTime.Now;
                return __MailTemplateCache[templateName];
            }
        }

        public static BasicMailTemplate GetBestMatch (string templateName, string language, string country,
            Organization org)
        {
            List<BasicMailTemplate> tmplList = GetCachedTemplates (templateName);

            Organizations orgLine = (org != null) ? org.GetLine() : new Organizations();

            int[] lineIDs = orgLine.Identities;
            List<int> idlist = new List<int> (lineIDs);

            BasicMailTemplate templateDefault = null;
            BasicMailTemplate countryDefault = null;
            BasicMailTemplate bestSofar = null;
            int bestIndex = -1;

            foreach (BasicMailTemplate bmt in tmplList)
            {
                int thisIndex = idlist.IndexOf (bmt.OrganizationId);
                if (thisIndex > bestIndex)
                {
                    bestIndex = thisIndex;
                    bestSofar = bmt;
                }
                else if (bmt.CountryCode.ToUpper() == country && bmt.OrganizationId < 1)
                {
                    countryDefault = bmt;
                }
                else if (bmt.CountryCode == "" && bmt.OrganizationId < 1)
                {
                    templateDefault = bmt;
                }
            }
            if (bestSofar != null)
                return bestSofar;
            if (countryDefault != null)
                return countryDefault;
            if (templateDefault != null)
                return templateDefault;
            return null;
        }
    }
}