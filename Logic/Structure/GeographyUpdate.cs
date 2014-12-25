using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Structure;
using Swarmops.Database;
using Swarmops.Logic.Automation;
using Swarmops.Logic.Cache;
using MasterGeography = Swarmops.Logic.Automation.Geography;
using MasterCity = Swarmops.Logic.Automation.City;
using MasterPostalCode = Swarmops.Logic.Automation.PostalCode;
using GeographyDataLoader = Swarmops.Logic.Automation.GetGeographyData;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class GeographyUpdate : BasicGeographyUpdate
    {
        [Obsolete("Do not call this ctor directly. It is intended for serialization use only.", true)]
        public GeographyUpdate()
        {
            
        }

        private GeographyUpdate (BasicGeographyUpdate basic)
            : base (basic)
        {
            // private ctor
        }

        public static GeographyUpdate FromBasic(BasicGeographyUpdate basic)
        {
            return new GeographyUpdate(basic);
        }

        public Country Country
        {
            get
            {
                if (string.IsNullOrEmpty (base.CountryCode))
                {
                    return null;
                }

                return Country.FromCode (base.CountryCode);
            }
        }

        public static GeographyUpdate Create (string updateType, string updateSource, string guid, string countryCode, string changeDataXml, DateTime effectiveDateTime)
        {
            int identity = SwarmDb.GetDatabaseForWriting()
                .CreateGeographyUpdate (updateType, updateSource, guid, countryCode, changeDataXml, DateTime.UtcNow,
                    effectiveDateTime);
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetGeographyUpdate (identity));  // "ForWriting" intentional to avoid db replication race conditions
        }

        public new bool Processed
        {
            get { return base.Processed; }
            set
            {
                if (value != base.Processed && value)
                {
                    SwarmDb.GetDatabaseForWriting().SetGeographyUpdateProcessed (this.Identity);
                    base.Processed = true;
                }
            }
        }

        public void Run()
        {
            // Run this geography update and apply it to the database.

            if (DateTime.UtcNow < EffectiveDateTime)
            {
                // we shouldn't process this yet - return unprocessed
                return; // DESIGN: throw exception instead?
            }

            switch (UpdateType)
            {
                case "PrimeCountry":
                    PrimeCountry (CountryCode); // static fn so needs to pass country code

                    break;
                case "SetPostalCodeData":
                    break;
                case "SetCountryPosition":
                    break;
                default:
                    throw new NotImplementedException("Unimplemented geography update type: " + UpdateType);
            }

            Processed = true;
        }

        public static void PrimeCountry (string countryCode)
        {
            Country country = Country.FromCode (countryCode);
            if (country.GeographyId != Geography.RootIdentity)
            {
                // already initialized
                return;
            }

            GeographyDataLoader geoDataLoader = new GeographyDataLoader();

            MasterGeography geography = geoDataLoader.GetGeographyForCountry(countryCode);
            MasterCity[] cities = geoDataLoader.GetCitiesForCountry(countryCode);
            MasterPostalCode[] postalCodes = geoDataLoader.GetPostalCodesForCountry(countryCode);

            // ID Translation lists

            Dictionary<int, int> geographyIdTranslation = new Dictionary<int, int>();
            Dictionary<int, int> cityIdTranslation = new Dictionary<int, int>();
            Dictionary<int, bool> cityIdsUsedLookup = new Dictionary<int, bool>();

            // Create the country's root geography

            int countryRootGeographyId = SwarmDb.GetDatabaseForWriting().CreateGeography(geography.Name,
                Geography.RootIdentity);
            geographyIdTranslation[geography.GeographyId] = countryRootGeographyId;
            SwarmDb.GetDatabaseForWriting().SetCountryGeographyId(country.Identity,
                countryRootGeographyId);

            int count = 0;
            int total = InitDatabaseThreadCountGeographyChildren(geography.Children);

            InitDatabaseThreadCreateGeographyChildren(geography.Children, countryRootGeographyId,
                ref geographyIdTranslation, ref count, total);

            // Find which cities are actually used

            foreach (MasterPostalCode postalCode in postalCodes)
            {
                cityIdsUsedLookup[postalCode.CityId] = true;
            }

            GuidCache.Set ("DbInitProgress", "(finalizing)");

            // Insert cities

            int newCountryId = country.Identity;

            int cityIdHighwater =
                SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommandScalar("SELECT Max(CityId) FROM Cities;");

            StringBuilder sqlCityBuild =
                new StringBuilder("INSERT INTO Cities (CityName, GeographyId, CountryId, Comment) VALUES ", 65536);
            bool insertComma = false;

            foreach (MasterCity city in cities)
            {
                if (!geographyIdTranslation.ContainsKey(city.GeographyId))
                {
                    cityIdsUsedLookup[city.CityId] = false; // force non-use of invalid city
                }

                if (cityIdsUsedLookup[city.CityId])
                {
                    int newGeographyId = geographyIdTranslation[city.GeographyId];

                    if (insertComma)
                    {
                        sqlCityBuild.Append(",");
                    }

                    sqlCityBuild.Append("('" + city.Name.Replace("'", "\\'") + "'," + newGeographyId + "," +
                                         newCountryId + ",'')");
                    insertComma = true;

                    cityIdTranslation[city.CityId] = ++cityIdHighwater; // Note that we assume the assigned ID here.
                }
            }

            sqlCityBuild.Append(";");

            SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlCityBuild.ToString());
            // Inserts all cities in one bulk op, to save roundtrips

            // Insert postal codes, if any

            if (postalCodes.Length > 0)
            {
                StringBuilder sqlBuild =
                    new StringBuilder ("INSERT INTO PostalCodes (PostalCode, CityId, CountryId) VALUES ", 65536);
                insertComma = false;

                foreach (MasterPostalCode postalCode in postalCodes)
                {
                    if (cityIdsUsedLookup[postalCode.CityId] == false)
                    {
                        // Remnants of invalid pointers

                        continue;
                    }

                    int newCityId = cityIdTranslation[postalCode.CityId];

                    if (insertComma)
                    {
                        sqlBuild.Append (",");
                    }

                    sqlBuild.Append ("('" + postalCode.PostalCode.Replace ("'", "\\'") + "'," + newCityId + "," +
                                     newCountryId + ")");
                    insertComma = true;
                }

                sqlBuild.Append (";");

                // Insert all postal codes in one bulk op, to save roundtrips
                SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlBuild.ToString());
            }
        }


        private static int InitDatabaseThreadCountGeographyChildren(MasterGeography[] children)
        {
            int count = 0;

            foreach (MasterGeography child in children)
            {
                count++;
                count += InitDatabaseThreadCountGeographyChildren(child.Children);
            }

            return count;
        }



        private static void InitDatabaseThreadCreateGeographyChildren(MasterGeography[] children,
            int parentGeographyId,
            ref Dictionary<int, int> geographyIdTranslation, ref int count, int total)
        {
            count++;

            foreach (MasterGeography geography in children)
            {
                int newGeographyId = SwarmDb.GetDatabaseForWriting().CreateGeography(geography.Name, parentGeographyId);
                geographyIdTranslation[geography.GeographyId] = newGeographyId;

                InitDatabaseThreadCreateGeographyChildren(geography.Children, newGeographyId, ref geographyIdTranslation,
                    ref count, total);
            }

            if (total != 0)
            {
                GuidCache.Set("DbInitProgress", String.Format ("({0}%)", count * 100 / total));
                // shouldn't be here but wtf
            }
        }

    }
}
