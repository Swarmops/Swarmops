using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.DataObjects
{

#if !__MonoCS__
    [DataObject(true)]
#endif
    public class OrganizationsDataObject
    {
        public class Org : Organization
        {
            private string parentName;
            private string anchorName;
            private string countryCode;

            public Org ()
                : base()
            {
            }

            public Org (Organization org)
                : base(org)
            {
                try
                {
                    parentName = Organization.FromIdentity(ParentOrganizationId).Name;
                }
                catch
                {
                    parentName = "N/A";
                }
                try
                {
                    anchorName = Geography.FromIdentity(AnchorGeographyId).Name;
                }
                catch
                {
                    anchorName = "N/A";
                }
                try
                {
                    countryCode = base.DefaultCountry.Code;
                }
                catch
                {
                    countryCode = "N/A";
                }
            }

            #region Properties for fields
            public new bool AcceptsMembers
            {
                get
                {
                    return base.AcceptsMembers;
                }
                set
                {
                    base.AcceptsMembers = value;
                }
            }
            public new int AnchorGeographyId
            {
                get
                {
                    return base.AnchorGeographyId;
                }
                set
                {
                    base.AnchorGeographyId = value;
                }
            }
            public new bool AutoAssignNewMembers
            {
                get
                {
                    return base.AutoAssignNewMembers;
                }
                set
                {
                    base.AutoAssignNewMembers = value;
                }
            }
            public new string Domain
            {
                get
                {
                    return base.Domain;
                }
                set
                {
                    base.Domain = value;
                }
            }
            public new string MailPrefix
            {
                get
                {
                    return base.MailPrefix;
                }
                set
                {
                    base.MailPrefix = value;
                }
            }
            public new string Name
            {
                get
                {
                    return base.Name;
                }
                set
                {
                    base.Name = value;
                }
            }
            public new string NameInternational
            {
                get
                {
                    return base.NameInternational;
                }
                set
                {
                    base.NameInternational = value;
                }
            }
            public new int Identity
            {
                get
                {
                    return base.OrganizationId;
                }
                set
                {
                    base.OrganizationId = value;
                }
            }
            public new string NameShort
            {
                get
                {
                    return base.NameShort;
                }
                set
                {
                    base.NameShort = value;
                }
            }
            public new int ParentOrganizationId
            {
                get
                {
                    return base.ParentOrganizationId;
                }
                set
                {
                    base.ParentOrganizationId = value;
                }
            }

            public new int DefaultCountryId
            {
                get
                {
                    return base.DefaultCountryId;
                }
                set
                {
                    base.DefaultCountryId = value;
                }
            }
            #endregion
            public string ParentName
            {
                get
                {
                    return parentName;
                }
                set
                {
                    parentName = value;
                }
            }
            public string AnchorName
            {
                get
                {
                    return anchorName;
                }
                set
                {
                    anchorName = value;
                }
            }
            public string CountryCode
            {
                get
                {
                    if ("" + countryCode == "")
                        countryCode = base.DefaultCountry.Code;
                    return countryCode;
                }
                set
                {
                    countryCode = value;
                }
            }
        }

        public class UptakeGeography : Structure.UptakeGeography
        {
            public UptakeGeography ()
                : base()
            {
            }
            
            public UptakeGeography (Structure.UptakeGeography other)
                : base(other)
            {
            
            }

            internal static UptakeGeography fromUptake (Structure.UptakeGeography up)
            {
                return new UptakeGeography(up);
            }
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select, true)]
#endif
        public static Org[] Select ()
        {
            List<Org> res = new List<Org>();

            Organizations orgs = Organizations.GetAll();
            foreach (Organization org in orgs)
            {
                res.Add(new Org(org));
            }

            return res.ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select, true)]
