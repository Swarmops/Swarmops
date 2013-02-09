using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Swarm
{
    public class ExternalIdentity : BasicExternalIdentity
    {

        private Person attachedToPerson;

        private ExternalIdentity (BasicExternalIdentity basic)
            : base(basic)
        {
        }


        public new int ExternalIdentityIdentity
        {
            get
            {
                return base.ExternalIdentityIdentity;
            }
            private set
            {
                throw new InvalidOperationException("Do not set individual fields of ExternalIdentity separately. Use SetExternalIdentity() method.");
            }
        }

        public new string ExternalSystem
        {
            get
            {
                return base.ExternalSystem;
            }
            private set
            {
                throw new InvalidOperationException("Do not set individual fields of ExternalIdentity separately. Use SetExternalIdentity() method.");
            }
        }
        public new string UserID
        {
            get
            {
                return base.UserID;
            }
            private set
            {
                throw new InvalidOperationException("Do not set individual fields of ExternalIdentity separately. Use SetExternalIdentity() method.");
            }
        }
        public new string Password
        {
            get
            {
                return base.Password;
            }
            private set
            {
                throw new InvalidOperationException("Do not set individual fields of ExternalIdentity separately. Use SetExternalIdentity() method.");
            }
        }
        public Person AttachedToPerson
        {
            get
            {
                if (attachedToPerson == null)
                    attachedToPerson = Person.FromIdentity(base.AttachedToPersonID);
                return attachedToPerson;
            }
            private set
            {
                throw new InvalidOperationException("Do not set individual fields of ExternalIdentity separately. Use SetExternalIdentity() method.");
            }
        }
        public new ExternalIdentityType TypeOfAccount
        {
            get
            {
                return base.TypeOfAccount;
            }
            private set
            {
                throw new InvalidOperationException("Do not set individual fields of ExternalIdentity separately. Use SetExternalIdentity() method.");
            }
        }


        public static ExternalIdentity FromBasic (BasicExternalIdentity basic)
        {
            return new ExternalIdentity(basic);
        }

        public static ExternalIdentity FromIdentity (int externalIdentityId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetExternalIdentity(externalIdentityId));
        }

        public static ExternalIdentity FromUserIdAndType (string userid, ExternalIdentityType type)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetExternalIdentityFromUserIdAndType(userid, type));
        }

        public static ExternalIdentity FromPersonIdAndType (int persId, ExternalIdentityType type)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetExternalIdentityFromPersonIdAndType(persId, type));
        }

        public static List<ExternalIdentity> ExternalItentitiesForPerson (int persId)
        {
            List<BasicExternalIdentity> templist = SwarmDb.GetDatabaseForReading().GetExternalIdentities(persId);

            List<ExternalIdentity> retlist = new List<ExternalIdentity>();

            foreach (BasicExternalIdentity ext in templist)
            {
                retlist.Add(FromBasic(ext));
            }

            return retlist;

        }


        public ExternalIdentity SetExternalIdentity (string ExternalSystem,
                                        string UserID,
                                        string Password,
                                        int AttachedToPerson,
                                        ExternalIdentityType TypeOfAccount)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().SetExternalIdentity(
                this.Identity, TypeOfAccount, ExternalSystem,
                UserID, Password, AttachedToPerson));



        }
        public static ExternalIdentity CreateExternalIdentity (string ExternalSystem,
                                        string UserID,
                                        string Password,
                                        int AttachedToPerson,
                                        ExternalIdentityType TypeOfAccount)
        {
            try
            {
                return FromBasic(SwarmDb.GetDatabaseForWriting().SetExternalIdentity(
                    0, TypeOfAccount, ExternalSystem,
                    UserID, Password, AttachedToPerson));
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to create ExternalIdentity", ex);
            }


        }

    }
}
