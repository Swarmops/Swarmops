<%@ WebHandler Language="C#" Class="SMSReceiver" %>

using System;
using System.Collections.Generic;
using System.Web;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Communications;
using Activizr.Logic.Support;

public class SMSReceiver : IHttpHandler
{

    public void ProcessRequest (HttpContext context)
    {
        if (context.IsDebuggingEnabled)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("\nPhone number: " + context.Request["phoneNumber"] + "\n");
            context.Response.Write("Received at: " + context.Request["receivedAt"] + "\n");
            context.Response.Write("Message: " + context.Request["message"] + "\n");
        }

        if ((context.Request["phoneNumber"] != null)
            || (context.Request["receivedAt"] != null)
            || (context.Request["message"] != null))
        {
            try
            {
                DateTime sentAt = DateTime.MinValue;
                if (!DateTime.TryParse("" + context.Request["receivedAt"], out sentAt))
                {
                    Double unixTS = 0;
                    double.TryParse("" + context.Request["receivedAt"], out unixTS);
                    sentAt = ConvertFromUnixTimestamp(unixTS);
                    if (sentAt.AddDays(50) < DateTime.Now || sentAt > DateTime.Now.AddDays(1))
                    {
                        throw new Exception("Failed to decode receivedAt");
                    }
                }

                PhoneMessageReceiver.Handle(context.Request["phoneNumber"].Trim(), context.Request["message"].Trim(), sentAt);
            }
            catch (Exception ex)
            {
                PWLog.Write(PWLogItem.None, 0, PWLogAction.Failure, "Failed to handle SMS reply",
                            "Phone number: " + context.Request["phoneNumber"] +
                             " Received at: " + context.Request["receivedAt"] +
                             " Message: " + context.Request["message"] + "\n" + ex.ToString());

                context.Response.Write("");
                context.Response.End();
            }
        }
    }
    static DateTime ConvertFromUnixTimestamp (double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timestamp);
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
