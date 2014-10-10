namespace Swarmops.Utility.Server
{
    public class ActiveDirectoryCode
    {
        /*
             ENTIRE CLASS DEACTIVATED -- Windows migration in progress -- no AD left
          
         
		static public string GetADAccountName(Person person)
		{
			string result = string.Empty;

			string[] nameParts = person.Name.Split(" .-".ToCharArray());

			foreach (string part in nameParts)
			{
				if (part.Length > 0)
				{
					char firstChar = Char.ToLowerInvariant (part[0]);

					switch (firstChar)
					{
						case 'å':
						case 'ä':
							firstChar = 'a';
							break;
						case 'e':
							firstChar = 'e';
							break;
						case 'ö':
							firstChar = 'o';
							break;
					}

					if (Char.IsLetter(firstChar))
					{
						result += firstChar;
					}

				}
			}

			result += "-" + person.Identity.ToString();

			return result;
		}



		static public void AddEmailAddress(Person person, string address, bool primary)
		{
			DirectoryEntry personEntry = new DirectoryEntry("LDAP://" + GetPersonDistinguishedName (person));

			PropertyValueCollection addressCollection = personEntry.Properties["proxyAddresses"];
			
			// TODO: check to see if the address is already there.

			if (addressCollection == null)
			{
				throw new Exception("Could not open proxyaddress key");
			}

			string prefix = "smtp:";

			if (primary)
			{
				prefix = prefix.ToUpper();
			}

			addressCollection.Add(prefix + address);

			if (primary)
			{
				personEntry.Properties["Mail"].Value = address;
			}

			personEntry.CommitChanges();
		}


		static public string RootDse
		{
			get
			{
				return "OU=Pirates,OU=Hosted,DC=internal,DC=pirateweb,DC=net";
			}
		}


		static public string GetPersonDistinguishedName(Person person)
		{
			return GetPersonContainerName(person) + ",OU=Accounts,OU=Pirates,OU=Hosted,DC=internal,DC=pirateweb,DC=net";
		}


		static public string GetPersonContainerName(Person person)
		{
			return "CN=" + person.Name.Trim() + " (" + person.Country.Code + "-" + person.Identity.ToString() + ")";
		}



		static public void AddUserToPirateSecurityGroup(Person person, Organization organization)
		{
			DirectoryEntry piratesGroup = new DirectoryEntry("LDAP://CN=" + organization.Name + ",OU=Security Groups,OU=Pirates,OU=Hosted,DC=internal,DC=pirateweb,DC=net");
			piratesGroup.Invoke("Add", "LDAP://" + GetPersonDistinguishedName(person));
		}


		static public void AddUser(Person person, Organization organization, string password)
		{
			DirectoryEntry obDirEntry = null;
			try
			{
				obDirEntry = new DirectoryEntry("LDAP://OU=Accounts,OU=Pirates,OU=Hosted,DC=internal,DC=pirateweb,DC=net");
				DirectoryEntries entries = obDirEntry.Children;
				DirectoryEntry newUser = entries.Add(GetPersonContainerName(person), "User");
				newUser.Properties["DisplayName"].Value = person.Name.Trim() + " (" + organization.MailPrefixInherited + ")";
				newUser.Properties["sAMAccountName"].Value = ActiveDirectoryCode.GetADAccountName(person);
				newUser.Properties["Mail"].Value = person.PPMailAddress;
				newUser.Properties["TelephoneNumber"].Value = person.Phone;
				
				newUser.CommitChanges();

				object obRet = newUser.Invoke("SetPassword", password);
				newUser.Properties["UserAccountControl"].Value = 0x200 + 0x10000;
				newUser.CommitChanges();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				throw ex;
			}
		}



		static public void SetUserPassword(Person person, string newPassword)
		{
			DirectoryEntry personEntry = new DirectoryEntry("LDAP://" + GetPersonDistinguishedName(person));
			personEntry.Invoke("SetPassword", newPassword);
		} */
    }
}