using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic
{
    public class Config
    {
        static public int SmtpPort
        {
            get
            {
                string portString = ConfigurationSettings.AppSettings["SmtpPort"];

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
                string smtpHost = ConfigurationSettings.AppSettings["SmtpHost"];

                if (String.IsNullOrEmpty(smtpHost))
                {
                    return "piratesmtp";
                }

                return smtpHost;
            }
        }
    }
}
