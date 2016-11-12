<%@ WebHandler Language="C#" Class="LoginXMPP" %>

using System;
using System.Web;
using PirateWeb.Logic.Pirates;
using PirateWeb.Basic.Enums;

public class LoginXMPP : IHttpHandler
{

    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Charset = "utf-8";
        
        string user = ("" + context.Request["user"]).Trim();
        string domain = ("" + context.Request["domain"]).Trim();
        
        string jid = user + "@" + domain;
        
        ExternalIdentity extID = null;
        try
        {
            extID = ExternalIdentity.FromUserIdAndType(jid, ExternalIdentityType.PPXMPPAccount);
        }
        catch 
        {
            People hits = People.FromOptionalData(ObjectOptionalDataType.PartyEmail, jid);
            if (hits.Count == 1)
            {
                extID = ExternalIdentity.FromPersonIdAndType(hits[0].Identity , ExternalIdentityType.PPXMPPAccount);
            }   
        }       

        if (extID == null)
        {
            context.Response.StatusCode = 400;
            return;
        }
        
        context.Response.StatusCode = 200;

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}