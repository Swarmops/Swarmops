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
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using System.Text.RegularExpressions;
using Activizr.Logic.Security;
using System.Collections.Generic;
using Activizr.Interface.Objects;

public partial class Pages_Public_SE_People_ActivistSignoff : System.Web.UI.Page
{
   
    protected void Page_Load (object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            Wizard1.ActiveStepIndex = 0;
            if (Request.QueryString["ticket"] != null)
            {
                Wizard1.ActiveStepIndex = 2;
                TextBoxCode.Text = Request.QueryString["ticket"].ToString();
            }
        }
        Button NextBtn = null;
        NextBtn = RecursiveFindControl(Wizard1, "StartNextButton") as Button;
        if ((NextBtn != null))
            Form.DefaultButton = NextBtn.UniqueID;
    }

    protected void Wizard1_NextButtonClick (object sender, WizardNavigationEventArgs e)
    {
        //LabelErrEmail.Text = "Denna sida är inte i funktion ännu. Återkom en annan dag...";
        //e.Cancel = true;
        //return;

        switch (e.CurrentStepIndex)
        {
            case 0:
                try
                {
                    string email = TextBoxEmail.Text.Trim();
                    if (email.StartsWith("#"))
                    {
                        string mNumber = email.Substring(1);
                        int mId = 0;
                        int.TryParse(mNumber, out mId);
                        Person pers = Person.FromIdentity(mId);
                        if (pers == null)
                        {
                                LabelErrEmail.Text = "Okänt nummer.";
                                e.Cancel = true;
                                break;
                        }
                        else
                        {
                            email = pers.Email;
                        }
                    }
                    else
                    {
                        Regex reNumber = new Regex("^[0-9]+$");
                        if (!reNumber.IsMatch(email))
                        {
                            if (!Formatting.ValidateEmailFormat(email))
                            {
                                LabelErrEmail.Text = "Felaktigt formaterad mailadress.";
                                e.Cancel = true;
                                break;
                            }
                        }
                    }
                    string URL = this.Request.Url.AbsoluteUri;
                    URL += "?ticket={0}";
                    Person person = Authentication.RequestActivistSignoffProcess(email, URL);
                    if (person == null)
                        throw new Exception();
                }
                catch (Exception)
                {
                    // Berätta inte ifall adressen tillhör en PP medlem. 
                    // Det kan missbrukas för att kartlägga medlemskap.
                    //LabelErrEmail.Text = "Kunde inte hitta den mailadressen i registret";
                    //e.Cancel = true;
                }
                break;
            case 1: break;
            case 2:
                try
                {
                    LabelErrCode.Text = "";

                    string code = TextBoxCode.Text.Trim();
                    string shortCode = code.Substring(0, 4);
                    string persIDStr = code.Substring(4);
                    int persId = 0;
                    int.TryParse(persIDStr, out persId);

                    List<Person> persList = People.FromIdentities(new int[] { persId });
                    if (persList.Count > 0)
                    {
                        Person p = persList[0];
                        Authentication.ValidateRequestActivistSignoffProcess(p, code);


                        if (p.IsActivist)
                        {
                            ActivistEvents.TerminateActivistWithLogging(p, EventSource.SignupPage);
                        }

                    }
                    else
                    {
                        LabelErrCode.Text = "Okänd kod. Begär en ny.";
                        e.Cancel = true;
                    }
                }
                catch (Authentication.VerificationTicketWrongException)
                {
                    LabelErrCode.Text = "Okänd kod.";
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    LabelErrCode.Text = ex.Message;
                    e.Cancel = true;
                }
                break;
            case 3:

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
