CREATE PROCEDURE `SetCashAdvanceBudget`(
  IN cashAdvanceId INTEGER,
  IN budgetId INTEGER
)
BEGIN

  UPDATE CashAdvances SET CashAdvances.FinancialAccountId=budgetId
    WHERE CashAdvances.CashAdvanceId=cashAdvanceId;

END
