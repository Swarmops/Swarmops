using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Swarmops.Basic.Enums;

namespace Swarmops.Logic.Security
{
    public class PermissionSet
    {
        public class Item
        {
            public Item (int pOrgId, int pGeoId, Permission pPerm)
            {
                orgId = pOrgId;
                geographyId = pGeoId;
                perm = pPerm;
            }
            public Item (int pOrgId,  Permission pPerm)
            {
                orgId = pOrgId;
                geographyId = -1;
                perm = pPerm;
            }
            public Item (Permission pPerm)
            {
                orgId = -1;
                geographyId = -1;
                perm = pPerm;
            }

            public int orgId
            { get; private set; }

            public int geographyId
            { get; private set; }

            public Permission perm
            { get; private set; }

        }
        public List<PermissionSet.Item> permsList = new List<PermissionSet.Item>();

        private bool needAll = false;

        public bool NeedAll
        {
            get { return needAll; }
            set { needAll = value; }
        }

        public bool NeedOne
        {
            get { return !needAll; }
            set { needAll = !value; }
        }


        public PermissionSet (Permission p)
        {
            permsList.Add(new Item(-1, -1, p));
        }
        public PermissionSet (Permission p,int orgId)
        {
            permsList.Add(new Item(orgId , -1, p));
        }
        public PermissionSet (Permission p, int orgId, int geoId)
        {
            permsList.Add(new Item(orgId , geoId , p));
        }

        public PermissionSet (PermissionSet.Item pi)
        {
            permsList.Add(pi);
        }

        public PermissionSet (string permsString)
        {
            Regex re = new Regex(
@"          # will search Permission(1,1),Permission(1,1),Permission
(                   # group for each ,Permission(1,1) or ,Permission
    (                   
        (               # Permission(1)
            (?<perm>\w+)
            (
                \(             # Permission(
                (?<params>          # 1,1 or 1
                    (.+?)((,(.+?))*?)
                )
                \)                  # )
            ){0,1}
        )
    )
    ,*                  # group could end with comma
)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            MatchCollection matches = re.Matches(permsString);
            foreach (Match m in matches)
            {
                try
                {
                    int org = -1;
                    int geo = -1;
                    string hit = m.Groups["perm"].Value;
                    try
                    {
                        string pars = m.Groups["params"].Value;
                        string[] split = pars.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (pars.Length > 0)
                        {
                            org = int.Parse(split[0]);
                            if (pars.Length > 1)
                                geo = int.Parse(split[1]);
                        }
                    }
                    catch
                    { }
                    Permission p = (Permission)Enum.Parse(typeof(Permission), hit, true);
                    PermissionSet.Item item = new Item(org, geo, p);
                    permsList.Add(item);
                }
                catch
                { }
            }
        }

        public bool IsInSet (Permission p)
        {
            foreach (PermissionSet.Item ps in permsList)
                if (ps.perm == p)
                    return true;
            return false;
        }

        public bool IsInSet (PermissionSet ps)
        {
            foreach (Permission p in ps)
                if (!this.IsInSet(p))
                    return false;
            return true;
        }

        public bool IsAnyInSet (PermissionSet ps)
        {
            foreach (Permission p in ps)
                if (this.IsInSet(p))
                    return true;
            return false;
        }

        public override string ToString ()
        {
            List<string> reslist = new List<string>(); 
            foreach (PermissionSet.Item itm in this)
            {
                reslist.Add(itm.perm.ToString());
            }
            string[] resArr = reslist.ToArray();
            string res = string.Join(needAll ? " & " : " | ", resArr);
            return res;
        }

        public Enumerator GetEnumerator ()
        {
            return new Enumerator(this);
        }

        public class Enumerator : IEnumerator
        {
            int currentIndex = -1;
            PermissionSet parent;

            internal Enumerator (PermissionSet p)
            {
                this.parent = p;
                this.Reset();
            }

            public object Current
            {
                get
                {
                    if (currentIndex < 0 || currentIndex > parent.permsList.Count - 1)
                        return null;
                    return parent.permsList[currentIndex];
                }
            }

            public bool MoveNext ()
            {
                ++currentIndex;

                if (Current != null)
                    return true;
                else
                    return false;
            }

            public void Reset ()
            {
                currentIndex = -1;
            }
        }
    }
}
