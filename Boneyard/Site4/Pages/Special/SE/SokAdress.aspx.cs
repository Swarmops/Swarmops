using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Logic.Pirates;
using System.Text;
using System.Globalization;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using System.Web.UI.HtmlControls;
using Activizr.Logic.Structure;
using System.Text.RegularExpressions;

public partial class Pages_Special_SE_SokAdress : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {

    }
    protected void Button1_Click (object sender, EventArgs e)
    {

        string contents = TextBox1.Text;
        contents = contents.Replace("\r", "\n");
        string[] rows = contents.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, bool> usedPostcodesHash = new Dictionary<string, bool>();
        Dictionary<string, string> addresses = new Dictionary<string, string>();
        Dictionary<string, string[]> addresses2 = new Dictionary<string, string[]>();
        Dictionary<int, string> personHit = new Dictionary<int, string>();
        Regex rePostadress = new Regex(@"(?<postnr>([\s0-9]+))(?<ort>.*)");
        foreach (string row in rows)
        {
            string[] cols = (row + "\t\t").Split('\t');
            Match match = rePostadress.Match(cols[1]);
            string pcode = match.Groups["postnr"].Value.Trim().ToLower().Replace(" ", "");
            usedPostcodesHash[pcode] = true;
            cols[0] = RemoveDiacritics(cols[0].ToLower().Replace(" ", ""));
            if (cols[0] != "")
            {
                addresses[cols[0]] = pcode;
                addresses2[cols[0]] = cols;
            }

        }

        string[] usedPostcodes = new string[usedPostcodesHash.Count]; ;
        usedPostcodesHash.Keys.CopyTo(usedPostcodes, 0);

        People searchResults = People.FromPostalCodes(usedPostcodes);


        searchResults = searchResults.Filter(
            delegate(Person p)
            {
                string testAddress = RemoveDiacritics(("" + p.Street).ToLower().Replace(" ", ""));
                if (addresses.ContainsKey(testAddress) && addresses[testAddress] == ("" + p.PostalCode).Trim().ToLower().Replace(" ", ""))
                {
                    personHit[p.Identity] = testAddress;
                    return true;
                }
                else
                {
                    foreach (string key in addresses.Keys)
                    {
                        if (testAddress.StartsWith(key))
                        {
                            personHit[p.Identity] = key;
                            return true;
                        }
                    }
                }
                return false;
            });


        searchResults = searchResults.GetVisiblePeopleByAuthority(_authority, 40).RemoveUnlisted();
        
        HtmlTableRow tr = new HtmlTableRow();
        this.outputTab.Rows.Add(tr);
        HtmlTableCell td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "träffadress";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "träffort";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "namn";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "adress";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "postnr";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "email";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "telefon";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "födelsedat";
        td = new HtmlTableCell(); tr.Cells.Add(td);
        td.InnerText = "medlemskap går ut";
        foreach (Person p in searchResults)
        {
            Membership ms = p.GetRecentMembership(35, Organization.PPSEid);
            if (ms != null)
            {
            string[] cols=addresses2[personHit[p.Identity]];
                 tr = new HtmlTableRow();
                this.outputTab.Rows.Add(tr);
                 td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = cols[0];
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = cols[1];
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = p.Name;
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = p.Street;
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = p.PostalCode;
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = p.Email;
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = p.Phone;
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = p.Birthdate.ToString("yyyy-MM-dd");
                td = new HtmlTableCell(); tr.Cells.Add(td);
                td.InnerText = ms.Expires.ToString("yyyy-MM-dd");
            }
        }

    }


    private static String RemoveDiacritics (string s)
    {
        string normalizedString = s.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < normalizedString.Length; i++)
        {
            char c = normalizedString[i];
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }
}
