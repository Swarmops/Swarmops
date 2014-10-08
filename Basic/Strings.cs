namespace Swarmops.Basic
{
    public class Strings
    {
        public static string MailSenderName
        {
            get
            {
                if (System.IO.Path.DirectorySeparatorChar == '/')
                {
                    return "PirateWeb";
                }
                else
                {
                    return "PirateWeb-W";
                }
            }
        }

        public static string MailSenderAddress
        {
            get { return "noreply@pirateweb.net"; }
        }

        public static string MailSenderNameFinancial
        {
            get { return "PirateWeb Financials"; }
        }

        public static string MailSenderNameHumanResources
        {
            get { return "PirateWeb Human Resources"; }
        }

        public static string MailSenderNameServices
        {
            get { return "PirateWeb Services"; }
        }

        public static string InternationalCultureCode
        {
            get { return "en-US"; }
        }
    }
}