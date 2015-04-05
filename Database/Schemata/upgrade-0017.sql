CREATE PROCEDURE `SetCashAdvanceAmountCents`(
  IN cashAdvanceId INTEGER,
  IN amountCents INTEGER
)
BEGIN

  UPDATE CashAdvances SET CashAdvances.AmountCents=amountCents
    WHERE CashAdvances.CashAdvanceId=cashAdvanceId
      AND CashAdvances.PaidOut=0
      AND CashAdvances.Open=1;

  SELECT ROW_COUNT() AS RecordsUpdated;

END
