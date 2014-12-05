using System;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class AuditedPerson
    {
        private readonly Person person;
        private string actionDescr = "";
        private Person executor;

        protected AuditedPerson (Person person)
        {
            this.person = person;
            this.executor = null;
            this.actionDescr = "";
        }

        public virtual string Name
        {
            get { return this.person.Name; }
            set
            {
                AuditField ("Name", Name, value);
                this.person.Name = value;
            }
        }


        public virtual string Street
        {
            get { return this.person.Street; }
            set
            {
                AuditField ("Street", Street, value);
                this.person.Street = value;
            }
        }


        public virtual string PostalCode
        {
            get { return this.person.PostalCode; }
            set
            {
                AuditField ("PostalCode", PostalCode, value);
                this.person.PostalCode = value;
            }
        }


        public virtual string City
        {
            get { return this.person.CityName; }
            set
            {
                AuditField ("City", City, value);
                this.person.CityName = value;
            }
        }


        public virtual Country Country
        {
            get { return this.person.Country; }
            set
            {
                AuditField ("Country", Country == null ? "" : Country.Identity.ToString(),
                    value == null ? "" : value.Identity.ToString());
                this.person.Country = value;
            }
        }


        public virtual Geography Geography
        {
            get { return this.person.Geography; }
            set
            {
                AuditField ("Geography", Geography == null ? "" : Geography.Identity.ToString(),
                    value == null ? "" : value.Identity.ToString());
                this.person.Geography = value;
            }
        }


        public virtual string Email
        {
            get { return this.person.Mail; }
            set
            {
                AuditField ("Email", Email, value);
                this.person.Mail = value;
            }
        }


        public virtual string Phone
        {
            get { return this.person.Phone; }
            set
            {
                AuditField ("Phone", Phone, value);
                this.person.Phone = value;
            }
        }


        public virtual DateTime Birthdate
        {
            get { return this.person.Birthdate; }
            set
            {
                AuditField ("Birthdate", Birthdate.ToString(), value.ToString());
                this.person.Birthdate = value;
            }
        }

        public virtual PersonGender Gender
        {
            get { return this.person.Gender; }
            set
            {
                AuditField ("Gender", Gender.ToString(), value.ToString());
                this.person.Gender = value;
            }
        }


        // Optional data properties

        public virtual string PartyEmail
        {
            get { return this.person.PartyEmail; }
            set
            {
                AuditField ("PartyEmail", PartyEmail, value);
                this.person.PartyEmail = value;
            }
        }


        public virtual string BankName
        {
            get { return this.person.BankName; }
            set
            {
                AuditField ("BankName", BankName, value);
                this.person.BankName = value;
            }
        }


        public virtual string BankAccount
        {
            get { return this.person.BankAccount; }
            set
            {
                AuditField ("BankAccount", BankAccount, value);
                this.person.BankAccount = value;
            }
        }


        public virtual string BankClearing
        {
            get { return this.person.BankClearing; }
            set
            {
                AuditField ("BankClearing", BankClearing, value);
                this.person.BankClearing = value;
            }
        }

        [Obsolete ("Use NationalIdNumber instead")]
        public virtual string PersonalNumber
        {
            get { return this.person.PersonalNumber; }
            set
            {
                AuditField ("PersonalNumber", PersonalNumber, value);
                this.person.PersonalNumber = value;
            }
        }

        public virtual bool MailUnreachable
        {
            get { return this.person.MailUnreachable; }
            set
            {
                AuditField ("MailUnreachable", MailUnreachable.ToString(), value.ToString());
                this.person.MailUnreachable = value;
            }
        }

        public virtual bool LimitMailToLatin1
        {
            get { return this.person.LimitMailToLatin1; }
            set
            {
                AuditField ("LimitMailToLatin1", LimitMailToLatin1.ToString(), value.ToString());
                this.person.LimitMailToLatin1 = value;
            }
        }

        public virtual bool LimitMailToText
        {
            get { return this.person.LimitMailToText; }
            set
            {
                AuditField ("LimitMailToText", LimitMailToText.ToString(), value.ToString());
                this.person.LimitMailToText = value;
            }
        }

        public virtual bool NeverMail
        {
            get { return this.person.NeverMail; }
            set
            {
                AuditField ("NeverMail", NeverMail.ToString(), value.ToString());
                this.person.NeverMail = value;
            }
        }

        public virtual bool EMailIsInvalid
        {
            get { return this.person.EMailIsInvalid; }
            set
            {
                AuditField ("EMailIsInvalid", EMailIsInvalid.ToString(), value.ToString());
                this.person.EMailIsInvalid = value;
            }
        }


        public virtual string Handle
        {
            get { return this.person.Handle; }
            set
            {
                AuditField ("Handle", Handle, value);
                this.person.Handle = value;
            }
        }

        public virtual int SwedishForumAccountId
        {
            get { return this.person.SwedishForumAccountId; }
            set
            {
                AuditField ("SwedishForumAccountId", SwedishForumAccountId.ToString(), value.ToString());
                this.person.SwedishForumAccountId = value;
            }
        }

        public static implicit operator Person (AuditedPerson a)
        {
            return a.person;
        }


        public static AuditedPerson FromPerson (Person person)
        {
            return new AuditedPerson (person);
        }

        public static AuditedPerson FromPerson (Person person, Person executor, string actionDescr)
        {
            AuditedPerson ap = new AuditedPerson (person);
            ap.SetAudit (executor, actionDescr);
            return ap;
        }

        public void SetAudit (Person executor, string actionDescr)
        {
            this.executor = executor;
            this.actionDescr = actionDescr;
        }

        protected void AuditField (string field, string oldVal, string newVal)
        {
            if (this.executor != null && ("" + oldVal).Trim() != ("" + newVal).Trim())
            {
                PWLog.Write (this.executor, PWLogItem.Person, this.person.Identity, PWLogAction.PersonFieldChange,
                    this.actionDescr, "", field, "" + oldVal, "" + newVal);
            }
        }


        public virtual void SetPassword (string newPassword)
        {
            AuditField ("SetPassword", "oldpassword", "newpassword");
            this.person.SetPassword (newPassword);
        }

        public virtual void SetSubscription (int newsletterFeedId, bool subscribe)
        {
            AuditField ("SetSubscription", "Feed:" + newsletterFeedId, "" + subscribe);
            this.person.SetSubscription (newsletterFeedId, subscribe);
        }
    }
}