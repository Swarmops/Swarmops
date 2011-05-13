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
using Activizr.Logic.Structure;
using Activizr.Logic.Pirates;
using Activizr.Logic.DataObjects;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using System.Collections.Generic;

public partial class Pages_v4_ActivePirate_DHL : PageV4Base
{
    protected static int recieverPersonID = 1185;

    protected void Page_Load (object sender, EventArgs e)
    {

        if (Request["Steps"] != null)
        {
            labelWasSent.Visible = true;
            Wizard1.Visible = false;
        }
        else
        {

            if (!IsPostBack)
            {
                Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
                BasicPersonRole[] personRoles = RolesDataObject.Select(viewingPerson, RoleClass.Local);
                List<int> orgs = new List<int>();
                ddOrganisation.Items.Clear();
                foreach (BasicPersonRole role in personRoles)
                {
                    if (!orgs.Contains(role.OrganizationId))
                    {
                        orgs.Add(role.OrganizationId);
                    }
                }
                Organizations oOrgs = Organizations.FromIdentities(orgs.ToArray());
                foreach (Organization org in oOrgs)
                {
                    ddOrganisation.Items.Add(new ListItem(org.Name));
                }
                avsNamn.Text = viewingPerson.Name;
                avsAddr.Text = viewingPerson.Street;
                avsPostCode.Text = viewingPerson.PostalCode;
                avsPostCity.Text = viewingPerson.CityName;
                avsEmail.Text = viewingPerson.Email;
                avsPhone.Text = viewingPerson.Phone;
            }

            Button NextBtn = null;
            NextBtn = RecursiveFindControl(Wizard1, "StartNextButton") as Button;
            if ((NextBtn != null))
                Form.DefaultButton = NextBtn.UniqueID;

        }

    }

    protected void Wizard1_NextButtonClick (object sender, WizardNavigationEventArgs e)
    {
        int checkstep = e.CurrentStepIndex;
        VerifyStep(e, checkstep);
        if (e.Cancel)
            Wizard1.ActiveStepIndex = checkstep;

        RefreshSummary();

    }

    protected void Wizard1_FinishButtonClick (object sender, WizardNavigationEventArgs e)
    {
        RefreshSummary();
        for (int i = 0; i < Wizard1.WizardSteps.Count; ++i)
        {
            VerifyStep(e, i);
            if (e.Cancel)
            {
                e.Cancel = false;
                Wizard1.FinishDestinationPageUrl = "";
                Wizard1.ActiveStepIndex = i;
                return;
            }
        }

        Person reciever = Person.FromIdentity(recieverPersonID);
        string msg = SummaryPanel.InnerHtml.Replace("\r", "").Replace("<br />", "\r\n");
        reciever.SendNotice("Beställning Fraktdokument", msg, Organization.PPSEid);
        Wizard1.FinishDestinationPageUrl = "DHL.aspx?Steps=Done";
    }

