
DROP PROCEDURE IF EXISTS `SetExpenseClaimGroup`

#

DROP PROCEDURE IF EXISTS `SetExpenseClaimGroupClosed`

#


CREATE PROCEDURE `SetExpenseClaimGroupClosed` (
  IN expenseClaimGroupId INT
)
BEGIN
  DECLARE createdByPersonId INT;
  DECLARE organizationId INT;

  SELECT ExpenseClaimGroups.CreatedByPersonId
    INTO createdByPersonId FROM ExpenseClaimGroups
    WHERE ExpenseClaimGroups.ExpenseClaimGroupId = expenseClaimGroupId;

  SELECT ExpenseClaimGroups.OrganizationId
    INTO organizationId FROM ExpenseClaimGroups
    WHERE ExpenseClaimGroups.ExpenseClaimGroupId = expenseClaimGroupId;

  UPDATE ExpenseClaimGroups 
    SET ExpenseClaimGroups.Open = 0
    WHERE ExpenseClaimGroups.ExpenseClaimGroupId = expenseClaimGroupId;

  UPDATE ExpenseClaims
    SET ClaimingPersonId = createdByPersonId,
        OrganizationId = organizationId
    WHERE ExpenseClaims.ExpenseClaimGroupId = expenseClaimGroupId;

END

#

CREATE PROCEDURE `SetExpenseClaimGroup` (
  IN expenseClaimId INT,
  IN expenseClaimGroupId INT
)
BEGIN
  UPDATE ExpenseClaims
    SET ExpenseClaimGroupId=expenseClaimGroupId
    WHERE ExpenseClaims.ExpenseClaimId=expenseClaimId;
END


