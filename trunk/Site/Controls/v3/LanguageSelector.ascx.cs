using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Controls_LanguageSelector : System.Web.UI.UserControl
{
    protected Dictionary<String,bool> showLanguages =new Dictionary<String,bool>()  ;

    public string ShowLanguages
    {
        // ShowLanguages="sv,en,fr,de,ru,ja,es"
        set { 
            showLanguages.Clear();
            foreach (string lang in value.Split(new char[]{','}))
                showLanguages[lang.Trim().ToLower()] = true;
        }
    }
	protected void Page_Load(object sender, EventArgs e)
	{
        if (!showLanguages.ContainsKey("sv")) ButtonLanguageSwedish.Visible = false;
        if (!showLanguages.ContainsKey("en")) ButtonLanguageEnglish.Visible = false;
        if (!showLanguages.ContainsKey("fr")) ButtonLanguageFrench.Visible = false;
        if (!showLanguages.ContainsKey("de")) ButtonLanguageGerman.Visible = false;
        if (!showLanguages.ContainsKey("ru")) ButtonLanguageRussian.Visible = false;
        if (!showLanguages.ContainsKey("ja")) ButtonLanguageJapanese.Visible = false;
        if (!showLanguages.ContainsKey("es")) ButtonLanguageSpanish.Visible = false;
        if (!showLanguages.ContainsKey("fi")) ButtonLanguageSpanish.Visible = false;
    }


    protected void ButtonLanguageEnglish_Click (object sender, ImageMapEventArgs e)
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
		FireLanguageChanged();
	}

    protected void ButtonLanguageSwedish_Click (object sender, ImageMapEventArgs e)
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
		FireLanguageChanged();
	}

    protected void ButtonLanguageFrench_Click (object sender, ImageMapEventArgs e)
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
		FireLanguageChanged();
	}

    protected void ButtonLanguageGerman_Click (object sender, ImageMapEventArgs e)
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
		FireLanguageChanged();
	}

    protected void ButtonLanguageSpanish_Click (object sender, ImageMapEventArgs e)
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
		FireLanguageChanged();
	}

    protected void ButtonLanguageRussian_Click (object sender, ImageMapEventArgs e)
	{
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
		FireLanguageChanged();
	}

    protected void ButtonLanguageJapanese_Click (object sender, ImageMapEventArgs e)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ja-JP");
        FireLanguageChanged();
    }

    protected void ButtonLanguageFinnish_Click (object sender, ImageMapEventArgs e)
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fi-FI");
        FireLanguageChanged();
    }


	private void FireLanguageChanged()
	{
        HttpCookie kaka = new HttpCookie("PreferredCulture");
        kaka.Value = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToString();
        kaka.Expires = DateTime.Now.AddDays(365);
        Response.Cookies.Add(kaka);

		if (LanguageChanged != null)
		{
			LanguageChanged(this, new EventArgs());
		}
        Page.ClientScript.RegisterStartupScript(this.GetType(), "changedLanguage", "location=location;", true);
        
	}


	public event EventHandler LanguageChanged;

}
