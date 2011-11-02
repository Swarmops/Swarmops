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
using Activizr.Logic.Governance;
using Activizr.Logic.Structure;

public partial class Pages_v4_Organization_AddMotion : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextTitle.Style[HtmlTextWriterStyle.Width] = "300px";
        this.TextThreadUrl.Style[HtmlTextWriterStyle.Width] = "300px";
        this.TextPreamble.Style[HtmlTextWriterStyle.Width] = "100%";
        this.TextAmendments.Style[HtmlTextWriterStyle.Width] = "100%";

        if (_currentUser.Identity != 1)
        {
            throw new UnauthorizedAccessException("No access to page");
        }

    }

    protected void ButtonAdd_Click(object sender, EventArgs e)
    {
        /*
        string designation = this.TextDesignation.Text;
        string title = this.TextTitle.Text;
        string preamble = this.TextPreamble.Text;
        string threadUrl = this.TextThreadUrl.Text;

        string amendmentsRaw = this.TextAmendments.Text;
        string[] amendments = amendmentsRaw.Split('\r', '\n');

        Meeting meeting = Meeting.FromIdentity(1);

        Motion motion = Motion.Create(meeting, designation, title, preamble, threadUrl);

        foreach (string amendment in amendments)
        {
            if (amendment.Trim().Length > 0)
            {
                motion.AddAmendment(amendment.Trim());
            }
        }

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Motion #" + motion.Identity + " registrerades.');", true);

        this.TextTitle.Text = string.Empty;
        this.TextPreamble.Text = string.Empty;
        this.TextDesignation.Text = string.Empty;
        this.TextAmendments.Text = string.Empty;
        this.TextThreadUrl.Text = string.Empty;

        this.TextDesignation.Focus();*/
    }
}
