using System;

using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Interfaces;
using System.Collections.Generic;

namespace Activizr.Logic.Structure
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