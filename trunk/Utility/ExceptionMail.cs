using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Swarm;
using Swarmops.Basic;
using Swarmops.Database;
using Swarmops.Utility.Mail;

namespace Swarmops.Utility
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