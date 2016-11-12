using System;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using PirateWeb.Interface.Localization;
using PirateWeb.Logic.Security;
using PirateWeb.Basic.Types;
using System.Text.RegularExpressions;

public partial class Security_SubSystemLogin : System.Web.UI.Page
{

    private object lockObject = new object();
    private static Dictionary<string, DateTime> lastcallers = new Dictionary<string, DateTime>();
    string redirectURL = "";
    string redirectURLParams = "";

    protected void Page_Load (object sender, EventArgs e)
    {
        Response.Expires = -1;
        
        bool localCall = false;
        string[] local = (Request.ServerVariables["LOCAL_ADDR"]).Split(new char[] { '.' });
        string remoteaddr = (Request.ServerVariables["REMOTE_ADDR"]);
        string[] remote = remoteaddr.Split(new char[] { '.' });
        if (local[0] == remote[0] && local[1] == remote[1])
            localCall = true;

        redirectURL = Request["redirect"] != null ? Request["redirect"].ToString() : "";
        redirectURLParams = "";
        foreach (string s in Request.Form.Keys)
        {
            if (s != "redirect" && s != "username" && s != "password")
                redirectURLParams += "&" + s + "=" + Server.UrlEncode(Request.Form[s]);
        }

        string loginToken = Request["username"] != null ? Request["username"].ToString() : "";
        string password = Request["password"] != null ? Request["password"].ToString() : "";
        lock (lockObject)
        {
            if ((localCall == false) && lastcallers.ContainsKey(remoteaddr) && lastcallers[remoteaddr] > DateTime.Now)
            {
                lastcallers[remoteaddr] = DateTime.Now.AddSeconds(10);
                DoFail("4", "EPIC FAIL");
            }
            else
            {
                lastcallers[remoteaddr] = DateTime.Now.AddSeconds(4);
                if (!string.IsNullOrEmpty(loginToken.Trim()) && !string.IsNullOrEmpty(password.Trim()))
                {
                    try
                    {
                        BasicPerson authenticatedPerson = PirateWeb.Logic.Security.Authentication.Authenticate(loginToken, password);
                        if (authenticatedPerson == null) throw new Exception("Wrong UserId/Password");
                        string ticket = Convert.ToBase64String(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(Guid.NewGuid().ToByteArray()));
                        Regex re = new Regex(@"[^a-z0-9]", RegexOptions.IgnoreCase);
                        ticket = re.Replace(ticket,"");
                        InternalLoginTicket ticketObj = new InternalLoginTicket();
                        ticketObj.created = DateTime.Now;
                        ticketObj.validatedUserID = authenticatedPerson.Identity;

                        Dictionary<string, InternalLoginTicket> openTickets = new Dictionary<string, InternalLoginTicket>();

                        Application.Lock();
                        if (Application["SubSystem_LoginTickets"] != null)
                        {
                            openTickets = (Dictionary<string, InternalLoginTicket>)Application["SubSystem_LoginTickets"];
                            List<string> toremove = new List<string>();
                            foreach (string key in openTickets.Keys)
                            {
                                if (openTickets[key].created.AddMinutes(2) < DateTime.Now)
                                    toremove.Add(key);
                            }

                            foreach (string key in toremove)
                            {
                                openTickets.Remove(key);
                            }
                        }
                        openTickets.Add(ticket, ticketObj);
                        Application["SubSystem_LoginTickets"] = openTickets;
                        Application.UnLock();

                        Response.Redirect(redirectURL + "?result=success&ticket=" + Server.UrlEncode(ticket.ToString()) + redirectURLParams, false);
                    }
                    catch (Exception exc)
                    {
                        DoFail("1", exc.Message);
                    }
                }
                else
                {
                    DoFail("2", "Invalid Parameter");
                }
            }

            if (lastcallers.Count > 10)
            {
                string[] keys = (new List<string>(lastcallers.Keys)).ToArray();
                foreach (string key in keys)
                {
                    if (lastcallers[key] < DateTime.Now.AddMinutes(-5))
                    {
                        lastcallers.Remove(key);
                    }
                }
            }
        }
        Session.Abandon();
    }

    protected void DoFail (string errno, string msg)
    {
        Response.Redirect(redirectURL + "?result=fail&reason=" + errno + "&msg=" + Server.UrlEncode(msg) + redirectURLParams, false);
    }
}
