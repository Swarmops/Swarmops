using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Communications
{
    public class PhoneMessageTransmitter
    {
        private static ServiceCredential credential; // credentials cache
        private static DateTime lastLoad = DateTime.MinValue;
        private static object locker = new object();

        private static ServiceCredential Credential
        {
            get
            {
                lock (locker)
                {
                    if (credential == null || DateTime.Now.Subtract(lastLoad).TotalSeconds > 30)
                    {
                        lastLoad = DateTime.Now;
                        String serviceName = Persistence.Key["SMSService"];
                        BasicExternalCredential basicCredential = SwarmDb.GetDatabaseForReading().GetExternalCredential(serviceName);
                        Encoding enc = Encoding.GetEncoding(28591); //ISO encoding (latin1)

                        credential = new ServiceCredential(basicCredential.Login, basicCredential.Password, basicCredential.ServiceName, enc);
                    }

                    return credential;
                }
            }
        }

        public static double SMSCost
        {
            get
            {
                switch (Credential.ServiceName)
                {
                    case "SwedishSms":
                        return 0.63;
                    case "SwedishKannel":
                        return 0.03; //Guess
                    default:
                        throw new Exception("Error on SMSCost. Unknown SMS service: " + Credential.ServiceName);
                }
            }
        }

        internal static void Send (string phoneNumber, string message)
        {
            switch (Credential.ServiceName)
            {
                case "SwedishSms":
                    SendMoSMS(phoneNumber, message);
                    break;
                case "SwedishKannel":
                    SendKannel(phoneNumber, message);
                    break;
                default:
                    throw new Exception("Error on SMS transmit. Unknown SMS service: " + Credential.ServiceName);
            }
        }

        public static bool CheckServiceStatus ()
        {
            switch (Credential.ServiceName)
            {
                case "SwedishSms":
                    return true; //cant test this

                case "SwedishKannel":
                    {
                        String username = Credential.Login;
                        String password = Credential.Password;
                        Encoding enc = Credential.Encoding;
                        String url = "https://nurse.sanitarium.se/kannel/status";
                        String result;

                        HttpWebResponse responseOut = null;
                        try
                        {
                            result = HTTPSender.Send(url, enc, 1500, out responseOut);
                            if (((int)responseOut.StatusCode).ToString().StartsWith("2"))
                                return true;
                            else
                                return false;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                default:
                    throw new Exception("Error on SMS transmit. Unknown SMS service: " + Credential.ServiceName);
            }
        }

        public static void SendKannel (string phoneNumber, string message)
        {

            //This is the swedish HOMEGROWN SMS sending mechanisma
            String encodedMessage = HttpUtility.UrlEncode(message, Credential.Encoding);
            String encodedPhone = NormalizePhoneNumber(phoneNumber, Credential.Encoding);

            String username = Credential.Login;
            String password = Credential.Password;
            Encoding enc = Credential.Encoding;
            String url = "https://nurse.sanitarium.se/sms/sendsms";
            String call_string;
            String result;

            call_string = url +
                          "?username=" + username +
                          "&password=" + password +
                          "&to=" + encodedPhone +
                          "&text=" + encodedMessage +
                          "&charset=latin1";

            result = HTTPSender.Send(call_string, enc);

            if (result != "0: Accepted for delivery")
            {
                throw new Exception("Error on SMS transmit: phone number " + phoneNumber + ", error code " + result);
            }
        }

        internal static void SendMoSMS (string phone, string message)
        {
            // This is the SWEDISH sms transmission mechanism.

            // Sätt användarnamn, lösenord och URL till MO-SMS.
            String mosms_username = Credential.Login;
            String mosms_password = Credential.Password;
            Encoding enc = Credential.Encoding;
            String mosms_url = "http://www.mosms.com/se/sms-send.php";
            String call_string;
            String result;

            // Sätt mottagarens telefonnummer
            String mosms_number = phone;

            // Sätt vilken typ av SMS som skall skickas.
            String mosms_type = "text";

            String encodedMessage = HttpUtility.UrlEncode(message, Credential.Encoding);

            // Sätt SMS-meddelandet som skall skickas
            String mosms_data = encodedMessage;
            mosms_data = HttpUtility.UrlEncode(mosms_data, enc);

            call_string = mosms_url +
                          "?username=" + mosms_username +
                          "&password=" + mosms_password +
                          "&nr=" + mosms_number +
                          "&type=" + mosms_type +
                          "&data=" + mosms_data;

            result = HTTPSender.Send(call_string, enc);
            result = HttpUtility.UrlDecode(result, enc);

            if (result != "0")
            {
                throw new Exception("Error on SMS transmit: phone number " + phone + ", error code " + result);
            }
        }

        // A phone number should be in the format: 
        //   +<countrycode><phoneNumber without leading 0>
        // Also URL encode as the + is treated as a space if not encoded
        internal static string NormalizePhoneNumber (string phoneNumber, Encoding enc)
        {
            phoneNumber = phoneNumber.Trim();
            if (phoneNumber.Length > 4 && !phoneNumber.StartsWith("+"))
            {
                if (phoneNumber.StartsWith("00"))
                {
                    phoneNumber = "+" + phoneNumber.Substring(2);
                }
                else if (phoneNumber.StartsWith("0"))
                {
                    phoneNumber = "+46" + phoneNumber.Substring(1);
                }
                else
                {
                    phoneNumber = "+" + phoneNumber;
                }
            }

            phoneNumber = HttpUtility.UrlEncode(phoneNumber, enc);

            return phoneNumber;
        }

        public static string[] DeNormalizedPhoneNumber (string phoneNumber)
        {
            string[] phoneNumbers = new string[3];

            Match match = Regex.Match(phoneNumber, @"^(((00|\+)46)|0)(7[0236]\d+)$");
            if (match.Success)
            {
                GroupCollection groups = match.Groups;
                int i = groups.Count - 1;
                phoneNumbers[0] = "0" + groups[i];
                phoneNumbers[1] = "0046" + groups[i];
                phoneNumbers[2] = "46" + groups[i]; // PW Doesn't save the + so don't search for it
            }
            else
                phoneNumbers[0] = phoneNumber;

            return phoneNumbers;
        }


        #region HTTP Sending Class


        private class HTTPSender : ICertificatePolicy
        {

            public bool CheckValidationResult (ServicePoint sp, X509Certificate certificate, WebRequest request, int error)
            {
                return true;
            }


            public static string Send (string http_payload, Encoding enc)
            {
                HttpWebResponse responseOut = null;
                return Send(http_payload, enc, -1, out responseOut);
            }


            public static string Send (string http_payload, Encoding enc, int timeout, out HttpWebResponse responseOut)
            {
                // variabel för indata
                var sb = new StringBuilder();

                // buffer för inläsning
                var buf = new byte[8092];

#pragma warning disable 618
                // ServicePointManager.CertificatePolicy is obsoleted, but is the only thing that works on both windows and mono.
                //Needed to avoid errors from homegrown SSL cert.
                
                ICertificatePolicy oldPolicy = ServicePointManager.CertificatePolicy;
                ServicePointManager.CertificatePolicy = new HTTPSender();

                // anropa websidan med aktuella data
                var request = (HttpWebRequest)WebRequest.Create(http_payload);

                if (timeout > -1)
                {
                    request.Timeout = timeout;
                }

                // ta emot svarskoder
                var response = (HttpWebResponse)request.GetResponse();
                responseOut = response;
                ServicePointManager.CertificatePolicy = oldPolicy;
#pragma warning restore 618

                // läs in dataströmmen via responsmetoden
                Stream resStream = response.GetResponseStream();
                string tempString = null;
                int count = 0;

                do
                {
                    // fyll bufferten
                    count = resStream.Read(buf, 0, buf.Length);
                    // kontrollera att data finns att läsa
                    if (count != 0)
                    {
                        // koda som ISO text
                        tempString = enc.GetString(buf, 0, count);

                        // bygg upp svarssträngen
                        sb.Append(tempString);
                    }
                } while (count > 0); // finns mer data att läsa?

                return tempString;
            }
        }

        #endregion
        #region Nested type: ServiceCredential

        internal class ServiceCredential
        {
            internal readonly string Login;
            internal readonly string Password;
            internal readonly string ServiceName;
            internal readonly Encoding Encoding;

            internal ServiceCredential (string login, string password, string serviceName, Encoding enc)
            {
                this.Login = login;
                this.Password = password;
                this.ServiceName = serviceName;
                this.Encoding = enc;
            }
        }

        #endregion
    }
}