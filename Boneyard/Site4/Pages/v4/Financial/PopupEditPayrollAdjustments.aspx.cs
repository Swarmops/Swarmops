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
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_PopupEditPayrollAdjustments : PageV4Base
{
    PayrollItem _payrollItem = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        _payrollItem = GetPayrollItem();

        if (!_authority.HasPermission(Permission.CanManagePayroll, _payrollItem.OrganizationId, -1, Authorization.Flag.ExactOrganization))
            throw new UnauthorizedAccessException("Access Denied");


        if (!Page.IsPostBack)
        {
            // Populate all data

            this.TextDescription.Style.Add(HtmlTextWriterStyle.Width, "200px");
            this.TextAmount.Style.Add(HtmlTextWriterStyle.Width, "80px");

            this.LabelPayroll.Text = _payrollItem.Person.Canonical;
            this.LabelMonth.Text = DateTime.Now.AddMonths(1).ToString("yyyy MMM");

            PopulateGrid();

        }

        Page.Title = "Editing payroll of " + _payrollItem.Person.Canonical;

    }

    private void PopulateGrid ()
    {
        List<PayrollLineItem> lines = new List<PayrollLineItem>();
        PayrollAdjustments adjustments = PayrollAdjustments.ForPayrollItem(_payrollItem);

        lines.Add(new PayrollLineItem("BASE SALARY", _payrollItem.BaseSalaryCents / 100.0));
        double pay = _payrollItem.BaseSalaryCents / 100.0;

        foreach (PayrollAdjustment adjustment in adjustments)
        {
            if (adjustment.Type == PayrollAdjustmentType.GrossAdjustment)
            {
                pay += adjustment.AmountCents / 100.0;
                lines.Add(new PayrollLineItem(adjustment.Description, adjustment.AmountCents / 100.0));
            }
        }

        lines.Add(new PayrollLineItem("GROSS SALARY", pay));

        double subtractiveTax = TaxLevels.GetTax(_payrollItem.Country, _payrollItem.SubtractiveTaxLevelId, pay);
        if (subtractiveTax < 1.0)
        {
            // this is a percentage and not an absolute number

            subtractiveTax = pay * subtractiveTax;
        }

        lines.Add(new PayrollLineItem("Income Tax", -subtractiveTax));
        pay -= subtractiveTax;

        foreach (PayrollAdjustment adjustment in adjustments)
        {
            if (adjustment.Type == PayrollAdjustmentType.NetAdjustment)
            {
                pay += adjustment.AmountCents / 100.0;
                lines.Add(new PayrollLineItem(adjustment.Description, adjustment.AmountCents / 100.0));
            }
        }

        if (pay < 0.0)
        {
            lines.Add(new PayrollLineItem("Negative amount rolling over to next salary", -pay));
            pay = 0.0;
        }

        lines.Add(new PayrollLineItem("NET SALARY PAYOUT", pay));

        this.GridProjectedPayroll.DataSource = lines;
        this.GridProjectedPayroll.DataBind();
    }

    private PayrollItem GetPayrollItem()
    {
        int payrollItemId = Int32.Parse(Request.QueryString["PayrollItemId"]);
        PayrollItem payrollItem = PayrollItem.FromIdentity(payrollItemId);

        return payrollItem;
    }


    protected void ButtonAdd_Click (object sender, EventArgs e)
    {

        if (!_authority.HasPermission(Permission.CanManagePayroll, _payrollItem.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to change payroll records.');", true);
            return;
        }

        if (!Page.IsValid)
        {
            return;
        }

        string description = this.TextDescription.Text;
        string amount = this.TextAmount.Text;
        PayrollAdjustmentType type = (PayrollAdjustmentType) Int32.Parse(this.DropType.SelectedValue);

        amount = amount.Replace(",", ".");  // compensate for cultures

        double adjustmentAmount;

        bool success = Double.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out adjustmentAmount);

        if (!success)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('Unable to parse the amount.');", true);
            return;
        }

        PayrollAdjustment.Create(_payrollItem, type, adjustmentAmount, description);

        this.TextDescription.Text = string.Empty;
        this.TextAmount.Text = string.Empty;
        this.DropType.SelectedIndex = 0;

        PopulateGrid();
    }


    class PayrollLineItem
    {
        public PayrollLineItem (string description, double amount)
        {
            this.description = description;
            this.amount = amount;
        }

        // .Net 2.0 so can't use auto properties

        public string Description
        {
            get { return this.description; }
        }
        public double Amount
        {
            get { return this.amount; }
        }

        private string description;
        private double amount;
    }



}