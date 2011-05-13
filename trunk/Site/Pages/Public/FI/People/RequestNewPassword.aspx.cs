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
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using Activizr.Basic.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Activizr.Logic.Support;

public partial class Pages_Public_FI_People_RequestNewPassword : System.Web.UI.Page
{
    int CurrentPersonIDVS
    {
        get
        {
            if (ViewState["member"] == null)
                ViewState["member"] = 0;
            return (int)ViewState["member"];
        }
        set { ViewState["member"] = value; }
    }


    protected void Page_Load (object sender, EventArgs e)
    {

        if (!this.IsPostBack)
        {
            Wizard1.ActiveStepIndex = 0;
            if (Request.QueryString["ticket"] != null && Request.QueryString["member"] != null)
            {
                Wizard1.ActiveStepIndex = 2;
                TextBoxCode.Text = Request.QueryString["ticket"].ToString();
                CurrentPersonIDVS = int.Parse(Request.QueryString["member"]);
            }
        }
        Button NextBtn = null;
        NextBtn = RecursiveFindControl(Wizard1, "StartNextButton") as Button;
        if ((NextBtn != null))
            Form.DefaultButton = NextBtn.UniqueID;
    }

    protected void Wizard1_NextButtonClick (object sender, WizardNavigationEventArgs e)
    {
                    LabelErrEmail.Visible = false;
                    LabelErrCode.Visible = false;
                    LabelErrPassword1.Visible = false;
                    LabelErrPassword2.Visible = false;

                    switch (e.CurrentStepIndex)
        {
            case 0:
                try
                {
                    LabelErrEmail.Visible = false;

                    CurrentPersonIDVS = 0;
                    string email = TextBoxEmail.Text;

                    Regex reNumber = new Regex("^[0-9]+$");
                    if (!reNumber.IsMatch(email))
                    {
                        if (!Formatting.ValidateEmailFormat(email))
                        {
                            LabelErrEmail.Visible = true;
                            e.Cancel = true;
                            break;
                        }
                    }
                    string URL = this.Request.Url.AbsoluteUri;
                    URL += "?member={0}&ticket={1}";
                    Person person = Authentication.RequestNewPasswordProcess(email, URL);
                    if (person == null)
                        throw new Exception("No such address");
                    CurrentPersonIDVS = person.PersonId;
                }
                catch (Exception ex)
                {
                    // Berätta inte ifall adressen tillhör en PP medlem. 
                    // Det kan missbrukas för att kartlägga medlemskap.
                    //LabelErrEmail.Text = "Kunde inte hitta den mailadressen i registret";
                    //Response.Write("<!-- " + ex.Message + "\r\n" + ex.StackTrace + " -->");
                    //e.Cancel = true;
                }
                break;
            case 1: break;
            case 2:
                try
                {

                    string code = TextBoxCode.Text.Trim();

                    List<Person> persList = People.FromIdentities(new int[] { CurrentPersonIDVS });
                    if (persList.Count > 0)
                    {
                        Person p = persList[0];
                        Authentication.ValidateEmailVerificationTicket(p, code);
                    }
                    else
                    {
                        LabelErrCode.Visible = true;
                        e.Cancel = true;
                    }

                }
                catch (Authentication.VerificationTicketLengthException)
                {
                    LabelErrCode.Visible = true;
                    e.Cancel = true;
                }
                catch (Authentication.VerificationTicketTooOldException)
                {
                    LabelErrCode.Visible = true;
                    e.Cancel = true;
                }
                catch (Authentication.VerificationTicketWrongException)
                {
                    LabelErrCode.Visible = true;
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    LabelErrCode.Visible = true;
                    e.Cancel = true;
                }
                break;
            case 3:
                try
                {
                    string code = TextBoxCode.Text.Trim();
                    List<Person> persList = People.FromIdentities(new int[] { CurrentPersonIDVS });
                    Person p = persList[0];

                    if (TextBoxPassword1.Text.Length < 5)
                    {
                        LabelErrPassword1.Visible = true;
                        e.Cancel = true;
                    }
                    else if (TextBoxPassword1.Text != TextBoxPassword2.Text)
                    {
                        LabelErrPassword2.Visible = true;
                        e.Cancel = true;
                    }
                    else
                    {
                        LabelErrPassword2.Text = "Kunde inte spara det nya lösenordet.";
                        Authentication.SetPasswordByEmailVerificationTicket(p, code, TextBoxPassword1.Text);
                    }
                }
                catch (Exception)
                {
                    e.Cancel = true;
                }
                break;
            case 4: break;
        }
        Button NextBtn = null;
        if (e.NextStepIndex == (Wizard1.WizardSteps.Count - 1))
            NextBtn = RecursiveFindControl(Wizard1, "FinishButton") as Button;
        else if (e.NextStepIndex > 0)
            NextBtn = RecursiveFindControl(Wizard1, "StepNextButton") as Button;
        else if (e.NextStepIndex == 0)
            NextBtn = RecursiveFindControl(Wizard1, "StartNextButton") as Button;
        if (NextBtn != null)
            Form.DefaultButton = NextBtn.UniqueID;

    }

    protected void Wizard1_FinishButtonClick (object sender, WizardNavigationEventArgs e)
    {
        Response.Redirect("~/", true);
    }

    private Control RecursiveFindControl (Control ctrl, string p)
    {
        if (ctrl.ID == p)
            return ctrl;
        if (ctrl.HasControls())
        {
            foreach (Control c in ctrl.Controls)
            {
                Control foundC = RecursiveFindControl(c, p);
                if (foundC != null)
                    return foundC;
            }
        }
        return null;
    }

}
