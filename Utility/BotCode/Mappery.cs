using System;
using System.Collections.Generic;
using System.Drawing;

namespace Swarmops.Utility.BotCode
{
    public class Mappery
    {
        public Dictionary<int, string> OrgColorLookup; // ugly way to transfer data to delegate

        [Obsolete("Too specialized for Swarmops. Plugin or generalize.", true)]
        public static void CreateUngPiratUptakeMap()
        {
            throw new NotImplementedException();

            /*
            string svg = string.Empty;
            string legendTemplate = string.Empty;

            using (
                StreamReader reader = new StreamReader("content/se-up-uptake-municipalities-template.svg",
                                                       Encoding.Default))
            {
                svg = reader.ReadToEnd();
            }

            using (
                StreamReader reader = new StreamReader("content/se-up-uptakemap-legend-template.txt", Encoding.Default))
            {
                legendTemplate = reader.ReadToEnd();
            }

            Organizations effectiveOrgs = new Organizations();
            Organizations possibleOrgs = Organization.FromIdentity(Organization.UPSEid).GetTree();

            foreach (Organization org in possibleOrgs)
            {
                if (org.AcceptsMembers && org.AutoAssignNewMembers)
                {
                    effectiveOrgs.Add(org);
                }
            }

            string legend = string.Empty;

            effectiveOrgs.Sort();

            Dictionary<int, string> colorLookup = new Dictionary<int, string>();
            int position = 0;
            float curLight = .35f;
            float curSaturation = 1f;

            foreach (Organization org in effectiveOrgs)
            {
                curLight += .25f;
                if (curLight > .85f)
                {
                    curLight = .35f;
                }
                else if (curLight > .8f)
                {
                    curLight = .75f;
                }

                curSaturation = 1.45f - curSaturation;


                Color color = ColorFromAhsb(128, (float) position*360/(float) effectiveOrgs.Count, curSaturation,
                                            curLight);

                if (org.CatchAll)
                {
                    color = Color.FromArgb(240, 240, 240);
                }

                string colorString = String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
                colorLookup[org.Identity] = colorString;

                string orgLegend = legendTemplate.
                    Replace("%%NAME%%", Encoding.Default.GetString(Encoding.UTF8.GetBytes(org.NameShort))).
                    Replace("%%YPOS%%", (position*17).ToString()).
                    Replace("%%COLOR%%", colorString).
                    Replace("%%GROUPID%%", "org" + org.Identity.ToString());

                legend += orgLegend;

                position++;
            }

            Mappery mappery = new Mappery();
            mappery.OrgColorLookup = colorLookup; // Ugly way to transfer data to delegate

            svg = svg.Replace("<!--%%LEGEND%%-->", legend);

            Regex regex =
                new Regex("(?<start>\\<path.+?fill:)(?<color>.+?)(?<middle>;.+?id=\\\")(?<id>.+?)(?<end>\\\".+?/\\>)",
                          RegexOptions.Singleline);

            MatchEvaluator matchEvaluator = new MatchEvaluator(mappery.UPUptakeLookupReplacer);

            Persistence.Key["OrgUptakeMap-2"] = regex.Replace(svg, matchEvaluator);*/
        }

        /*
        public string UPUptakeLookupReplacer (Match match)
        {
            string resultColor = "#000000";
            string id = match.Groups["id"].Value;

            if (!id.StartsWith("text"))
            {
                Geography geo = Geography.FromOfficialDesignation(1, GeographyLevel.Municipality, id);
                Organization org = Organizations.GetMostLocalOrganization(geo.Identity, Organization.UPSEid);

                resultColor = OrgColorLookup[org.Identity];
            }

            return match.Groups["start"].Value + resultColor + match.Groups["middle"].Value + id +
                   match.Groups["end"].Value;
        }*/