    private void VerifyStep (WizardNavigationEventArgs e, int checkstep)
    {
        Label errMsg = RecursiveFindControl(Wizard1, "ErrorMsgLabel") as Label;
        errMsg.Text = "";
        switch (checkstep)
        {
            case 0: //Inledning
                break;
            case 1://Fraktsätt
                if (this.Typ.SelectedValue == "Paket" && this.Avisering.SelectedValue != "Ingen")
                {
                    e.Cancel = true;
                    errMsg.Text = "För DHL-paket kan man inte ha avisering.";
                    this.Avisering.SelectedValue = "Ingen";
                }
                break;
            case 2://Avsändare
                if ((this.avsNamn.Text.Trim() == "")
                    || (this.avsAddr.Text.Trim() == "")
                    || (this.avsPostCode.Text.Trim() == "")
                    || (this.avsPostCity.Text.Trim() == "")
                    || (this.avsEmail.Text.Trim() == "")
                    || (this.avsPhone.Text.Trim() == ""))
                {
                    errMsg.Text = "Alla fält måste fyllas i";
                    e.Cancel = true;
                }
                break;
            case 3: //Fakturering
                if (this.fakturering.SelectedIndex == 0 && this.budget.Text.Trim() == "")
                {
                    errMsg.Text = "Budgetställe måste anges vid samfakturering";
                    e.Cancel = true;
                }
                if (this.fakturering.SelectedIndex == -1)
                {
                    errMsg.Text = "Ange fakureringstyp";
                    e.Cancel = true;
                }
                break;
            case 4: //Mottagare
                if ((this.mottNamn.Text.Trim() == "")
                    || (this.mottAdress1.Text.Trim() == "" && this.mottAdress2.Text.Trim() == "")
                    || (this.mottPostCode.Text.Trim() == "")
                    || (this.mottCity.Text.Trim() == "")
                    )
                {
                    errMsg.Text += "Namn, adress, postnr, ort måste fyllas i.<br />";
                    e.Cancel = true;
                }

                if ((this.Avisering.SelectedValue == "SMS")
                    && (this.mottPhone.Text.Trim() == "")
                    )
                {
                    errMsg.Text += "Telefonr måste fyllas i vid avisering via SMS.<br />";
                    e.Cancel = true;
                }

                if ((this.Avisering.SelectedValue == "epost")
                    && (this.mottEMail.Text.Trim() == "")
                    )
                {
                    errMsg.Text += "Mailadress måste fyllas i vid avisering via e-post.<br />";
                    e.Cancel = true;
                }
                break;
            case 5://Leverans
                int antk = 0;
                if (!int.TryParse(antKolli.Text, out antk))
                {
                    errMsg.Text += "Antal kolli måste fyllas i.<br />";
                    e.Cancel = true;
                }
                else if (antk < 1 || antk > 99)
                {
                    errMsg.Text += "Antal kolli fel.<br />";
                    e.Cancel = true;
                }
                else
                {
                    string[] viktArr = vikt.Text.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (viktArr.Length != antk)
                    {
                        errMsg.Text += "Fel antal vikter/kolli.<br />";
                        e.Cancel = true;
                    }
                    else if (innehDekl.Text.Trim() == "")
                    {
                        errMsg.Text += "Innehåll ska anges.<br />";
                        e.Cancel = true;
                    }
                }

                break;
            case 6://Övr

                break;

            case 7: //Sammanst
                break;
        }
        Button NextBtn = null;
        if (e.NextStepIndex == (Wizard1.WizardSteps.Count-1))
            NextBtn = RecursiveFindControl(Wizard1, "FinishButton") as Button;
        else if (e.NextStepIndex > 0)
            NextBtn = RecursiveFindControl(Wizard1, "StepNextButton") as Button;
        else if (e.NextStepIndex == 0)
            NextBtn = RecursiveFindControl(Wizard1, "StartNextButton") as Button;
        if (NextBtn != null)
            Form.DefaultButton = NextBtn.UniqueID;

    }

    private void RefreshSummary ()
    {
        string txt = "";
        txt += "Skicka adresslappar avseende:<br />";
        txt += antKolli.Text + " st kolli, med vikt(er):" + vikt.Text + " kg. <br /><br />";
        txt += "Innehåll: " + innehDekl.Text + "<br />";
        txt += "<br />";
        txt += "<br />";
        txt += "Försändelsen ska skickas som:";
        txt += Typ.SelectedItem.Text + "<br />";
        txt += "<br />";
        txt += "till mottagare:<br />";
        txt += mottNamn.Text + "<br />";
        txt += mottAdress1.Text + "<br />";
        txt += mottAdress2.Text + "<br />";
        txt += mottPostCode.Text + " ";
        txt += mottCity.Text + "<br /><br />";
        txt += "Telefon: " + mottPhone.Text + "<br />";
        txt += "Email: " + mottEMail.Text + "<br />";
        txt += "Portkod: " + mottCode.Text + "<br />";
        txt += "<br />";
        txt += "Avisering till mottagaren: " + Avisering.SelectedValue + "<br />";
        txt += "<br />";
        txt += "Leveransinstruktion till transportör:<br />" + leveransAnv.Text.Replace("\n", "<br />");
        txt += "<br />";
        txt += "<br />";
        txt += "Frakten ska betalas av: " + ddOrganisation.SelectedItem.Text + "<br />";
        txt += "över budget för: " + budget.Text + "<br />";
        txt += fakturering.SelectedItem.Text + "<br />";
        txt += "<br />";
        txt += "<br />";
        txt += "Adresslapparna ska skickas till:<br />";
        txt += avsNamn.Text + "<br />";
        txt += avsAddr.Text + "<br />";
        txt += avsPostCode.Text + " ";
        txt += avsPostCity.Text + "<br /><br />";
        txt += "Telefon: " + avsPhone.Text + "<br />";
        txt += "Email: " + avsEmail.Text + "<br />";
        txt += "<br />";
        txt += "<br />";
        txt += "Övriga uppgifter:<br />" + ovrUppl.Text.Replace("\n", "<br />");


        SummaryPanel.InnerHtml = txt;
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
