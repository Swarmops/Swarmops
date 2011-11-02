using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Xml;

public partial class Pages_Security_InternalLogin : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (Request.QueryString["ticket"] != null && hTicket.Value == "")
        {
            //this is the callback
            this.hTicket.Value = Request.QueryString["ticket"].ToString();
            LoginPanel.Visible = false;

           HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://pirateweb.net/Pages/Security/ValidateTicket.aspx?ticket=" + Request.QueryString["ticket"].ToString());

             //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://localhost:2900/Pages/Security/ValidateTicket.aspx?ticket=" + Request.QueryString["ticket"].ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string tmp = reader.ReadToEnd();
            response.Close();

            Response.ContentType = "text/xml";
            Response.Clear();
            Response.Write(tmp);
            Response.End();

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(tmp);
            ResultPanel.Visible = true;
            resultdiv.InnerHtml = "";
            if (xdoc.SelectSingleNode("//USER") != null)
            {
                resultdiv.InnerHtml += xdoc.SelectSingleNode("//USER/ID").InnerText;
                resultdiv.InnerHtml += "<BR>";
                resultdiv.InnerHtml += xdoc.SelectSingleNode("//USER/NAME").InnerText;
                resultdiv.InnerHtml += "<BR>";
                resultdiv.InnerHtml += xdoc.SelectSingleNode("//USER/GEOGRAPHIESFORPERSON/GEOGRAPHY").Attributes["name"];
                resultdiv.InnerHtml += "<BR>";
            }
            else
            {
                resultdiv.InnerHtml=xdoc.InnerText;
            }
        }
        else if (this.hTicket.Value != "")
        {
            //the callback has been done we are running normally logged in

        }
        else
        {
            // prepare for login
            LoginPanel.Visible = true;
        }
    }
}
