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
using Activizr.Basic.Types;
using Activizr.Interface;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

public partial class Controls_v4_ComboPerson : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private const int initialItemCount = 50;

    protected void ComboControl_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
    {
        People candidates = null;

        if (e.Text.Replace(" ", "").Length >= 4)
        {
            candidates = People.FromNamePattern(e.Text);
        }

        if (candidates == null)
        {
            candidates = new People();
        }

        candidates = candidates.GetVisiblePeopleByAuthority(this.authority);
        candidates.Sort();

        int itemOffset = e.NumberOfItems;
        int itemCount = Math.Min(itemOffset + initialItemCount, candidates.Count);
        e.EndOfItems = (itemCount == candidates.Count ? true : false);

        for (int i = itemOffset; i < itemCount; i++)
        {
            string descriptionString = candidates[i].Canonical;

            RadComboBoxItem comboItem = new RadComboBoxItem(descriptionString, candidates[i].Identity.ToString());
            comboItem.ImageUrl = "/Images/Public/Fugue/icons-shadowless/" +
                                 PersonIcon.ForPerson(candidates[i], Organizations.FromSingle(Organization.PPSE)).Image;

            this.ComboControl.Items.Add(comboItem);
        }

        e.Message = GetStatusMessage(itemCount, candidates.Count);
    }

    private static string GetStatusMessage(int offset, int total)
    {
        if (total <= 0)
            return "No matches";

        return String.Format("People <b>1</b>-<b>{0}</b> out of <b>{1}</b>", offset, total);
    }

    private Authority authority;

    public Authority Authority
    {
        get { return this.authority; }
        set { this.authority = value; }
    }

    public string Text
    {
        get { return this.ComboControl.Text; }
        set { this.ComboControl.Text = value; }
    }

    public bool HasSelection
    {
        get { return this.ComboControl.SelectedValue.Trim().Length > 0 ? true : false; }
    }

    public Person SelectedPerson
    {
        get
        {
            if (HasSelection)
            {
                return Person.FromIdentity(Int32.Parse(this.ComboControl.SelectedValue));
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (value == null)
            {
                this.ComboControl.SelectedIndex = -1;
                this.ComboControl.Text = string.Empty;
            }
            else
            {
                this.ComboControl.SelectedValue = value.Identity.ToString();
            }
        }
    }


    protected void ComboControl_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (this.SelectedPersonChanged != null)
        {
            SelectedPersonChanged (this, new EventArgs());
        }
    }

    public int Width
    {
        set { this.ComboControl.Width = value; }
    }

    public event EventHandler SelectedPersonChanged; 
}