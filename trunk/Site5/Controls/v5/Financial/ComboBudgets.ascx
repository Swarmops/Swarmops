<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ComboBudgets.ascx.cs" Inherits="Swarmops.Controls.Financial.BudgetCombo" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=this.ClientID %>_DropBudgets').combotree({
            animate: true,
            height: 30
        });

        $('#<%=this.ClientID %>_SpanBudgets span.combo input.combo-text').click(function () {
            $('#<%=this.ClientID %>_SpanBudgets span.combo span span.combo-arrow').click();
        });

    });
 </script>
 
 <span id="<%=this.ClientID %>_SpanBudgets"><select class="easyui-combotree" url="/Automation/Json-ExpensableBudgetsTree.aspx" name="DropBudgets" id="<%=this.ClientID %>_DropBudgets" animate="true" style="width:300px"></select></span>