#endif
        public static Org Select (int orgid)
        {
            try
            {
                return new Org(Organization.FromIdentity(orgid));
            }
            catch
            {
                return null;
            }
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static Organization[] Select (int[] OrganizationIds)
        {
            if (OrganizationIds == null)
            {
                return new Organization[0];
            }

            return Organizations.FromIdentities(OrganizationIds).ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static Organization[] SelectStatic (Organization[] OrganizationArray)
        {
            if (OrganizationArray == null)
            {
                return new Organization[0];
            }

            return OrganizationArray;
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static Organization[] SelectStatic (Organizations Organizations)
        {
            if (Organizations == null)
            {
                return new Organization[0];
            }

            return Organizations.ToArray();
        }


#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static Organization[] SelectSortedStatic (Organizations Organizations, string sort)
        {
            if (Organizations == null)
            {
                return new Organization[0];
            }


            List<Organization> pList = new List<Organization>();
            Organization[] foundOrganizations = SelectStatic((Organizations)Organizations);
            foreach (Organization pers in foundOrganizations)
            {
                pList.Add(pers);
            }

            switch (sort)
            {
                case "OrganizationId": pList.Sort(IdentityComparison); break;
            }

            return pList.ToArray();
        }





#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
#endif
        public int AddOrganization (Org org)
        {
            try
            {
                org.DefaultCountryId = Country.FromCode(org.CountryCode).CountryId;
            }
            catch
            {
                org.DefaultCountryId = FindDefaultCountry(org);
            }
            org.Identity = Organization.Create(org.ParentOrganizationId, org.NameInternational,
                org.Name, org.NameShort, org.Domain, org.MailPrefix, org.AnchorGeographyId, org.AcceptsMembers,
                org.AutoAssignNewMembers, org.DefaultCountryId).Identity;
            // Return true if precisely one row was inserted, otherwise false
            return org.Identity;
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Update, false)]
#endif
        public int AddDuplicateOrganization (Org org)
        {

            AddOrganization(org);
            return org.Identity;
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Update, true)]
#endif
        public bool UpdateOrganisation (Org org)
        {
            try
            {
                try
                {
                    org.DefaultCountryId = Country.FromCode(org.CountryCode).CountryId;
                }
                catch
                {
                    org.DefaultCountryId = FindDefaultCountry(org);
                }
                Organization.UpdateOrganization(org.ParentOrganizationId, org.NameInternational,
                    org.Name, org.NameShort, org.Domain, org.MailPrefix, org.AnchorGeographyId, org.AcceptsMembers,
                    org.AutoAssignNewMembers, org.DefaultCountryId,
                    org.Identity);

                return true;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Failed Update:" + ex.Message);
                throw new Exception("UpdateOrganisation failed because:" + ex.Message);
            }
        }

        private static int FindDefaultCountry (Org org)
        {
            //Find default country = Country of anchor.
            Geographies line = Geography.FromIdentity(org.AnchorGeographyId).GetLine();
            Geography[] arrLine = line.ToArray();
            Array.Reverse(arrLine);
            Countries countries = Countries.GetAll();
            int foundCntry = 1; //sweden
            foreach (Geography geo in arrLine)
            {
                if (geo.Identity == 1)
                    return foundCntry; //Didnt find any, we are at world.
                foreach (Country cntry in countries)
                {
                    if (geo.Identity == cntry.GeographyId)
                    {
                        foundCntry = cntry.CountryId;
                        return foundCntry;
                    }
                }
            }
            return foundCntry; //Didnt find any
        }


        public static Comparison<Organization> IdentityComparison = delegate(Organization p1, Organization p2)
        {
            return p1.OrganizationId.CompareTo(p2.OrganizationId);
        };


        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        
        private static UptakeGeography[] UptakeFromArray(Structure.UptakeGeography[] inputArray)
        {
            List<UptakeGeography> retlist=new List<UptakeGeography>();
        
            foreach(Structure.UptakeGeography up in inputArray)
            {
            retlist.Add(UptakeGeography.fromUptake(up));
            }
            return retlist.ToArray();
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select, false)]
#endif
        public static UptakeGeography[] SelectOrgMineUptake (int orgid)
        {
            try
            {
                return UptakeFromArray(Organization.FromIdentity(orgid).GetUptakeGeographies(false));
            }
            catch
            {
                return null;
            }
        }
#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select, false)]
#endif
        public static UptakeGeography[] SelectOrgOthersUptake (int orgid)
        {
            try
            {
                return UptakeFromArray(Organization.FromIdentity(orgid).GetUptakeGeographies(true));
            }
            catch
            {
                return null;
            }
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select, false)]
#endif
        public static UptakeGeography[] SelectOrgOthersUptakeForGeo (int orgid, int geoid)
        {
            try
            {
                List<UptakeGeography> filtered = new List<UptakeGeography>();
                UptakeGeography[] unfiltered = UptakeFromArray(Organization.FromIdentity(orgid).GetUptakeGeographies(true));
                foreach (UptakeGeography bu in unfiltered)
                {
                    if (bu.GeoId == geoid)
                        filtered.Add(bu);
                }
                return filtered.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static void UpdateOrgUptake (int orgid, int[] newUptakeGeos)
        {
            try
            {
                Organization org = Organization.FromIdentity(orgid);
                List<int> inOld = new List<int>();
                foreach (Geography oldGeo in org.UptakeGeographies)
                {
                    if (Array.IndexOf(newUptakeGeos, oldGeo.Identity) > -1)
                    {
                        inOld.Add(oldGeo.Identity);
                    }
                    else
                    {
                        org.DeleteUptakeGeography(oldGeo.Identity);
                    }
                }
                foreach (int newgeo in newUptakeGeos)
                {
                    if (!inOld.Contains(newgeo))
                    {
                        org.AddUptakeGeography(newgeo);
                    }
                }
            }
            catch (Exception)
            {
            }
        }


#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
#endif
        public bool DeleteUptake (UptakeGeography ut)
        {
            Organization org = Organization.FromIdentity(ut.OrgId);
            org.DeleteUptakeGeography(ut.GeoId);

            return true;
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Insert, false)]
#endif
        public bool AddUptake (UptakeGeography ut)
        {
            Organization org = Organization.FromIdentity(ut.OrgId);
            org.AddUptakeGeography(ut.GeoId);

            // Return true if precisely one row was inserted, otherwise false
            return true;
        }

    }


}