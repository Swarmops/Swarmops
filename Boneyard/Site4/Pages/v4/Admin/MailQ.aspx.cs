using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Database;
using System.Data.Common;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using Activizr.Logic.Communications;


public partial class Pages_v4_admin_MailQ : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        BuildTable();

    }
    protected void Button1_Click (object sender, EventArgs e)
    {
        BuildTable();

    }

    private void BuildTable ()
    {
        tab.Rows.Clear();

        OutboundMails mails = OutboundMails.FromArray(PirateDb.GetDatabase().GetOutboundMailQueue(500));

        TableRow row = new TableRow();
        tab.Rows.Add(row);
        TableCell c = new TableCell(); row.Cells.Add(c);

        c.Text = "Template";

        c = new TableCell(); row.Cells.Add(c);
        c.Text = "Title";

        c = new TableCell(); row.Cells.Add(c);
        c.Text = "Release time";

        c = new TableCell(); row.Cells.Add(c);
        c.Text = "Time until release";

        c = new TableCell(); row.Cells.Add(c);
        c.Text = "Counts";

        c = new TableCell(); row.Cells.Add(c);
        c.Text = "Started";

        foreach (OutboundMail mail in mails)
        {
            TypedMailTemplate tmpl = null;
            if (mail.MailType != 0)
            {
                tmpl = TypedMailTemplate.FromName(mail.Title);
                tmpl.Initialize("SE", mail.OrganizationId, mail, "");
                tmpl.InsertAllPlaceHoldersToTemplate();
            }
            row = new TableRow();
            tab.Rows.Add(row);

            if (mail.MailType != 0)
            {
                c = new TableCell(); row.Cells.Add(c);
                c.Text = mail.Title;

                c = new TableCell(); row.Cells.Add(c);
                c.Text = tmpl.Template.TemplateTitleText;
            }
            else
            {
                c = new TableCell(); row.Cells.Add(c);
                c.Text = "none";

                c = new TableCell(); row.Cells.Add(c);
                c.Text = mail.Title;
            }
            c = new TableCell(); row.Cells.Add(c);
            c.Text = mail.ReleaseDateTime.ToString();
            if ((new DateTime(1970, 1, 1)) == mail.StartProcessDateTime && mail.ReleaseDateTime < DateTime.Now)
            {
                c.ForeColor = System.Drawing.Color.Red;
                c.Font.Bold = true;
            }

            c = new TableCell(); row.Cells.Add(c);
            c.Text = Math.Round((DateTime.Now.Subtract(mail.ReleaseDateTime).TotalMinutes)).ToString();


            c = new TableCell(); row.Cells.Add(c);
            c.Text = mail.RecipientCount.ToString();
            c.Text += " / " + mail.RecipientsSuccess.ToString();
            c.Text += " / " + mail.RecipientsFail.ToString();

            c = new TableCell(); row.Cells.Add(c);
            c.Text = mail.StartProcessDateTime.ToString();


        }

    }
}

