using System;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class GeographyWithPeople
    {
        public int ActivistCount;
        public int GeographyID;
        public string LeadContent;
        public int MemberCount;
        public string Name;
        public int OrgId = 0;
        public int ParentGeographyId;

        public string SecondsContent;


        public static int RootIdentity
        {
            get { return 1; } // The identity of the root geography (i.e., "World")
        }
    }
}