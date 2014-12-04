using System;
using System.Web.UI;
using Swarmops.Logic.Swarm;

namespace Swarmops.Controls.Swarm
{
    public delegate void PersonChangedEventHandler(object sender, PersonChangedEventArgs e);

    public partial class PersonDetailPopup : ControlV5Base
    {
        private Person _person;

        public object Cookie { get; set; }

        public Person Person
        {
            get
            {
                if (this._person == null)
                {
                    // return this.PersonNew.SelectedPerson;
                }

                return this._person;
            }
            set
            {
                this._person = value;
                // this.PersonNewOwner.SelectedPerson = _person;

                if (value != null)
                {
                    ViewState[ClientID + "_PersonId"] = value.Identity;

                    this.ImageAvatar.ImageUrl = this._person.GetSecureAvatarLink(96);
                    this.LabelPersonName.Text = this._person.Name;
                    this.LabelPersonIdentity.Text = "#" + this._person.Identity;
                }
                else
                {
                    ViewState[ClientID + "_PersonId"] = null;
                    this.ImageAvatar.ImageUrl = "https://pirateweb.net/Images/Public/Fugue/icons-shadowless/cross.png";
                    this.LabelPersonIdentity.Text = "#";
                    this.LabelPersonName.Text = "Nobody";
                }
            }
        }


        public bool Changeable { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            object personId = ViewState[ClientID + "_PersonId"];

            if (personId != null)
            {
                this._person = Person.FromIdentity((int) personId);
            }

            this.LabelPersonName.Style[HtmlTextWriterStyle.FontSize] = "20px";
            // this.PersonNew.Width = 157;

            this.ButtonSetNew.Visible = Changeable;

            // Localize

            // this.ButtonSetNew.Text = Resources.Controls.Pirates.PersonDetailPopup_ChangePerson;
            // this.LabelWriteLabel.Text = Resources.Controls.Pirates.PersonDetailPopup_ChangeTo;
            this.ButtonConfirmPerson.Text = Resources.Global.Global_Confirm;
            this.ButtonCancel.Text = Resources.Global.Global_Cancel;
        }


        protected void ButtonSetNew_Click(object sender, EventArgs e)
        {
            this.PanelRead.Visible = false;
            this.PanelWrite.Visible = true;
            // this.PersonNew.Focus();
        }

        public void Reset()
        {
            this.PanelWrite.Visible = false;
            this.PanelRead.Visible = true;
            this.ButtonConfirmPerson.Visible = false;
            // this.PersonNew.SelectedPerson = null;

            if (ViewState[ClientID + "_PersonId"] == null)
            {
                this.ImageAvatar.ImageUrl = "https://pirateweb.net/Images/Public/Fugue/icons-shadowless/cross.png";
            }
            else
            {
                this.ImageAvatar.ImageUrl =
                    Person.FromIdentity((int) ViewState[ClientID + "_PersonId"]).GetSecureAvatarLink(96);
            }
        }


        protected void PersonNew_SelectedPersonChanged(object sender, EventArgs e)
        {
            // this.ImageAvatar.ImageUrl = this.PersonNew.SelectedPerson.GetSecureAvatarLink(96);
            this.ButtonConfirmPerson.Visible = true;
        }

        protected void ButtonConfirmPerson_Click(object sender, EventArgs e)
        {
            // this._person = this.PersonNew.SelectedPerson;

            IOwnerSettable settableInterface = Cookie as IOwnerSettable;

            if (settableInterface != null)
            {
                settableInterface.SetOwner(this._person);
            }

            if (PersonChanged != null)
            {
                // PersonChanged(this, new PersonChangedEventArgs(this.PersonNew.SelectedPerson, this.Cookie));
            }

            this.LabelPersonName.Text = this._person.Name;
            this.LabelPersonIdentity.Text = "#" + this._person.Identity;
            Reset();
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Reset();
        }

        public event PersonChangedEventHandler PersonChanged;
    }


    public class PersonChangedEventArgs : EventArgs
    {
        public readonly object Cookie;
        public readonly Person NewPerson;

        public PersonChangedEventArgs(Person newPerson, object cookie)
        {
            this.NewPerson = newPerson;
            this.Cookie = cookie;
        }
    }
}