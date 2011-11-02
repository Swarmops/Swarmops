using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Support;

public partial class Controls_v4_DocumentList : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Todo: read this from language resource

        string alphabetPrimer =
            "Alpha|Bravo|Charlie|Delta|Echo|Foxtrot|Golf|Hotel|India|Juliet|Kilo|Lima|Mike|November|Oscar|Papa|Quebec|Romeo|Sierra|Tango|Uniform|Victor|Whiskey|X-ray|Yankee|Zulu";

        alphabet = alphabetPrimer.Split('|');

        this.LiteralDocList.Text = "(unprimed)";

    }

    protected void Page_PreRender (object sender, EventArgs e)
    {
        if (Documents == null)
        {
            this.LiteralDocList.Text = "(null)";
            return;
        }

        if (Documents.Count == 0)
        {
            this.LiteralDocList.Text = "none.";
            return;
        }

        int count = Documents.Count;

        string result = string.Empty;

        for (int index = 0; index < count; index++)
        {
            if (index > 0 && (count > 2 || useShortForm))
            {
                result += ", ";
            }

            if (index == count - 1 && count > 1 && !useShortForm)
            {
                result += " and ";
            }

            result += GetDocumentLink(Documents[index], index+1); // improve
        }

        this.LiteralDocList.Text = result;
    }


    protected string GetDocumentLink (Document doc, int count)
    {
        string name = Server.HtmlEncode(doc.ClientFileName); // TODO: prioritize description

        if (UseShortForm)
        {
            name = alphabet[count - 1];
        }

        return "<a href=\"/Pages/v4/Support/DownloadDocument.aspx?DocumentId=" + doc.Identity.ToString() +"\" target=\"_blank\">" + name + "</a>"; // TODO: Fix link
    }

    public Documents Documents;

    private string[] alphabet;

    private bool useShortForm;

// ReSharper disable ConvertToAutoProperty
    public bool UseShortForm
// ReSharper restore ConvertToAutoProperty
    {
        set { this.useShortForm = value; }
        get { return this.useShortForm; }
    }
}
