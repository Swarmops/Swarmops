using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Logic.Communications;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Communications_PaperLetterInbox : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateGrid();
        }
    }


    private void PopulateGrid()
    {
        PaperLetters letters = PaperLetters.ForPerson(_currentUser);
        this.GridLetters.DataSource = letters;
    }


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.ExpenseDate, claim1.ExpenseDate);
    }



    protected void GridLetters_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            PaperLetter letter = (PaperLetter) e.Item.DataItem;

            if (letter == null)
            {
                return;
            }

            Controls_v4_DocumentList docList = (Controls_v4_DocumentList)e.Item.FindControl("DocumentList");

            if (docList != null)
            {
                docList.Documents = Documents.ForObject(letter);
            }

            Label labelAddress = (Label) e.Item.FindControl("LabelAddress");
            labelAddress.Text = String.Join("; ", letter.ReplyAddressLines);
        }

    }
}
