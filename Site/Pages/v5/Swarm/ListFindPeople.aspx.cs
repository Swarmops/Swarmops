using System;
using System.Security;
using System.Web;
using System.Web.Services;
using Resources;

using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.Swarm
{
    public partial class ListFindPeople : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageIcon = "iconshock-group-search";
            this.PageAccessRequired = new Access (CurrentOrganization, AccessAspect.PersonalData, AccessType.Read);

            if (!Page.IsPostBack)
            {
                Localize();
            }

            RegisterControl (EasyUIControl.Tree | EasyUIControl.DataGrid);
        }

        private void Localize()
        {
            PageTitle = Resources.Pages.Swarm.ListFindPeople_Title;
            InfoBoxLiteral = Resources.Pages.Swarm.ListFindPeople_Info;
            this.LabelGeography.Text = Global.Global_Geography;
            this.LabelNamePattern.Text = Resources.Pages.Swarm.ListFindPeople_NamePattern;
            this.LabelMatchingPeopleInX.Text = String.Format (Resources.Pages.Swarm.ListFindPeople_MatchingPeopleInX,
                CurrentOrganization.Name);

            this.LabelGridHeaderAction.Text = Global.Global_Action;
            this.LabelGridHeaderGeography.Text = Global.Global_Geography;
            this.LabelGridHeaderMail.Text = Global.Global_Mail;
            this.LabelGridHeaderName.Text = Global.Global_Name;
            this.LabelGridHeaderPhone.Text = Global.Global_Phone;
            this.LabelGridHeaderNotes.Text = Global.Global_Notes;
        }

        // ReSharper disable once InconsistentNaming
        public string Localized_NoticeTooManyHits
        {
            get { return JavascriptEscape(Resources.Pages.Swarm.ListFindPeople_TooManyHits); }
        }
    }
}