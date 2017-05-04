DROP PROCEDURE IF EXISTS `SetExpenseClaimVatCents`


#


CREATE PROCEDURE `SetExpenseClaimVatCents`(
  IN expenseClaimId INTEGER,
  IN vatCents BIGINT
)
BEGIN

 UPDATE ExpenseClaims SET ExpenseClaims.VatCents=vatCents
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;

END
