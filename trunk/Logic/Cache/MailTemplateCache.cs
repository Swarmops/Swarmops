using System;
using Activizr.Logic.Structure;
using Activizr.Database;
using Activizr.Basic.Types;
using System.Collections.Generic;
using Activizr.Basic.Enums;

namespace Activizr.Logic.Cache
{
    public class MailTemplateCache
    {
        static private DateTime lastAccess = DateTime.MinValue;
        static private object loadCacheLock = new object();
        static public bool loadCache = true;
        static private Dictionary<string, List<BasicMailTemplate>> __MailTemplateCache = new Dictionary<string, List<BasicMailTemplate>>();
        static readonly int cacheLifeSpanMinutes = 1;

        static MailTemplateCache ()
        {
            lastAccess = DateTime.MinValue;
        }

        static private List<BasicMailTemplate> GetCachedTemplates (string templateName)
        {
            lock (loadCacheLock)
            {
                if (loadCache || lastAccess.AddMinutes(cacheLifeSpanMinutes) < DateTime.Now)
                {
                    __MailTemplateCache = new Dictionary<string, List<BasicMailTemplate>>();
                    loadCache = false;
                }

                if (!__MailTemplateCache.ContainsKey(templateName))
                {
                    BasicMailTemplate[] basicTemplates = PirateDb.GetDatabase().GetMailTemplatesByName(templateName);
                    List<BasicMailTemplate> tmplList = new List<BasicMailTemplate>(basicTemplates);
                    __MailTemplateCache[templateName] = tmplList;

                }

                lastAccess = DateTime.Now;
                return __MailTemplateCache[templateName];
            }
        }

        static public BasicMailTemplate GetBestMatch (string templateName, string language, string country, Organization org)
        {
            List<BasicMailTemplate> tmplList = GetCachedTemplates(templateName);

            Organizations orgLine = ( org != null) ? org.GetLine() : new Organizations();

            int[] lineIDs = orgLine.Identities;
            List<int> idlist = new List<int>(lineIDs);

            BasicMailTemplate templateDefault = null;
            BasicMailTemplate countryDefault = null;
            BasicMailTemplate bestSofar = null;
            int bestIndex = -1;

            foreach (BasicMailTemplate bmt in tmplList)
            {
                int thisIndex = idlist.IndexOf(bmt.OrganizationId);
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
            else if (countryDefault != null)
                return countryDefault;
            else if (templateDefault != null)
                return templateDefault;
            else
                return null;
        }
    }
}