using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Security;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Security
{
    public class Authority
    {
        public static Authority FromXml (string xml)
        {
            return new Authority (AuthorityData.FromXml (xml));
        }

        public static Authority FromEncryptedXml (string cryptXml)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(SystemSettings.InstallationId.Replace("-", ""));  // unique for this install
            byte[] cryptoBytes = Convert.FromBase64String(cryptXml);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = keyBytes;
                aes.IV = new ArraySegment<byte> (cryptoBytes, 0, 16).ToArray();
                cryptoBytes = new ArraySegment<byte>(cryptoBytes, 16, cryptoBytes.Length - 16).ToArray();

                using (ICryptoTransform crypto = aes.CreateDecryptor())
                {
                    byte[] clearBytes = crypto.TransformFinalBlock(cryptoBytes, 0, cryptoBytes.Length);
                    return FromXml(Encoding.UTF8.GetString (clearBytes));
                }
            }
        }

        public string ToXml()
        {
            return _data.ToXml();
        }

        public string ToEncryptedXml()
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(SystemSettings.InstallationId.Replace ("-",""));  // unique for this install
            byte[] dataBytes = Encoding.UTF8.GetBytes (ToXml());

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = keyBytes;
                aes.GenerateIV(); // unique for every encryption

                if (aes.IV.Length != 16)
                {
                    throw new InvalidDataException("IV is not 16 bytes long");
                }

                using (ICryptoTransform crypto = aes.CreateEncryptor())
                {
                    byte[] cryptoBytes = crypto.TransformFinalBlock (dataBytes, 0, dataBytes.Length);
                    return Convert.ToBase64String (aes.IV.Concat (cryptoBytes).ToArray()); // joins two byte[] arrays
                }
            }
        }

        private Authority (AuthorityData data)
        {
            this._data = data;
        }

        public static Authority FromLogin (Person person)
        {
            int lastOrgId = person.LastLogonOrganizationId;
            PositionAssignment assignment = null;

            if (lastOrgId != 0)
            {
                Organization organization = Organization.FromIdentity (lastOrgId);
                assignment = person.GetPrimaryAssignment (organization);
            }

            // TODO: Verify membership OR position OR volunteer

            return new Authority (new AuthorityData
            {
                CustomData = new Basic.Types.Common.SerializableDictionary<string, string>(),
                LoginDateTimeUtc = DateTime.UtcNow,
                OrganizationId = lastOrgId,
                PersonId = person.Identity,
                PositionAssignmentId = (assignment != null ? assignment.Identity : 0)
            });
        }

        private readonly AuthorityData _data;





        public bool CanSeePerson (Person person)
        {
            People initialList = People.FromSingle (person);

            throw new NotImplementedException();

            /*People filteredList =  Authorization.FilterPeopleToMatchAuthority (initialList, this);

            if (filteredList.Count == 0)
            {
                return false;
            }

            return true;*/
        }


        
    }

    [Serializable]
    public class AuthorityData
    {
        public int PersonId { get; set; }
        public int OrganizationId { get; set; }
        public int PositionAssignmentId { get; set; }
        public DateTime LoginDateTimeUtc { get; set; }
        public Basic.Types.Common.SerializableDictionary<string,string> CustomData { get; set; }

        internal string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(GetType());

            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.UTF8.GetString(xmlBytes);
        }

        static internal AuthorityData FromXml (string xml)
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith("?"))
            {
                xml = xml.Substring(1);
            }

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            XmlSerializer serializer = new XmlSerializer(typeof(AuthorityData));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            AuthorityData result = (AuthorityData)serializer.Deserialize(stream);
            stream.Close();

            return result;
        }
    }
}