        public static Color ColorFromAhsb(int a, float h, float s, float b)
        {
            if (0 == s)
            {
                return Color.FromArgb(a, Convert.ToInt32(b*255),
                    Convert.ToInt32(b*255), Convert.ToInt32(b*255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b*s) + s;
                fMin = b + (b*s) - s;
            }
            else
            {
                fMax = b + (b*s);
                fMin = b - (b*s);
            }

            iSextant = (int) Math.Floor(h/60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f*(float) Math.Floor(((iSextant + 1f)%6f)/2f);
            if (0 == iSextant%2)
            {
                fMid = h*(fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h*(fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax*255);
            iMid = Convert.ToInt32(fMid*255);
            iMin = Convert.ToInt32(fMin*255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(a, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(a, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(a, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(a, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(a, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(a, iMax, iMid, iMin);
            }
        }

        /*
        public static void CreatePiratpartietOrganizationStrengthCircuitMap()
        {
            string svg = string.Empty;

            using (
                StreamReader reader = new StreamReader("content/se-circuits-orgstrength-template.svg", Encoding.Default)
                )
            {
                svg = reader.ReadToEnd();
            }

            Regex regex =
                new Regex("(?<start>\\<path.+?fill:)(?<color>.+?)(?<middle>;.+?id=\\\")(?<id>.+?)(?<end>\\\".+?/\\>)",
                          RegexOptions.Singleline);

            MatchEvaluator matchEvaluator = new MatchEvaluator(new Mappery().SwedishCircuitMapOrgStrengthLookupReplacer);

            Persistence.Key["CircuitOrgStrength-1"] = regex.Replace(svg, matchEvaluator);
        }


        public static void CreatePiratpartietOrganizationStrengthCityMap()
        {
            string svg = string.Empty;

            using (
                StreamReader reader = new StreamReader("content/se-municipalities-orgstrength-template.svg",
                                                       Encoding.Default))
            {
                svg = reader.ReadToEnd();
            }

            svg = svg.Replace("%COUNT%", GetCityLeadCount(1, 1).ToString());

            Regex regex =
                new Regex("(?<start>\\<path.+?fill:)(?<color>.+?)(?<middle>;.+?id=\\\")(?<id>.+?)(?<end>\\\".+?/\\>)",
                          RegexOptions.Singleline);

            MatchEvaluator matchEvaluator = new MatchEvaluator(new Mappery().SwedishCityMapOrgStrengthLookupReplacer);

            Persistence.Key["CityOrgStrength-1"] = regex.Replace(svg, matchEvaluator);
        }


        public static int GetCityLeadCount (int organizationId, int countryId)
        {
            Geographies cities = Geographies.FromLevel(countryId, GeographyLevel.Municipality);

            int leadCount = 0;

            foreach (Geography city in cities)
            {
                RoleLookup roles = RoleLookup.FromGeographyAndOrganization(city.Identity, organizationId);

                if (roles[RoleType.LocalLead].Count > 0)
                {
                    leadCount++;
                }
            }

            return leadCount;
        }

        public string SwedishCircuitMapOrgStrengthLookupReplacer (Match match)
        {
            // Five groups: "color" and "id" are the keys, "start", "middle" and "end" are what to paste in between.

            Dictionary<string, int> lookup = new Dictionary<string, int>();

            lookup["2N"] = 2;
            lookup["2S"] = 38;
            lookup["1"] = 40;

            Organization org = Organization.FromIdentity(1); // Replace later for more generic

            string resultColorString = "#000000";
            string id = match.Groups["id"].Value;
            int geoId = 0;

            try
            {
                if (lookup.ContainsKey(id))
                {
                    geoId = lookup[id];
                }
                else
                {
                    geoId = Int32.Parse(id);
                }

                Geography geo = Geography.FromIdentity(geoId);

                RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, org.Identity);

                bool hasLead = officers[RoleType.LocalLead].Count > 0;
                bool hasSecond = officers[RoleType.LocalDeputy].Count > 0;

                Geographies geoTree = geo.GetTree();

                int cities = 0;
                int citiesWithLead = 0;

                foreach (Geography localGeo in geoTree)
                {
                    if (localGeo.Identity == geo.Identity)
                    {
                        continue;
                    }

                    if (localGeo.Name.EndsWith("kommun"))
                    {
                        cities++;

                        officers = RoleLookup.FromGeographyAndOrganization(localGeo.Identity, org.Identity);

                        if (officers[RoleType.LocalLead].Count > 0)
                        {
                            citiesWithLead++;
                        }
                    }
                }

                int cityLeadPercent = 100;

                if (cities > 0)
                {
                    cityLeadPercent = citiesWithLead*100/cities;
                }

                // Determine color

                Color color = Color.Red;

                if (!hasLead)
                {
                    color = Color.Red;
                }
                else if (cityLeadPercent > 80 && hasSecond)
                {
                    color = Color.Green;
                }
                else
                {
                    // Find a hue between Orange and Light Green. Say, between 30 and 120.

                    cityLeadPercent += 30;
                    if (cityLeadPercent > 120)
                    {
                        cityLeadPercent = 120;
                    }

                    color = ColorFromAhsb(100, cityLeadPercent, 1.0f, 0.5f);
                }

                resultColorString = String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
            }
            catch (Exception)
            {
                // Ignore - color will be black
            }

            return match.Groups["start"].Value + resultColorString + match.Groups["middle"].Value + id +
                   match.Groups["end"].Value;
        }


        public string SwedishCityMapOrgStrengthLookupReplacer (Match match)
        {
            // Five groups: "color" and "id" are the keys, "start", "middle" and "end" are what to paste in between.

            Organization org = Organization.FromIdentity(1); // Replace later for more generic

            string resultColorString = "#000000";
            string id = match.Groups["id"].Value;

            // If not constructed for this instance, construct the volunteer cache

            if (this.volunteerCache == null)
            {
                this.volunteerCache = new Dictionary<int, bool>();

                Volunteers volunteers = Volunteers.GetOpen();

                foreach (Volunteer volunteer in volunteers)
                {
                    volunteerCache[volunteer.Geography.Identity] = true;
                }
            }


            try
            {
                Geography geo = Geography.FromOfficialDesignation(1, GeographyLevel.Municipality, id);

                int circuitLeadCount = 0;
                int cityLeadCount = 0;
                int cityVolunteerCount = 0;

                RoleLookup lookup = RoleLookup.FromGeographyAndOrganization(geo.Identity, 1);
                if (lookup[RoleType.LocalLead].Count > 0)
                {
                    cityLeadCount = 1;

                    if (lookup[RoleType.LocalDeputy].Count > 0)
                    {
                        cityLeadCount = 2;
                    }
                }

                if (volunteerCache.ContainsKey(geo.Identity))
                {
                    cityVolunteerCount = 1;
                }

                // Move up to circuit level

                while (!(geo.AtLevel(GeographyLevel.ElectoralCircuit)))
                {
                    geo = geo.Parent;
                }

                lookup = RoleLookup.FromGeographyAndOrganization(geo.Identity, 1);
                if (lookup[RoleType.LocalLead].Count > 0)
                {
                    circuitLeadCount = 1;

                    if (lookup[RoleType.LocalDeputy].Count > 0)
                    {
                        circuitLeadCount = 2;
                    }
                }

                float saturation = 1.0f;
                float brightness = 0.15f + 0.2f*circuitLeadCount;
                float hue = cityLeadCount*60; // red, yellow, green hues at 0°, 60°, 120°

                if (cityLeadCount < 2 && cityVolunteerCount > 0)
                {
                    hue += 30; // There are volunteers? Place at orange (for none) or yellow-green (for one).
                }

                Color color = ColorFromAhsb(255, hue, saturation, brightness);

                resultColorString = String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
            }
            catch (Exception)
            {
            }

            try
            {
                if (lookup.ContainsKey(id))
                {
                    geoId = lookup[id];
                }
                else
                {
                    geoId = Int32.Parse(id);
                }

                Geography geo = Geography.FromIdentity(geoId);

                RoleLookup officers = RoleLookup.FromGeographyAndOrganization(geo.Identity, org.Identity);

                bool hasLead = officers[RoleType.LocalLead].Count > 0;
                bool hasSecond = officers[RoleType.LocalDeputy].Count > 0;

                Geographies geoTree = geo.GetTree();

                int cities = 0;
                int citiesWithLead = 0;

                foreach (Geography localGeo in geoTree)
                {
                    if (localGeo.Identity == geo.Identity)
                    {
                        continue;
                    }

                    if (localGeo.Name.EndsWith("kommun"))
                    {
                        cities++;

                        officers = RoleLookup.FromGeographyAndOrganization(localGeo.Identity, org.Identity);

                        if (officers[RoleType.LocalLead].Count > 0)
                        {
                            citiesWithLead++;
                        }
                    }
                }

                int cityLeadPercent = 100;

                if (cities > 0)
                {
                    cityLeadPercent = citiesWithLead * 100 / cities;
                }

                // Determine color

                Color color = Color.Red;

                if (!hasLead)
                {
                    color = Color.Red;
                }
                else if (cityLeadPercent > 80 && hasSecond)
                {
                    color = Color.Green;
                }
                else
                {
                    // Find a hue between Orange and Light Green. Say, between 30 and 120.

                    cityLeadPercent += 30;
                    if (cityLeadPercent > 120)
                    {
                        cityLeadPercent = 120;
                    }

                    color = ColorFromAhsb(100, cityLeadPercent, 1.0f, 0.5f);
                }

                resultColorString = String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
            }
            catch (Exception)
            {
                // Ignore - color will be black
            }
              


            return match.Groups["start"].Value + resultColorString + match.Groups["middle"].Value + id +
                   match.Groups["end"].Value;
        }

        private Dictionary<int, bool> volunteerCache;*/
    }
}