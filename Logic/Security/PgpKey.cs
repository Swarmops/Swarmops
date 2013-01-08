using System;
using System.Collections.Generic;
using System.Text;
using Starksoft.Cryptography.OpenPGP;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Security
{
    public class PgpKey
    {
        static public void Generate (Person person, Organization organization)
        {
            GnuPG generatingInstance = new GnuPG();
            generatingInstance.Timeout = 120000;
            generatingInstance.GenerateKeyPair(person.Name, person.PartyEmail, organization.Name,
                                        new DateTime(DateTime.Today.Year + 2, 12, 31));

            GnuPGKeyCollection keys = new GnuPG().GetKeys();
            foreach (GnuPGKey key in keys)
            {
                if (key.UserId == person.PartyEmail)
                {
                    // Console.Write(" signing...");
                    generatingInstance.SignKey(key.Fingerprint, "system@pirateweb.net");
                    // Console.Write(" uploading...");
                    generatingInstance.UploadKey(key.Fingerprint.Replace(" ", ""));
                    string armorSecret = new GnuPG().GetSecretKey(key.Fingerprint.Replace(" ", ""));
                    string armorPublic = new GnuPG().GetPublicKey(key.Fingerprint.Replace(" ", ""));
                    // Console.Write(" deleting...");
                    key.Delete();

                    person.CryptoPublicKey = armorPublic;
                    person.CryptoSecretKey = armorSecret;
                    person.CryptoFingerprint = key.Fingerprint;
                }
            }

        }

        public string Fingerprint { get; private set; }
    }
}
