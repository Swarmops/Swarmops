using System.Collections.Generic;
namespace Swarmops.Basic.Enums
{
    public enum MailAuthorType
    {
        Unknown = 0,
        Person = 1,
        MemberService = 2,
        PirateWeb = 3,
        ActivistService = 4,
        Service = 5,
        PressService = 6,
        Accounting = 7
    }

    public class FunctionalMail
    {
        public class AddressItem
        {
            public AddressItem (string email, string name)
            {
                this.email = email;
                this.name = name;
            }
            private string email;
            private string name;
            public string Email
            {
                get
                {
                    return email;
                }
            }
            public string Name
            {
                get
                {
                    return name;
                }
            }
        }

        static public Dictionary<MailAuthorType, AddressItem> Address = initList();
        public static Dictionary<MailAuthorType, AddressItem> initList ()
        {
            Dictionary<MailAuthorType, AddressItem> retval = new Dictionary<MailAuthorType, AddressItem>();
            retval.Add(MailAuthorType.MemberService, new AddressItem("medlemsservice@piratpartiet.se", "Piratpartiet Medlemsservice"));
            retval.Add(MailAuthorType.PirateWeb, new AddressItem("noreply@pirateweb.net", "PirateWeb"));
            retval.Add(MailAuthorType.ActivistService, new AddressItem("aktivistservice@piratpartiet.se", "Piratpartiet Aktivistservice"));
            retval.Add(MailAuthorType.Service, new AddressItem("service@piratpartiet.se", "Piratpartiet Service"));
            retval.Add(MailAuthorType.PressService, new AddressItem("press@piratpartiet.se", "Piratpartiet Press"));
            retval.Add(MailAuthorType.Accounting, new AddressItem("accounting@piratpartiet.se", "Piratpartiet Accounting"));
            return retval;
        }


    }
}