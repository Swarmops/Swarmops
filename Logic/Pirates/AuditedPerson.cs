using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Pirates
{
    public class AuditedPerson 
    {
        public static implicit operator Person (AuditedPerson a)
        {
            return a.person;
        }


        public static AuditedPerson FromPerson (Person person)
        {
            return new AuditedPerson(person);
        }

        public static AuditedPerson FromPerson (Person person, Person executor, string actionDescr)
        {
            AuditedPerson ap= new AuditedPerson(person);
            ap.SetAudit(executor, actionDescr);
            return ap;
        }

        protected AuditedPerson (Person person)
        {
            this.person = person;
            this.executor = null;
            this.actionDescr = "";
        }

        public void SetAudit (Person executor, string actionDescr)
        {
            this.executor = executor;
            this.actionDescr = actionDescr;
        }

        protected void AuditField (string field, string oldVal, string newVal)
        {
            if (executor != null && (""+oldVal).Trim() != (""+newVal).Trim())
            {
                PWLog.Write(executor, PWLogItem.Person, this.person.Identity, PWLogAction.PersonFieldChange, actionDescr, "", field, ""+oldVal, ""+newVal);
            }
        }

        public virtual string Name
        {
            get { return person.Name; }
            set
            {
                AuditField("Name", Name, value);
                person.Name = value;
            }
        }


        public virtual string Street
        {
            get { return person.Street; }
            set
            {
                AuditField("Street", Street, value);
                person.Street = value;
            }
        }


        public virtual string PostalCode
        {
            get { return person.PostalCode; }
            set
            {
                AuditField("PostalCode", PostalCode, value);
                person.PostalCode = value;
            }
        }


        public virtual string City
        {
            get { return person.CityName; }
            set
            {
                AuditField("City", City, value);
                person.CityName = value;
            }
        }


        public virtual Country Country
        {
            get { return person.Country; }
            set
            {
                AuditField("Country", Country == null ? "" : Country.Identity.ToString(), value == null ? "" : value.Identity.ToString());
                person.Country = value;
            }
        }


        public virtual Geography Geography
        {
            get { return person.Geography; }
            set
            {
                AuditField("Geography", Geography == null ? "" : Geography.Identity.ToString(), value == null ? "" : value.Identity.ToString());
                person.Geography = value;
            }
        }


        public virtual string Email
        {
            get { return person.Email; }
            set
            {
                AuditField("Email", Email, value);
                person.Email = value;
            }
        }


        public virtual string Phone
        {
            get { return person.Phone; }
            set
            {
                AuditField("Phone", Phone, value);
                person.Phone = value;
            }
        }


        public virtual DateTime Birthdate
        {
            get { return person.Birthdate; }
            set
            {
                AuditField("Birthdate", Birthdate.ToString(), value.ToString());
                person.Birthdate = value;
            }
        }

        public virtual PersonGender Gender
        {
            get { return person.Gender; }
            set
            {
                AuditField("Gender", Gender.ToString(), value.ToString());
                person.Gender = value;
            }
        }


        // Optional data properties

        public virtual string PartyEmail
        {
            get { return person.PartyEmail; }
            set
            {
                AuditField("PartyEmail", PartyEmail, value);
                person.PartyEmail = value;
            }
        }


        public virtual string BankName
        {
            get { return person.BankName; }
            set
            {
                AuditField("BankName", BankName, value);
                person.BankName = value;
            }
        }


        public virtual string BankAccount
        {
            get { return person.BankAccount; }
            set
            {
                AuditField("BankAccount", BankAccount, value);
                person.BankAccount = value;
            }
        }


        public virtual string BankClearing
        {
            get { return person.BankClearing; }
            set
            {
                AuditField("BankClearing", BankClearing, value);
                person.BankClearing = value;
            }
        }


        public virtual string PersonalNumber
        {
            get { return person.PersonalNumber; }
            set
            {
                AuditField("PersonalNumber", PersonalNumber, value);
                person.PersonalNumber = value;
            }
        }

        public virtual bool MailUnreachable
        {
            get { return person.MailUnreachable; }
            set
            {
                AuditField("MailUnreachable", MailUnreachable.ToString(), value.ToString());
                person.MailUnreachable = value;
            }
        }

        public virtual bool LimitMailToLatin1
        {
            get { return person.LimitMailToLatin1; }
            set
            {
                AuditField("LimitMailToLatin1", LimitMailToLatin1.ToString(), value.ToString());
                person.LimitMailToLatin1 = value;
            }
        }

        public virtual bool LimitMailToText
        {
            get { return person.LimitMailToText; }
            set
            {
                AuditField("LimitMailToText", LimitMailToText.ToString(), value.ToString());
                person.LimitMailToText = value;
            }
        }

        public virtual bool NeverMail
        {
            get { return person.NeverMail; }
            set
            {
                AuditField("NeverMail", NeverMail.ToString(), value.ToString());
                person.NeverMail = value;
            }
        }

        public virtual bool EMailIsInvalid
        {
            get { return person.EMailIsInvalid; }
            set
            {
                AuditField("EMailIsInvalid", EMailIsInvalid.ToString(), value.ToString());
                person.EMailIsInvalid = value;
            }
        }


        public virtual string Handle
        {
            get { return person.Handle; }
            set
            {
                AuditField("Handle", Handle, value);
                person.Handle = value;
            }
        }

        public virtual int SwedishForumAccountId
        {
            get { return person.SwedishForumAccountId; }
            set
            {
                AuditField("SwedishForumAccountId", SwedishForumAccountId.ToString(), value.ToString());
                person.SwedishForumAccountId = value;
            }
        }


        public virtual void SetPassword (string newPassword)
        {
            AuditField("SetPassword", "oldpassword", "newpassword");
            person.SetPassword(newPassword);
        }

        public virtual void SetSubscription (int newsletterFeedId, bool subscribe)
        {
            AuditField("SetSubscription", "Feed:" + newsletterFeedId, "" + subscribe);
            person.SetSubscription(newsletterFeedId, subscribe);
        }


        private Person executor = null;
        private string actionDescr = "";
        private Person person;
    }
}
