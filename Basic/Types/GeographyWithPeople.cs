using System;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicGeographyWithPeople
    {
        public int ActivistCount;
        public int GeographyID;
        public string GeograpyName;
        public string LeadContent;
        public int MemberCount;
        public int OrgId = 0;
        public string OrgName;
        public int ParentGeographyId;
        public int ParentOrgId;
        public string SecondsContent;
    }
}