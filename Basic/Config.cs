using System;
using System.Configuration;

namespace Swarmops.Basic
{
    public class Config
    {
        static public int SmtpPort
        {
            get
            {
                string portString = ConfigurationManager.AppSettings["SmtpPort"];

                if (String.IsNullOrEmpty(portString))
                {
                    return 587;
                }

                return Int32.Parse(portString);
            }
        }

        static public string SmtpHost
        {
            get
            {
                string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];

                if (String.IsNullOrEmpty(smtpHost))
                {
                    return "piratesmtp";
                }

                return smtpHost;
            }
        }
    }
}
