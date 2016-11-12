<%@ Page Language="C#" %>
<%@ Import Namespace="Activizr.Logic.Pirates"%>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="Activizr.Basic.Enums" %>


<%
/*    
------------------------------------------------------------------------------
RECEIVE_SMS.ASPX -- code sample from MOSMS.com
------------------------------------------------------------------------------
*/
 
 
 
    NameValueCollection parameters = Request.QueryString;
    String phoneNumber;
    String sms;
    String tariff;
    Encoding enc = Encoding.GetEncoding(28591); //ISO encoding

    Response.ContentEncoding = Encoding.Default;
    Response.Charset = "iso-8859-1";
    Response.ContentType = "text/plain";
    
    // Plocka ut avsändarnumret
    phoneNumber = parameters["nr"];
 
    // Plocka ut SMS-meddelandet
    sms = HttpUtility.UrlDecode(parameters["sms"], enc);
 
    // Plocka ut priset slutanvändaren blev debiterad (för exempelvis egen statistik)
    tariff = parameters["tariff"];
    
    // Find the user. Was a member number supplied?

    string[] smsParts = sms.Replace ("  ", " ").Trim().Split(' ');
    
    int personId = 0;

    if (smsParts.Length > 2)
    {
        try
        {
            personId = Int32.Parse(smsParts[2]);
        }
        catch (Exception)
        {
            if (smsParts[2].ToLower() == "test")
            {
                Response.Write("R\xE4ksm\xF6rg\xE5s R\xC4KSM\xD6RG\xC5S ");
            }
        }
    }
    else
    {
        // No person id was supplied, so attempt to deduce from phone number

        People people = People.FromPhoneNumber("SE", phoneNumber);

        if (people.Count == 1)
        {
            // Exactly one hit means we can safely use this id

            personId = people[0].PersonId;
        }
        else if (people.Count > 1)
        {
            
        }
    }

    if (personId != 0)
    {
        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.ReceivedMembershipPayment, personId, Activizr.Logic.Structure.Organization.PPSEid, 0, personId, 0, sms.Replace(" ", "_") + "/" + phoneNumber);

        // Skicka ett svar tillbaka till slutanvändaren
        Response.Write("Tack f\xF6r din betalning f\xF6r " + DateTime.Today.Year.ToString() + "! Det kommer strax ett e-mail som bekr" + "\xE4" + "ftar.");
    }
    else
    {
        Response.Write("Tyv\xE4rr f\xF6rstod vi inte vem som betalade avgiften. Kontakta Medlemsservice.");
    }
 
/*    
------------------------------------------------------------------------------
RECEIVE_SMS.ASPX     END SCRIPT
------------------------------------------------------------------------------
*/
        
%>