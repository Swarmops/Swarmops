using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Enums;
using Activizr.Logic.Structure;
using Activizr.Logic.Security;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;
using System.Globalization;
using System.Web.Configuration;
using System.Text;
using Activizr.Basic.Types;
using Activizr.Database;
using System.Transactions;

public partial class Pages_Special_FI_Import : PageV4Base
{
    protected Country loadCntry = null;
    protected Dictionary<int, List<string>> errRows = new Dictionary<int, List<string>>();
    protected Geographies geotree = new Geographies();
    protected Organization currentOrg = null;
    protected DateTime nowValue;



    protected void Page_Load (object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanEditMemberships, Organization.PPFIid, Geography.FinlandId, Authorization.Flag.ExactGeographyExactOrganization))
        {
            loadCntry = Country.FromCode("FI");
            geotree = loadCntry.Geography.GetTree();
            nowValue = DateTime.Now;
            if (!IsPostBack)
            {
                foreach (Organization org in Organizations.GetAll())
                {
                    if (org.DefaultCountryId == loadCntry.Identity)
                    {
                        DropDownListOrgs.Items.Add(new ListItem(org.Name, "" + org.Identity));
                    }
                }
            }
            else
            {
                currentOrg = Organization.FromIdentity(Convert.ToInt32(DropDownListOrgs.SelectedValue));
            }
        }
        else
        {
            Response.Write("Access not allowed");
            Response.End();
        }
    }
    protected void Button1_Click (object sender, EventArgs e)
    {
        int col_firstNames = 0;
        int col_lastName = 1;
        int col_dateOfBirth = 2;
        int col_email = 3;
        int col_municipality = 4;
        int col_address = 5;
        int col_postalCode = 6;
        int col_city = 7;
        int col_phone = 8;
        int col_dateJoined = 9;
        int col_active = 10;

        int currentRow = -1;
        int currentImported = 0;

        DateTime T0 = DateTime.Now;

        Dictionary<string, BasicCity> postcodes = PirateDb.GetDatabase().GetCitiesPerPostalCode(loadCntry.Identity);
        Dictionary<string, BasicCity> cityPerName = new Dictionary<string, BasicCity>();
        Dictionary<int, BasicCity> cityPerId = new Dictionary<int, BasicCity>();
        foreach (BasicCity bc in postcodes.Values)
        {
            cityPerName[bc.Name.ToLower().Replace(" ", "")] = bc;
            cityPerId[bc.Identity] = bc;
        }

        People allpeople = People.GetAll();
        Dictionary<string, Person> peoplePerKey = new Dictionary<string, Person>();
        foreach (Person p in allpeople)
            peoplePerKey[p.Email.ToLower().Replace(" ", "") + p.Birthdate.ToString("yyMMdd")] = p;

        Memberships memberships = Memberships.ForOrganization(currentOrg);
        Dictionary<int, Membership> membershipsDict = new Dictionary<int, Membership>();
        foreach (Membership ms in memberships)
            membershipsDict[ms.PersonId] = ms;

        string[] rows = TextBoxImport.Text.Replace("\r\n", "\n").Split('\n');
        using (TransactionScope txScope = new TransactionScope(TransactionScopeOption.Required,new TimeSpan(0,30,0)))
        {
            foreach (string row in rows)
            {
                ++currentRow;
                string[] cols = (row + "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t").Split('\t');

                Geography mainGeo = null;
                string name = cols[col_firstNames] + " " + cols[col_lastName];
                if (!Formatting.ValidateEmailFormat(cols[col_email]))
                {
                    AddError(currentRow, "Bad email format:" + cols[col_email]);
                    continue;
                }
                BasicCity foundCity = null;
                Dictionary<int, Geography> geos = new Dictionary<int, Geography>();
                string pcode = cols[col_postalCode].Trim();
                if (pcode != "")
                {
                    while (pcode.Length < loadCntry.PostalCodeLength)
                        pcode = "0" + pcode;

                    if (!postcodes.ContainsKey(pcode))
                    {
                        AddError(currentRow, "Invalid postal code:" + pcode);
                    }
                    else
                    {
                        foundCity = postcodes[pcode];
                        mainGeo = Geography.FromIdentity(foundCity.GeographyId);
                        geos[foundCity.GeographyId] = mainGeo;
                    }
                }
                else if (cols[col_city].Trim() != "")
                {
                    if (!cityPerName.ContainsKey(cols[col_city].ToLower().Replace(" ", "")))
                    {
                        AddError(currentRow, "Invalid postal code:" + pcode);
                    }
                    else
                    {
                        foundCity = cityPerName[cols[col_city].ToLower().Replace(" ", "")];
                        mainGeo = Geography.FromIdentity(foundCity.GeographyId);
                        geos[foundCity.GeographyId] = mainGeo;
                    }

                }

                foreach (Geography g in geotree)
                {

                    string[] names = g.Name.ToLower().Replace(" ", "").Split('/');
                    foreach (string partName in names)
                    {
                        if (partName == cols[col_municipality].ToLower().Replace(" ", ""))
                        {
                            mainGeo = g;
                            geos[g.Identity] = g;
                        }
                    }
                }

                if (geos.Count == 0 || geos.Count > 1)
                {
                    AddError(currentRow, "Warning only: can not find a specific local geography");
                }


                DateTime dob = NormalizeDate(cols[col_dateOfBirth]);
                DateTime doj = NormalizeDate(cols[col_dateJoined]);
                string key = cols[col_email].ToLower().Replace(" ", "") + dob.ToString("yyMMdd");
                Person currentPerson = null;

                if (!peoplePerKey.ContainsKey(key))
                {
                    if (mainGeo == null)
                    {
                        mainGeo = loadCntry.Geography;
                    }
                    currentPerson = Person.Create(name, cols[col_email], "ABCABCABCABC", cols[col_phone], cols[col_address], pcode, cols[col_city], loadCntry.Code, dob, PersonGender.Unknown);
                    currentPerson.Geography = mainGeo;
                    PWLog.Write(PWLogItem.Person, currentPerson.Identity, PWLogAction.PersonCreated, "Created Person from Import", "Import for " + currentOrg.Name);
                }
                else
                {
                    currentPerson = peoplePerKey[key];
                    AddError(currentRow, "Warning only: Person with email already existed ");

                    if (currentPerson.Birthdate < new DateTime(1901, 1, 1)) currentPerson.Birthdate = dob;
                    if (currentPerson.Phone.Length < cols[col_phone].Length) currentPerson.Phone = cols[col_phone];
                    if (currentPerson.Street.Length < cols[col_address].Length) currentPerson.Street = cols[col_address];
                    if (currentPerson.PostalCode.CompareTo(pcode) < 0) currentPerson.PostalCode = pcode;
                    if (currentPerson.CityName.Length < cols[col_city].Length) currentPerson.CityName = cols[col_city];

                    if (mainGeo != null && mainGeo.Identity != currentPerson.GeographyId)
                    {
                        currentPerson.Geography = mainGeo;
                    }
                }

                // add membership
                if (!membershipsDict.ContainsKey(currentPerson.Identity))
                {
                    Membership newMs = Membership.Import(currentPerson, currentOrg, doj, nowValue.AddYears(100));
                    newMs.SetPaymentStatus(MembershipPaymentStatus.PaymentRecieved, nowValue);
                }

                // add activist
                if (cols[col_active] == "1")
                {
                    currentPerson.CreateActivist(true, true);
                    PWLog.Write(currentPerson, PWLogItem.Person, currentPerson.Identity, PWLogAction.ActivistJoin, "New activist joined.", "Import for " + currentOrg.Name);

                }

                ++currentImported;
            }
            txScope.Complete();
        }

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("ProcessTime= " + Math.Round(DateTime.Now.Subtract(T0).TotalSeconds));
        sb.AppendLine("Rows read= " + currentRow);
        sb.AppendLine("Rows imported= " + currentImported);
        sb.AppendLine("Errors and warnings");
        foreach (int row in errRows.Keys)
        {
            sb.AppendLine("Line: " + row);
            sb.AppendLine(rows[row]);

            foreach (string err in errRows[row])
            {
                sb.AppendLine("     " + err);
            }
            sb.AppendLine("");
        }
        TextBoxResult.Text = sb.ToString();
    }

    private void AddError (int currentRow, string p)
    {
        if (!errRows.ContainsKey(currentRow))
            errRows[currentRow] = new List<string>();
        errRows[currentRow].Add(p);
    }

    private DateTime NormalizeDate (string p)
    {
        DateTime temp = new DateTime(1900, 1, 1, 0, 0, 0);
        DateTime.TryParseExact(p, new string[]{
                        "yyyy-MM-dd","yyyy-MM-dd HH:mm:ss","yyyy-MM-dd HHmm:ss",
                        "dd.MM.yyyy","dd.MM.yyyy HH:mm:ss","dd.MM.yyyy HHmm:ss"
                        },
                    DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AllowWhiteSpaces,
                     out temp);


        return temp;
    }


}
