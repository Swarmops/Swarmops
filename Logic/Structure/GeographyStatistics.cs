using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class GeographyStatistics : Dictionary<int, GeographyDataPoint>
    {
        public DateTime Timestamp;

        public GeographyStatistics()
        {
            this.Timestamp = DateTime.Now;
        }

        public static GeographyStatistics GeneratePresent (int[] memberCountForOrganizations)
        {
            Dictionary<int, int> voterCounts = SwarmDb.GetDatabaseForReading().GetGeographyVoterCounts();
            GeographyStatistics result = new GeographyStatistics();

            // Step 1 - tally the leaf nodes

            foreach (int geographyId in voterCounts.Keys)
            {
                GeographyDataPoint dataPoint = new GeographyDataPoint
                {
                    GeographyName = Geography.FromIdentity (geographyId).Name,
                    GeographyId = geographyId,
                    VoterCount = voterCounts[geographyId]
                };

                result[geographyId] = dataPoint;
            }


            // Step 2 - add the member counts to the individual requested geo nodes

            foreach (int orgId in memberCountForOrganizations)
            {
                People members =
                    People.FromMemberships (
                        Memberships.ForOrganizations (Organization.FromIdentity (orgId).ThisAndBelow()));

                foreach (Person person in members)
                {
                    Geography geography = person.Geography;

                    // If we don't have this key, it's because it's too far down. Move up the tree until we're at least
                    // at municipal level.

                    while (geography.Identity != 1 && !result.ContainsKey (geography.Identity))
                    {
                        geography = geography.Parent;
                    }

                    // Add the data, unless we hit the roof in the last op.

                    if (geography.Identity != 1)
                    {
                        int birthYearBracket = (person.Birthdate.Year - 1900)/5;

                        if (birthYearBracket >= 0 && birthYearBracket < 30)
                        {
                            result[geography.Identity].OrganizationData[orgId - 1].BirthYearBracketMemberCounts[
                                birthYearBracket]++;
                        }

                        if (person.IsFemale)
                        {
                            result[geography.Identity].OrganizationData[orgId - 1].FemaleMemberCount++;
                        }
                        else if (person.IsMale)
                        {
                            result[geography.Identity].OrganizationData[orgId - 1].MaleMemberCount++;
                        }
                    }
                }
            }

            // TODO: Activist count as a new step


            // Step 3 - add up the totals for every intermediate node (expensive!)

            Geographies allGeographies = Geography.Root.ThisAndBelow();

            foreach (Geography geography in allGeographies)
            {
                Geographies localTree = geography.ThisAndBelow();
                int voterCount = 0;
                GeographyOrganizationDataPoint[] tempOrgData = new GeographyOrganizationDataPoint[2]; // HACK
                tempOrgData[0] = new GeographyOrganizationDataPoint();
                tempOrgData[1] = new GeographyOrganizationDataPoint();

                foreach (Geography localNode in localTree)
                {
                    // Increment our temp values for every geo node below the one we're currently processing.

                    if (!result.ContainsKey (localNode.Identity))
                    {
                        continue;
                    }

                    voterCount += result[localNode.Identity].VoterCount;

                    for (int orgIndex = 0; orgIndex < 2; orgIndex++)
                    {
                        for (int ageBracketIndex = 0; ageBracketIndex < 30; ageBracketIndex++)
                        {
                            tempOrgData[orgIndex].BirthYearBracketMemberCounts[ageBracketIndex] +=
                                result[localNode.Identity].OrganizationData[orgIndex].BirthYearBracketMemberCounts[
                                    ageBracketIndex];
                        }

                        tempOrgData[orgIndex].ActivistCount +=
                            result[localNode.Identity].OrganizationData[orgIndex].ActivistCount;
                        tempOrgData[orgIndex].FemaleMemberCount +=
                            result[localNode.Identity].OrganizationData[orgIndex].FemaleMemberCount;
                        tempOrgData[orgIndex].MaleMemberCount +=
                            result[localNode.Identity].OrganizationData[orgIndex].MaleMemberCount;
                    }
                }

                if (!result.ContainsKey (geography.Identity))
                {
                    result[geography.Identity] = new GeographyDataPoint
                    {
                        GeographyId = geography.Identity,
                        GeographyName = geography.Name
                    };
                }

                // Save our temp values to the processed node.

                result[geography.Identity].VoterCount = voterCount;

                for (int orgIndex = 0; orgIndex < 2; orgIndex++)
                {
                    for (int ageBracketIndex = 0; ageBracketIndex < 30; ageBracketIndex++)
                    {
                        result[geography.Identity].OrganizationData[orgIndex].BirthYearBracketMemberCounts[
                            ageBracketIndex] =
                            tempOrgData[orgIndex].BirthYearBracketMemberCounts[ageBracketIndex];
                    }

                    result[geography.Identity].OrganizationData[orgIndex].ActivistCount =
                        tempOrgData[orgIndex].ActivistCount;
                    result[geography.Identity].OrganizationData[orgIndex].FemaleMemberCount =
                        tempOrgData[orgIndex].FemaleMemberCount;
                    result[geography.Identity].OrganizationData[orgIndex].MaleMemberCount =
                        tempOrgData[orgIndex].MaleMemberCount;
                }
            }


            // Step 4 - collect

            return result;
        }

        public static GeographyStatistics FromXml (string xml)
        {
            GeographyStatisticsSerializable serializable = GeographyStatisticsSerializable.FromXml (xml);

            GeographyStatistics result = new GeographyStatistics();

            foreach (GeographyDataPoint dataPoint in serializable)
            {
                result[dataPoint.GeographyId] = dataPoint;
                result[dataPoint.GeographyId].GeographyName = HttpUtility.HtmlDecode (dataPoint.GeographyName);
            }

            return result;
        }


        public string ToXml()
        {
            // Serialize through a GeographyStatisticsSerializable.

            GeographyStatisticsSerializable serializable = new GeographyStatisticsSerializable();

            foreach (GeographyDataPoint dataPoint in Values)
            {
                dataPoint.GeographyName = HttpUtility.HtmlEncode (dataPoint.GeographyName);
                serializable.Add (dataPoint);
            }

            return serializable.ToXml();
        }
    }

    [Serializable]
    public class GeographyStatisticsSerializable : List<GeographyDataPoint>
    {
        public static GeographyStatisticsSerializable FromXml (string xml)
        {
            XmlSerializer serializer = new XmlSerializer (typeof (GeographyStatisticsSerializable));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.Default.GetBytes (xml);
            stream.Write (xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            GeographyStatisticsSerializable result = (GeographyStatisticsSerializable) serializer.Deserialize (stream);
            stream.Close();

            return result;
        }


        public string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer (typeof (GeographyStatisticsSerializable));

            MemoryStream stream = new MemoryStream();
            serializer.Serialize (stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.Default.GetString (xmlBytes);
        }
    }
}