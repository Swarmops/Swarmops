using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Swarmops.Basic.Enums;

namespace Swarmops.Logic.Security
{
    public class PermissionSet
    {
        private bool needAll;
        public List<Item> permsList = new List<Item>();


        public PermissionSet (Permission p)
        {
            this.permsList.Add (new Item (-1, -1, p));
        }

        public PermissionSet (Permission p, int orgId)
        {
            this.permsList.Add (new Item (orgId, -1, p));
        }

        public PermissionSet (Permission p, int orgId, int geoId)
        {
            this.permsList.Add (new Item (orgId, geoId, p));
        }

        public PermissionSet (Item pi)
        {
            this.permsList.Add (pi);
        }

        public PermissionSet (string permsString)
        {
            Regex re = new Regex (
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
            MatchCollection matches = re.Matches (permsString);
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
                        string[] split = pars.Split (new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                        if (pars.Length > 0)
                        {
                            org = int.Parse (split[0]);
                            if (pars.Length > 1)
                                geo = int.Parse (split[1]);
                        }
                    }
                    catch
                    {
                    }
                    Permission p = (Permission) Enum.Parse (typeof (Permission), hit, true);
                    Item item = new Item (org, geo, p);
                    this.permsList.Add (item);
                }
                catch
                {
                }
            }
        }

        public bool NeedAll
        {
            get { return this.needAll; }
            set { this.needAll = value; }
        }

        public bool NeedOne
        {
            get { return !this.needAll; }
            set { this.needAll = !value; }
        }

        public bool IsInSet (Permission p)
        {
            foreach (Item ps in this.permsList)
                if (ps.perm == p)
                    return true;
            return false;
        }

        public bool IsInSet (PermissionSet ps)
        {
            foreach (Permission p in ps)
                if (!IsInSet (p))
                    return false;
            return true;
        }

        public bool IsAnyInSet (PermissionSet ps)
        {
            foreach (Permission p in ps)
                if (IsInSet (p))
                    return true;
            return false;
        }

        public override string ToString()
        {
            List<string> reslist = new List<string>();
            foreach (Item itm in this)
            {
                reslist.Add (itm.perm.ToString());
            }
            string[] resArr = reslist.ToArray();
            string res = string.Join (this.needAll ? " & " : " | ", resArr);
            return res;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator (this);
        }

        public class Enumerator : IEnumerator
        {
            private readonly PermissionSet parent;
            private int currentIndex = -1;

            internal Enumerator (PermissionSet p)
            {
                this.parent = p;
                Reset();
            }

            public object Current
            {
                get
                {
                    if (this.currentIndex < 0 || this.currentIndex > this.parent.permsList.Count - 1)
                        return null;
                    return this.parent.permsList[this.currentIndex];
                }
            }

            public bool MoveNext()
            {
                ++this.currentIndex;

                if (Current != null)
                    return true;
                return false;
            }

            public void Reset()
            {
                this.currentIndex = -1;
            }
        }

        public class Item
        {
            public Item (int pOrgId, int pGeoId, Permission pPerm)
            {
                orgId = pOrgId;
                geographyId = pGeoId;
                perm = pPerm;
            }

            public Item (int pOrgId, Permission pPerm)
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

            public int orgId { get; private set; }

            public int geographyId { get; private set; }

            public Permission perm { get; private set; }
        }
    }
}