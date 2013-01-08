using System;

using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Interfaces;
using System.Collections.Generic;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class GeographyWithPeople 
    {


        public int GeographyID;
        public int OrgId=0;
        public string Name;
        public int ParentGeographyId;

        public int MemberCount;
        public int ActivistCount;
        public string LeadContent;
        public string SecondsContent;


        public static int RootIdentity
        {
            get { return 1; } // The identity of the root geography (i.e., "World")
        }
 
    }
}