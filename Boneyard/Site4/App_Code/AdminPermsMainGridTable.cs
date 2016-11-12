using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using CellAddressable;
using Activizr.Basic.Enums;
using Activizr.Logic.Security;
using System.Text.RegularExpressions;


public class AdminPermsMainGridTable : CellAddressableTable
{
    public int firstcol = 1; //first role column

    public AdminPermsMainGridTable ()
        : base()
    {
    }

    class RoleSortKey : IComparable<RoleSortKey>
    {
        static Dictionary<RoleType, int> staticSortOrder = new Dictionary<RoleType, int>();

        static RoleSortKey ()
        {
            int order = 0;
            foreach (RoleType rt in RoleTypes.AllRoleTypes)
            {
                staticSortOrder[rt]= order++;
            }
            staticSortOrder[RoleType.Unknown]= order++;

        }
        public RoleSortKey (RoleType t)
        {
            role = t;
        }

        public RoleType role;

        public int CompareTo (RoleSortKey other)
        {
            if (!staticSortOrder.ContainsKey(this.role)) this.role = RoleType.Unknown;
            if (!staticSortOrder.ContainsKey(other.role)) other.role = RoleType.Unknown;

            return staticSortOrder[this.role].CompareTo(staticSortOrder[other.role]);
        }

    }

    SortedList<RoleSortKey, Column> Roles = new SortedList<RoleSortKey, Column>();


    public Column AddRole (RoleType role)
    {
        int col = firstcol; //first role column

        RoleSortKey newKey = new RoleSortKey(role);
        foreach (RoleSortKey t in Roles.Keys)
        {
            if (t.CompareTo(newKey) == 1)
            {
                if (col - firstcol < Roles.Count)
                    this.InsertColumnAt(col);
                break;
            }
            ++col;
        }

        Column newColumn = this.Columns[col];
        Roles.Add(newKey, newColumn);

        if (col > firstcol)
        {
            this[0, firstcol].JoinCell(CellJoinDirection.RIGHT);
        }
        else
        {
            this[0, firstcol].AddClass("topLock").AddClass("gridCell").AddClass("rolecell");
        }

        string[] name = SplitInWords(Enum.GetName(role.GetType(), role));

        newColumn[1].Cell.InnerHtml = name[0];
        name[0] = "";
        newColumn[2].Cell.InnerHtml = (string.Join("<br />", name) + "       ").Substring(6).Trim();
        newColumn[2].Cell.Attributes["roleID"] = ((int)role).ToString();
        newColumn[2].Cell.ID = "R" + ((int)role).ToString();
        //CellAdressableCell c = this._Cells[3, col];
        //string cbName = "cb_" + ((int)role).ToString() + "_all";
        //c.Cell.InnerHtml = "<input type='checkbox' id='" + cbName + "' name='cbRoleAll' class='cb' value='" + cbName + "' ";
        //c.Cell.InnerHtml += " onclick='cbRoleClick(this," + ((int)role).ToString() + ")'";

        //c.Cell.InnerHtml += " >";
        //c.AddClass("rolecell");




        return newColumn;

    }
    private string[] SplitInWords (string p)
    {
        Regex re = new Regex(@"([a-zåäö])([A-ZÅÄÖ])");
        return re.Replace(p, "$1 $2").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public Row AddPermission (Permission perm)
    {
        Row newRow = this.Rows[this.Rows.Count];

        newRow[0].Cell.InnerHtml = Enum.GetName(perm.GetType(), perm);
        newRow[0].Cell.Attributes["permID"] = ((int)perm).ToString();
        newRow[0].Cell.ID = "P" + ((int)perm).ToString();

        newRow[0].Cell.Attributes["nowrap"] = "nowrap";
        return newRow;
    }

    public CellAdressableCell AddResult (RoleType role, Permission perm, bool isSet, bool editable)
    {
        int row = 0;
        for (row = 0; row < this.Rows.Count; ++row)
        {
            if (this.Rows[row][0].Cell.Attributes["permID"] == ((int)perm).ToString())
                break;
        }

        int col = 0;
        for (col = 0; col < this.Columns.Count; ++col)
        {
            if (this.Columns[col][2].Cell.Attributes["roleID"] == ((int)role).ToString())
                break;
        }


        CellAdressableCell c = this._Cells[row, col];
        string cbName = "cb_" + ((int)role).ToString() + "_" + ((int)perm).ToString();
        c.Cell.InnerHtml = "<input type='checkbox' id='" + cbName + "' name='cbPerm' class='cb' value='" + cbName + "' " + (isSet ? " CHECKED " : "");
        if (editable)
            c.Cell.InnerHtml += " onclick='cbPerClick(this," + ((int)role).ToString() + "," + ((int)perm).ToString() + ")'";
        else
            c.Cell.InnerHtml += " DISABLED ";
        c.Cell.InnerHtml += " >";
        c.AddClass("rolecell");
        c.Cell.Attributes["onmouseover"] = "onMouseEnterResult(" + ((int)role).ToString() + "," + ((int)perm).ToString() + ");";
        c.Cell.Attributes["onmouseout"] = "onMouseLeaveResult(" + ((int)role).ToString() + "," + ((int)perm).ToString() + ");";

        return null;
    }



}


