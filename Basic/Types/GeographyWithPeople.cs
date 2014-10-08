using System;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicGeographyWithPeople 
    {
        public int GeographyID;
        public int OrgId = 0;
        public string GeograpyName;
        public string OrgName;
        public int ParentGeographyId;
        public int ParentOrgId;
        public int MemberCount;
        public int ActivistCount;
        public string LeadContent;
        public string SecondsContent;
    }
}