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
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Telerik.Web.UI;

public partial class Controls_v5_PersonDetailPopup : ControlV5Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        object personId = ViewState[this.ClientID + "_PersonId"];

        if (personId != null)
        {
            _person = Person.FromIdentity((int) personId);
        }

        this.LabelPersonName.Style[HtmlTextWriterStyle.FontSize] = "20px";
        this.PersonNewOwner.Width = 167;
    }


    private Person _person;


    public Person Person
    {
        get
        {
            if (_person == null)
            {
                return this.PersonNewOwner.SelectedPerson;    
            }

            return _person;
        }
        set
        {
            _person = value;
            // this.PersonNewOwner.SelectedPerson = _person;

            if (value != null)
            {
                ViewState[this.ClientID + "_PersonId"] = value.Identity;

                this.ImageAvatar.ImageUrl = _person.GetSecureAvatarLink(96);
                this.LabelPersonName.Text = _person.Name;
                this.LabelPersonIdentity.Text = "#" + _person.Identity.ToString();
            }
            else
            {
                ViewState[this.ClientID + "_PersonId"] = null;
                this.ImageAvatar.ImageUrl = "https://pirateweb.net/Images/Public/Fugue/icons-shadowless/cross.png";
                this.LabelPersonIdentity.Text = "#";
                this.LabelPersonName.Text = "Nobody";
            }

        }
    }



    public FinancialAccount Account
    {
        set
        {
            _account = value;
        }
    }


    private FinancialAccount _account;

    protected void ButtonSetNewOwner_Click(object sender, EventArgs e)
    {
        this.PanelRead.Visible = false;
        this.PanelWrite.Visible = true;
        this.PersonNewOwner.Focus();
    }

    public void Reset()
    {
        this.PanelWrite.Visible = false;
        this.PanelRead.Visible = true;
        this.ButtonConfirmPerson.Visible = false;
        this.PersonNewOwner.SelectedPerson = null;

        if (ViewState[this.ClientID + "_PersonId"] == null)
        {
            this.ImageAvatar.ImageUrl = "https://pirateweb.net/Images/Public/Fugue/icons-shadowless/cross.png";
        }
        else
        {
            this.ImageAvatar.ImageUrl = Person.FromIdentity((int) ViewState[this.ClientID + "_PersonId"]).GetSecureAvatarLink(96);
        }
    }


    protected void PersonNewOwner_SelectedPersonChanged(object sender, EventArgs e)
    {
        this.ImageAvatar.ImageUrl = this.PersonNewOwner.SelectedPerson.GetSecureAvatarLink(96);
        this.ButtonConfirmPerson.Visible = true;
    }

    protected void ButtonConfirmPerson_Click(object sender, EventArgs e)
    {
        if (this._account == null)
        {
            throw new InvalidOperationException("Can't set owner for null account");
        }

        this._account.Owner = this.PersonNewOwner.SelectedPerson;
        this._person = this.PersonNewOwner.SelectedPerson;

        if (PersonChanged != null)
        {
            PersonChanged(this, new EventArgs());
        }

        this.LabelPersonName.Text = _person.Name;
        this.LabelPersonIdentity.Text = "#" + _person.Identity.ToString();
        Reset();
    }

    protected void ButtonCancel_Click(object sender, EventArgs e)
    {
        Reset();
    }

    public event EventHandler PersonChanged; 

}
