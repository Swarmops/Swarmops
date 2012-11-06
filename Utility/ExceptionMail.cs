using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic;
using Activizr.Database;
using Activizr.Utility.Mail;
using Activizr.Logic.Pirates;

namespace Activizr.Utility
{
    public class ExceptionMail
    {
        public static void Send (Exception e)
        {
            ExceptionMail.Send(e, false);
        }

        public static void Send (Exception e, bool logOnly)
        {
            try
            {
                PirateDb.GetDatabaseForWriting().CreateExceptionLogEntry(DateTime.Now, "ExceptionMail", e);
            }
            catch
            {
            }

            if (!logOnly)
            {
                new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                           "PirateWeb EXCEPTION!",
                                                           e.ToString(), People.FromIdentities(new int[] { 1 }), false).Send();
            }
        }
    }
}