using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Activizr.Logic.Communications;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Activizr.Logic.Security;

public partial class Pages_v4_EditMailTemplate : PageV4Base
{
    protected enum pageUIState
    { open, edit, saveas }

    public int CurrentTemplateIdViewState
    {
        get
        {
            if (ViewState["currentTemplateIdViewState"] == null)
                return 0;
            return (int)ViewState["currentTemplateIdViewState"];
        }
        set
        {
            ViewState["currentTemplateIdViewState"] = value;
        }
    }

    public string CurrentTemplateNameViewState
    {
        get
        {
            if (ViewState["currentTemplateNameViewState"] == null)
                return "";
            return (string)ViewState["currentTemplateNameViewState"];
        }
        set
        {
            ViewState["currentTemplateNameViewState"] = value;
        }
    }

    protected void Page_Load (object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetUIState(pageUIState.open);

            BuildTemplateDropdowns();
        }

        SetEditorToolbar();
        Controls_v4_WSOrgTreeDropDown.EmitScripts(this);
        (WSOrgTreeDropDown1).tree.SetAuthority(_authority, Permission.CanEditMailTemplates);

    }

    protected void Page_PreRender (object sender, EventArgs e)
    {

    }

    protected void SetUIState (pageUIState pageUIState)
    {
        if (pageUIState == pageUIState.open)
        {
            EditPanel.Visible = true;
            EditPanel.Style["display"] = "none";
            TopPanel.Visible = true;
            TopPanelSelect.Visible = true;
            TopPanelSpecify.Visible = false;
            TopPanelView.Visible = false;
            ButtonOpen.Visible = true;
            ButtonSave.Visible = false;
            ButtonSaveAs.Visible = false;
            ButtonCancelEdit.Visible = false;
            ButtonCancelSaveAs.Visible = false;
        }
        else if (pageUIState == pageUIState.edit)
        {
            EditPanel.Style["display"] = "";
            TopPanelSpecify.Visible = false;
            TopPanelSelect.Visible = false;
            TopPanelView.Visible = true;
            ButtonOpen.Visible = false;
            ButtonSave.Visible = true;
            ButtonSaveAs.Visible = true;
            ButtonCancelEdit.Visible = true;
            ButtonCancelSaveAs.Visible = false;
        }
        else if (pageUIState == pageUIState.saveas)
        {
            EditPanel.Style["display"] = "none";
            TopPanel.Visible = true;
            TopPanelSelect.Visible = false;
            TopPanelSpecify.Visible = true;
            TopPanelView.Visible = false;
            ButtonOpen.Visible = false;
            ButtonSave.Visible = true;
            ButtonSaveAs.Visible = false;
            ButtonCancelEdit.Visible = false;
            ButtonCancelSaveAs.Visible = true;
        }
        else
        {
        }

        ButtonSave.Visible = _authority.HasPermission(Permission.CanEditMailTemplates, WSOrgTreeDropDown1.SelectedOrganizationId, -1, Authorization.Flag.Default);      

    }

    private void BuildTemplateDropdowns ()
    {
        Organizations orgs = _authority.GetOrganizations(RoleTypes.AllLocalRoleTypes);
        Dictionary<string, Organization> cntryList = new Dictionary<string, Organization>();
        foreach (Organization org in orgs)
        {
            if (!cntryList.ContainsKey(org.DefaultCountry.Code.ToUpper()))
            {
                cntryList.Add(org.DefaultCountry.Code.ToUpper(), org);
            }
        }

        List<MailTemplate> templatelist = MailTemplate.GetAll();
        Dictionary<string, MailTemplate> templateDict = new Dictionary<string, MailTemplate>();
        foreach (MailTemplate mt in templatelist)
        {
            templateDict[mt.TemplateName] = mt;
        }

        DropDownName.Items.Clear();
        foreach (string basename in TypedMailTemplate.GetTemplateNames())
        {
            DropDownName.Items.Add(new ListItem(basename));
            DropDownName.Items.Add(new ListItem(basename + "Plain"));
        }

        DropDownListTemplates.Items.Clear();
        foreach (MailTemplate mt in templatelist)
        {
            // if (cntryList.ContainsKey(mt.CountryCode.ToUpper()))
            // {
                string ItemText = mt.TemplateName + ", "
                        + mt.LanguageCode + "-" + mt.CountryCode
                        + (mt.OrganizationId != 0 ? ", " + Organization.FromIdentity(mt.OrganizationId).Name : "");

                DropDownListTemplates.Items.Add(new ListItem(ItemText, mt.TemplateId.ToString()));
            // }
        }
        Countries countries = Countries.GetInUse();
        foreach (Country cntry in countries)
        {
            if (cntryList.ContainsKey(cntry.Code.ToUpper()))
            {
                DropDownCountry.Items.Add(new ListItem(cntry.Code + " - " + cntry.Name, cntry.Code));
            }
        }
    }

    private void SetEditorToolbar ()
    {
        EditorToolGroup foundgroup = null;
        foreach (EditorToolGroup tg in RadEditor1.Tools)
        {
            if (tg.Attributes["id"] == "placeholders")
                foundgroup = tg;
        }
        if (foundgroup != null)
            RadEditor1.Tools.Remove(foundgroup);

        EditorToolGroup main = new EditorToolGroup();

        main.Attributes["id"] = "placeholders";
        RadEditor1.Tools.Add(main);

        //add a custom dropdown and set its items and dimension attributes
        EditorDropDown ddn = new EditorDropDown("PlaceHolders");
        ddn.Text = "Place holders";

        //Set the popup width and height
        ddn.Attributes["width"] = "110px";
        ddn.Attributes["popupwidth"] = "240px";
        ddn.Attributes["popupheight"] = "200px";

        if (CurrentTemplateNameViewState != "")
        {
            TypedMailTemplate tmpl = TypedMailTemplate.FromName(CurrentTemplateNameViewState);

            foreach (string key in tmpl.Placeholders.Keys)
            {
                if (tmpl.Placeholders[key].TagOrId == TypedMailTemplate.PlaceholderType.Tag)
                {
                    ddn.Items.Add(tmpl.Placeholders[key].Label, " <span class='placeholder2' >%" + tmpl.Placeholders[key].Name + "%</span> ");
                }
                else if (tmpl.Placeholders[key].TagOrId == TypedMailTemplate.PlaceholderType.Id)
                {
                    ddn.Items.Add(tmpl.Placeholders[key].Label, " <div id=\"" + tmpl.Placeholders[key].Name + "\" class='placeholder1' > " + tmpl.Placeholders[key].Label + " </div> ");
                }
                else
                {
                    ddn.Items.Add(tmpl.Placeholders[key].Label, " <span id=\"" + tmpl.Placeholders[key].Name + "\" class='placeholder1' > <span class='placeholder2' >%" + tmpl.Placeholders[key].Name + "%</span> </span> ");
                }
            }
        }

        //Add tool to toolbar
        main.Tools.Add(ddn);
    }

    protected void ButtonOpen_Click (object sender, EventArgs e)
    {

        SetUIState(pageUIState.edit);

        int templateId = int.Parse(DropDownListTemplates.SelectedValue);
        OpenTemplateById(templateId);
    }

    private void OpenTemplateById (int templateId)
    {
        MailTemplate currentTemplate = MailTemplate.FromIdentity(templateId);
        ShowTemplate(currentTemplate);
    }

    private void ShowTemplate (MailTemplate currentTemplate)
    {
        this.LabelCountryName.Text = Country.FromCode(currentTemplate.CountryCode).Name;
        this.LabelOrgName.Text = "";
        if (currentTemplate.OrganizationId != 0)
        {
            this.LabelOrgName.Text = Organization.FromIdentity(currentTemplate.OrganizationId).Name;
            ButtonSave.Visible = _authority.HasPermission(Permission.CanEditMailTemplates, currentTemplate.OrganizationId, -1, Authorization.Flag.Default);
        }
        else
        {
            ButtonSave.Visible = _authority.HasPermission(Permission.CanEditMailTemplates, Organization.RootIdentity, -1, Authorization.Flag.Default);
        }

        this.LabelTemplateName.Text = currentTemplate.TemplateName;
        CurrentTemplateNameViewState = currentTemplate.TemplateName;
        CurrentTemplateIdViewState = currentTemplate.TemplateId;
        currentTemplate.NormalizeHtml();

        TypedMailTemplate typedTemplate = TypedMailTemplate.FromName(currentTemplate.TemplateName);
        Dictionary<string, string> placeholders = new Dictionary<string, string>();
        foreach (string ph in typedTemplate.Placeholders.Keys)
        {
            placeholders.Add(ph.ToLower(), ph);
        }

        currentTemplate.MarkPlaceholderSpans(placeholders);

        RadEditor1.Content = currentTemplate.TemplateBody;
        TextBoxSubject.Text = currentTemplate.TemplateTitle;
        SetEditorToolbar();
    }


    protected void ButtonSave_Click (object sender, EventArgs e)
    {
        if (this.CurrentTemplateIdViewState != 0 && ButtonCancelSaveAs.Visible == false)
        {
            MailTemplate currentTemplate = MailTemplate.FromIdentity(this.CurrentTemplateIdViewState);
            currentTemplate.TemplateBody = RadEditor1.Content;
            currentTemplate.NormalizeHtml();
            currentTemplate.TemplateTitle = TextBoxSubject.Text;
            currentTemplate.NormalizeHtml();
            currentTemplate.RemovePlaceholderSpans();
            currentTemplate.Update();
            ShowTemplate(currentTemplate);
            BuildTemplateDropdowns();
        }
        else
        {
            string cntryCode = "";
            string cntryLang = "";
            if (DropDownCountry.SelectedValue != "")
            {
                Country cntry = Country.FromCode(DropDownCountry.SelectedValue);
                cntryCode = cntry.Code;
                cntryLang = cntry.Culture.Substring(0, 2);
            }

            int orgId = WSOrgTreeDropDown1.SelectedOrganizationId;
            Organization org = Organization.FromIdentity(orgId);

            if (_authority.HasPermission(Permission.CanSetUpAutomail, orgId, org.DefaultCountry.GeographyId, Authorization.Flag.Default))
            {
                if (orgId == Organization.RootIdentity)
                    orgId = 0;

                MailTemplate currentTemplate = MailTemplate.Create(DropDownName.SelectedValue, cntryLang,
                                                                    cntryCode, orgId, RadEditor1.Content);
                this.CurrentTemplateIdViewState = currentTemplate.TemplateId;
                this.CurrentTemplateNameViewState = currentTemplate.TemplateName;
                currentTemplate.TemplateBody = RadEditor1.Content;
                currentTemplate.NormalizeHtml();
                currentTemplate.TemplateTitle = TextBoxSubject.Text;
                currentTemplate.NormalizeHtml();
                currentTemplate.MarkPlaceholderSpans();
                currentTemplate.Update();
                ShowTemplate(currentTemplate);
                BuildTemplateDropdowns();
                SetUIState(pageUIState.edit);
            }
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "StartupMessage", "alert('You have to select an organization for wich you have permissions to edit templates for.');", true);
        }

    }

    protected void ButtonSaveAs_Click (object sender, EventArgs e)
    {
        SetUIState(pageUIState.saveas);
    }

    protected void ButtonCancelEdit_Click (object sender, EventArgs e)
    {
        SetUIState(pageUIState.open);
    }

    protected void ButtonCancelSaveAs_Click (object sender, EventArgs e)
    {
        SetUIState(pageUIState.edit);
    }

    protected void WSOrgTreeDropDown1_SelectedNodeChanged (object sender, RadTreeNodeEventArgs args)
    {
        ButtonSave.Visible = _authority.HasPermission(Activizr.Basic.Enums.Permission.CanEditMailTemplates, WSOrgTreeDropDown1.SelectedOrganizationId, -1, Authorization.Flag.Default);
    }
}
