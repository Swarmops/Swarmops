namespace Swarmops.Utility.Server
{
    /// <summary>
    ///     Summary description for ExchangeCode.
    /// </summary>
    public class ExchangeCode
    {
        /*

         * 
         * ENTIRE CLASS DEACTIVATED -- Windows emigration in progress -- no more Exchange in server park
         * 
         * 
		public ExchangeCode()
		{
		}

		
		public static void CreateMailbox(Person person)
		{
			string homeMDB = "CN=Mailbox Store (HOOK),CN=First Storage Group,"
				+ "CN=InformationStore,CN=HOOK,CN=Servers,"
				+ "CN=First Administrative Group,CN=Administrative Groups,"
				+ "CN=PirateWeb,CN=Microsoft Exchange,CN=Services,"
				+ "CN=Configuration,DC=internal,DC=pirateweb,DC=net";

			DirectoryEntry user;
			CDOEXM.IMailboxStore mailbox;

			//This creates the new user in the "users" container.
			//Set the sAMAccountName and the password
			user = new DirectoryEntry("LDAP://" + ActiveDirectoryCode.GetPersonDistinguishedName(person));

			//Obtain the IMailboxStore interface, create the mailbox, and commit the changes.
			mailbox = (IMailboxStore)user.NativeObject;
			mailbox.CreateMailbox(homeMDB);
			user.CommitChanges();
		}

		
		public static bool EmailAddressExistInLdap(string mailAddress)
		{
			return EmailAddressExistInLdap(mailAddress, "LDAP://" + ActiveDirectoryCode.RootDse);
		}

		
		public static bool EmailAddressExistInLdap(string strMailAddress, string strRootDSE)
		{
			// Set the return value to True    
			// True means that the Active Directory would NOT be updated and
			// I prefer the default to be "don't do"     

			bool bRetVal = true;

			// Set a objSearch starting at RootDSE, and a place to return it.

			System.DirectoryServices.DirectorySearcher objSearch = new System.DirectoryServices.DirectorySearcher(strRootDSE);
			System.DirectoryServices.SearchResult objResult;

			// Filter only on the proxyAddress

			objSearch.Filter = "(& ( proxyAddresses=*" + strMailAddress + "*))";

			// if we even find one, we can't add another.
			// This is a slow way to look, but
			// it is better than having two Exchange Proxy
			// Address's and getting NDR's.

			objResult = objSearch.FindOne();

			if (objResult == null)
				bRetVal = false;
			return bRetVal;
		} */
    }
}